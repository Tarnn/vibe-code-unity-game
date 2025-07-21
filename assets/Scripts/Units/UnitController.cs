using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using FrostRealm.Data;

namespace FrostRealm.Units
{
    /// <summary>
    /// Controls individual unit behavior in FrostRealm Chronicles.
    /// Handles movement, combat, abilities, and AI following Warcraft III mechanics.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitController : MonoBehaviour
    {
        [Header("Unit Configuration")]
        [SerializeField] private UnitData unitData;
        [SerializeField] private int playerOwner = 0;
        [SerializeField] private bool isSelected = false;
        
        [Header("Current Stats")]
        [SerializeField] private float currentHP;
        [SerializeField] private float currentMP;
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentArmor;
        [SerializeField] private int currentDamage;
        
        [Header("Combat")]
        [SerializeField] private UnitController currentTarget;
        [SerializeField] private float lastAttackTime;
        [SerializeField] private bool isAttacking;
        
        [Header("Components")]
        private NavMeshAgent navAgent;
        private Animator animator;
        private AudioSource audioSource;
        private GameObject selectionCircle;
        
        // State management
        private UnitState currentState = UnitState.Idle;
        private Vector3 moveTarget;
        private List<Vector3> patrolPath = new List<Vector3>();
        private int currentPatrolIndex = 0;
        
        // Events
        public System.Action<UnitController> OnUnitDeath;
        public System.Action<UnitController, float> OnUnitDamaged;
        public System.Action<UnitController> OnUnitSelected;
        public System.Action<UnitController> OnUnitDeselected;
        
        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            CreateSelectionCircle();
        }
        
        private void Start()
        {
            if (unitData != null)
            {
                InitializeFromData();
            }
        }
        
        private void Update()
        {
            UpdateState();
            UpdateCombat();
            UpdateAnimation();
            UpdateDayNightVision();
        }
        
        public void InitializeFromData()
        {
            if (unitData == null) return;
            
            // Set stats
            currentHP = unitData.hitPoints;
            currentMP = unitData.manaPoints;
            currentArmor = unitData.armor;
            currentDamage = unitData.damage;
            
            // Configure NavMeshAgent
            navAgent.speed = unitData.moveSpeed / 100f; // Convert from WC3 units
            navAgent.angularSpeed = unitData.turnRate * 180f;
            navAgent.stoppingDistance = unitData.unitType == UnitType.Melee ? 1.5f : unitData.attackRange / 100f;
            navAgent.acceleration = 8f;
            
            // Set name
            gameObject.name = $"{unitData.unitName} (P{playerOwner})";
            
            // Load model if specified
            if (unitData.modelPrefab != null)
            {
                GameObject model = Instantiate(unitData.modelPrefab, transform);
                model.transform.localScale = Vector3.one * unitData.modelScale;
                animator = model.GetComponent<Animator>();
            }
            
            // Set animator controller
            if (animator != null && unitData.animatorController != null)
            {
                animator.runtimeAnimatorController = unitData.animatorController;
            }
        }
        
        private void CreateSelectionCircle()
        {
            selectionCircle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            selectionCircle.name = "Selection Circle";
            selectionCircle.transform.parent = transform;
            selectionCircle.transform.localPosition = Vector3.up * 0.01f;
            selectionCircle.transform.localScale = new Vector3(2f, 0.01f, 2f);
            
            // Remove collider
            Destroy(selectionCircle.GetComponent<Collider>());
            
            // Set material
            Material mat = new Material(Shader.Find("HDRP/Unlit"));
            mat.color = GetPlayerColor(playerOwner);
            selectionCircle.GetComponent<Renderer>().material = mat;
            
            selectionCircle.SetActive(false);
        }
        
        private Color GetPlayerColor(int player)
        {
            Color[] playerColors = new Color[]
            {
                Color.red,
                Color.blue,
                Color.cyan,
                new Color(0.5f, 0f, 0.5f), // Purple
                Color.yellow,
                new Color(1f, 0.5f, 0f), // Orange
                Color.green,
                new Color(1f, 0.5f, 1f), // Pink
                Color.gray,
                new Color(0.5f, 0.75f, 1f), // Light Blue
                new Color(0f, 0.5f, 0.5f), // Dark Green
                new Color(0.5f, 0.25f, 0f) // Brown
            };
            
            return player < playerColors.Length ? playerColors[player] : Color.white;
        }
        
        #region State Management
        
        private void UpdateState()
        {
            switch (currentState)
            {
                case UnitState.Idle:
                    // Check for enemies in acquisition range
                    if (unitData.unitType != UnitType.Worker)
                    {
                        CheckForEnemies();
                    }
                    break;
                    
                case UnitState.Moving:
                    if (navAgent.remainingDistance < 0.1f)
                    {
                        SetState(UnitState.Idle);
                    }
                    break;
                    
                case UnitState.Attacking:
                    if (currentTarget == null || currentTarget.IsDead())
                    {
                        SetState(UnitState.Idle);
                    }
                    else if (Vector3.Distance(transform.position, currentTarget.transform.position) > unitData.attackRange / 100f + 0.5f)
                    {
                        // Chase target
                        navAgent.SetDestination(currentTarget.transform.position);
                    }
                    break;
                    
                case UnitState.Patrolling:
                    UpdatePatrol();
                    break;
                    
                case UnitState.Fleeing:
                    if (currentHP / unitData.hitPoints > unitData.fleeHealthPercent * 2f)
                    {
                        SetState(UnitState.Idle);
                    }
                    break;
            }
        }
        
        private void SetState(UnitState newState)
        {
            if (currentState == newState) return;
            
            currentState = newState;
            
            switch (newState)
            {
                case UnitState.Idle:
                    navAgent.isStopped = true;
                    isAttacking = false;
                    break;
                    
                case UnitState.Moving:
                    navAgent.isStopped = false;
                    isAttacking = false;
                    break;
                    
                case UnitState.Attacking:
                    isAttacking = true;
                    break;
            }
        }
        
        #endregion
        
        #region Movement
        
        public void MoveTo(Vector3 position)
        {
            moveTarget = position;
            navAgent.SetDestination(position);
            SetState(UnitState.Moving);
            
            PlaySound(unitData.moveSounds);
        }
        
        public void AttackMove(Vector3 position)
        {
            MoveTo(position);
            // Will auto-acquire targets while moving
        }
        
        public void Patrol(List<Vector3> path)
        {
            if (path.Count < 2) return;
            
            patrolPath = new List<Vector3>(path);
            currentPatrolIndex = 0;
            MoveTo(patrolPath[0]);
            SetState(UnitState.Patrolling);
        }
        
        private void UpdatePatrol()
        {
            if (navAgent.remainingDistance < 0.5f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPath.Count;
                MoveTo(patrolPath[currentPatrolIndex]);
            }
        }
        
        public void Stop()
        {
            navAgent.isStopped = true;
            SetState(UnitState.Idle);
        }
        
        #endregion
        
        #region Combat
        
        public void Attack(UnitController target)
        {
            if (target == null || target == this) return;
            if (target.playerOwner == playerOwner) return; // Can't attack allies
            
            currentTarget = target;
            SetState(UnitState.Attacking);
            
            PlaySound(unitData.attackSounds);
        }
        
        private void UpdateCombat()
        {
            if (!isAttacking || currentTarget == null) return;
            
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            
            if (distance <= unitData.attackRange / 100f)
            {
                navAgent.isStopped = true;
                
                // Face target
                Vector3 lookDir = (currentTarget.transform.position - transform.position).normalized;
                lookDir.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * unitData.turnRate);
                
                // Attack if cooldown ready
                if (Time.time - lastAttackTime >= unitData.attackSpeed)
                {
                    PerformAttack();
                }
            }
            else
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(currentTarget.transform.position);
            }
        }
        
        private void PerformAttack()
        {
            lastAttackTime = Time.time;
            
            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // Deal damage after attack point
            Invoke(nameof(DealDamage), unitData.attackPoint);
        }
        
        private void DealDamage()
        {
            if (currentTarget == null || currentTarget.IsDead()) return;
            
            // Calculate damage with random bonus
            float damage = currentDamage + Random.Range(0, unitData.damageRandomBonus + 1);
            
            // Apply damage
            currentTarget.TakeDamage(damage, unitData.damageType);
            
            // Create damage effect
            CreateDamageEffect(currentTarget.transform.position);
        }
        
        public void TakeDamage(float damage, DamageType damageType)
        {
            // Calculate actual damage with armor reduction
            float actualDamage = DamageCalculator.CalculateDamage(damage, currentArmor, damageType, unitData.armorType);
            
            currentHP -= actualDamage;
            OnUnitDamaged?.Invoke(this, actualDamage);
            
            // Show damage number
            ShowDamageNumber(actualDamage);
            
            if (currentHP <= 0)
            {
                Die();
            }
            else if (unitData.canFlee && currentHP / unitData.hitPoints <= unitData.fleeHealthPercent)
            {
                Flee();
            }
        }
        
        private void CheckForEnemies()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, unitData.acquisitionRange / 100f);
            
            foreach (Collider col in colliders)
            {
                UnitController unit = col.GetComponent<UnitController>();
                if (unit != null && unit.playerOwner != playerOwner && !unit.IsDead())
                {
                    Attack(unit);
                    break;
                }
            }
        }
        
        private void Flee()
        {
            SetState(UnitState.Fleeing);
            
            // Find safe position away from enemies
            Vector3 fleeDirection = Vector3.zero;
            Collider[] colliders = Physics.OverlapSphere(transform.position, unitData.sightRangeDay / 100f);
            
            foreach (Collider col in colliders)
            {
                UnitController unit = col.GetComponent<UnitController>();
                if (unit != null && unit.playerOwner != playerOwner)
                {
                    fleeDirection += (transform.position - unit.transform.position).normalized;
                }
            }
            
            if (fleeDirection != Vector3.zero)
            {
                Vector3 fleeTarget = transform.position + fleeDirection.normalized * 10f;
                MoveTo(fleeTarget);
            }
        }
        
        #endregion
        
        #region Death and Effects
        
        private void Die()
        {
            currentHP = 0;
            isAttacking = false;
            navAgent.enabled = false;
            
            // Trigger death animation
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
            
            PlaySound(unitData.deathSounds);
            
            // Notify listeners
            OnUnitDeath?.Invoke(this);
            
            // Remove selection
            SetSelected(false);
            
            // Create death effect
            CreateDeathEffect();
            
            // Destroy after delay
            Destroy(gameObject, 3f);
        }
        
        public bool IsDead()
        {
            return currentHP <= 0;
        }
        
        private void CreateDamageEffect(Vector3 position)
        {
            // Create simple damage particle effect
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = position + Vector3.up;
            effect.transform.localScale = Vector3.one * 0.3f;
            
            Material mat = new Material(Shader.Find("HDRP/Unlit"));
            mat.color = Color.red;
            effect.GetComponent<Renderer>().material = mat;
            
            Destroy(effect.GetComponent<Collider>());
            Destroy(effect, 0.5f);
        }
        
        private void CreateDeathEffect()
        {
            // Create death particle effect
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = transform.position + Vector3.up;
            effect.transform.localScale = Vector3.one * 2f;
            
            Material mat = new Material(Shader.Find("HDRP/Unlit"));
            mat.color = new Color(0.5f, 0f, 0f, 0.5f);
            effect.GetComponent<Renderer>().material = mat;
            
            Destroy(effect.GetComponent<Collider>());
            Destroy(effect, 2f);
        }
        
        private void ShowDamageNumber(float damage)
        {
            // Create floating damage text
            GameObject damageText = new GameObject("Damage Text");
            damageText.transform.position = transform.position + Vector3.up * 2f;
            
            TextMesh text = damageText.AddComponent<TextMesh>();
            text.text = Mathf.RoundToInt(damage).ToString();
            text.fontSize = 24;
            text.color = Color.yellow;
            text.alignment = TextAlignment.Center;
            text.anchor = TextAnchor.MiddleCenter;
            
            // Animate upward and fade
            StartCoroutine(AnimateDamageText(damageText));
        }
        
        private System.Collections.IEnumerator AnimateDamageText(GameObject textObj)
        {
            float duration = 1f;
            float elapsed = 0f;
            Vector3 startPos = textObj.transform.position;
            TextMesh text = textObj.GetComponent<TextMesh>();
            Color startColor = text.color;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                textObj.transform.position = startPos + Vector3.up * t * 2f;
                text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
                
                yield return null;
            }
            
            Destroy(textObj);
        }
        
        #endregion
        
        #region Selection
        
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            if (selectionCircle != null)
            {
                selectionCircle.SetActive(selected);
            }
            
            if (selected)
            {
                OnUnitSelected?.Invoke(this);
                PlaySound(unitData.selectionSounds);
            }
            else
            {
                OnUnitDeselected?.Invoke(this);
            }
        }
        
        public bool IsSelected()
        {
            return isSelected;
        }
        
        #endregion
        
        #region Animation and Audio
        
        private void UpdateAnimation()
        {
            if (animator == null) return;
            
            // Set movement speed
            float moveSpeed = navAgent.velocity.magnitude;
            animator.SetFloat("MoveSpeed", moveSpeed);
            
            // Set combat state
            animator.SetBool("InCombat", isAttacking);
        }
        
        private void PlaySound(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0 || audioSource == null) return;
            
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        #endregion
        
        #region Vision
        
        private void UpdateDayNightVision()
        {
            // Update sight range based on day/night
            float currentSightRange = IsDayTime() ? unitData.sightRangeDay : unitData.sightRangeNight;
            
            // Night Elf units get bonus at night
            if (unitData.race == Race.NightElf && !IsDayTime())
            {
                currentSightRange *= 1.5f;
            }
        }
        
        private bool IsDayTime()
        {
            // Simple day/night check - can be expanded with proper day/night system
            return true; // For now, always day
        }
        
        #endregion
        
        #region Getters
        
        public UnitData GetUnitData() => unitData;
        public int GetPlayerOwner() => playerOwner;
        public float GetCurrentHP() => currentHP;
        public float GetCurrentMP() => currentMP;
        public float GetHPPercent() => currentHP / unitData.hitPoints;
        public float GetMPPercent() => unitData.manaPoints > 0 ? currentMP / unitData.manaPoints : 0f;
        public UnitState GetState() => currentState;
        
        #endregion
    }
    
    public enum UnitState
    {
        Idle,
        Moving,
        Attacking,
        Casting,
        Patrolling,
        Following,
        Fleeing,
        Dead
    }
}