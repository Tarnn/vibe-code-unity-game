using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostRealm.Core;
using FrostRealm.Data;

namespace FrostRealm.UI
{
    /// <summary>
    /// Controls the character selection screen UI using Unity UI Toolkit.
    /// Manages hero selection, preview display, and navigation.
    /// </summary>
    public class CharacterSelectionController : MonoBehaviour
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument uiDocument;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip selectionSound;
        [SerializeField] private AudioClip confirmSound;
        [SerializeField] private AudioClip cancelSound;
        
        // UI Elements
        private VisualElement rootElement;
        private VisualElement heroGrid;
        private VisualElement heroPortrait;
        private Label heroName;
        private Label heroClass;
        private Label heroFaction;
        private Label heroType;
        private Label heroDescription;
        private Label strengthValue;
        private Label agilityValue;
        private Label intelligenceValue;
        private VisualElement abilitiesGrid;
        private Button backButton;
        private Button startButton;
        
        // State
        private List<HeroCard> heroCards = new List<HeroCard>();
        private HeroData selectedHero;
        private int selectedIndex = 0;
        
        void Start()
        {
            InitializeUI();
            PopulateHeroGrid();
            SetupInputHandlers();
            
            // Set initial selection if heroes are available
            if (HeroRegistry.Instance.HeroCount > 0)
            {
                SelectHero(0);
            }
        }
        
        void OnEnable()
        {
            // Subscribe to input events
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnNavigate += HandleNavigation;
                InputManager.Instance.OnSubmit += HandleSubmit;
                InputManager.Instance.OnCancel += HandleCancel;
                InputManager.Instance.OnClick += HandleClick;
            }
        }
        
        void OnDisable()
        {
            // Unsubscribe from input events
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnNavigate -= HandleNavigation;
                InputManager.Instance.OnSubmit -= HandleSubmit;
                InputManager.Instance.OnCancel -= HandleCancel;
                InputManager.Instance.OnClick -= HandleClick;
            }
        }
        
        /// <summary>
        /// Initializes UI elements and references.
        /// </summary>
        private void InitializeUI()
        {
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }
            
            rootElement = uiDocument.rootVisualElement;
            
            // Get UI element references
            heroGrid = rootElement.Q<VisualElement>("hero-grid");
            heroPortrait = rootElement.Q<VisualElement>("hero-portrait");
            heroName = rootElement.Q<Label>("hero-name");
            heroClass = rootElement.Q<Label>("hero-class");
            heroFaction = rootElement.Q<Label>("hero-faction");
            heroType = rootElement.Q<Label>("hero-type");
            heroDescription = rootElement.Q<Label>("hero-description");
            strengthValue = rootElement.Q<Label>("strength-value");
            agilityValue = rootElement.Q<Label>("agility-value");
            intelligenceValue = rootElement.Q<Label>("intelligence-value");
            abilitiesGrid = rootElement.Q<VisualElement>("abilities-grid");
            backButton = rootElement.Q<Button>("back-button");
            startButton = rootElement.Q<Button>("start-button");
            
            // Initially disable start button
            startButton.SetEnabled(false);
        }
        
        /// <summary>
        /// Populates the hero grid with available heroes.
        /// </summary>
        private void PopulateHeroGrid()
        {
            heroGrid.Clear();
            heroCards.Clear();
            
            var heroRegistry = HeroRegistry.Instance;
            if (heroRegistry == null || heroRegistry.HeroCount == 0)
            {
                Debug.LogWarning("No heroes available in registry!");
                return;
            }
            
            for (int i = 0; i < heroRegistry.HeroCount; i++)
            {
                var hero = heroRegistry.GetHero(i);
                if (hero == null) continue;
                
                var heroCard = CreateHeroCard(hero, i);
                heroGrid.Add(heroCard.Element);
                heroCards.Add(heroCard);
            }
        }
        
        /// <summary>
        /// Creates a hero card UI element.
        /// </summary>
        private HeroCard CreateHeroCard(HeroData hero, int index)
        {
            var cardElement = new VisualElement();
            cardElement.AddToClassList("hero-card");
            cardElement.focusable = true;
            cardElement.tabIndex = index;
            
            // Portrait
            var portrait = new VisualElement();
            portrait.AddToClassList("hero-card-portrait");
            if (hero.Portrait != null)
            {
                portrait.style.backgroundImage = new StyleBackground(hero.Portrait);
            }
            cardElement.Add(portrait);
            
            // Info container
            var infoContainer = new VisualElement();
            infoContainer.AddToClassList("hero-card-info");
            
            var nameLabel = new Label(hero.HeroName);
            nameLabel.AddToClassList("hero-card-name");
            infoContainer.Add(nameLabel);
            
            var classLabel = new Label(hero.HeroClass.ToString());
            classLabel.AddToClassList("hero-card-class");
            infoContainer.Add(classLabel);
            
            cardElement.Add(infoContainer);
            
            // Add click handler
            cardElement.RegisterCallback<ClickEvent>(evt => SelectHero(index));
            cardElement.RegisterCallback<FocusInEvent>(evt => SelectHero(index));
            
            return new HeroCard { Element = cardElement, Hero = hero, Index = index };
        }
        
        /// <summary>
        /// Sets up button click handlers.
        /// </summary>
        private void SetupInputHandlers()
        {
            backButton.clicked += () => {
                PlaySound(cancelSound);
                GameManager.Instance.LoadMainMenu();
            };
            
            startButton.clicked += () => {
                if (selectedHero != null)
                {
                    PlaySound(confirmSound);
                    GameManager.Instance.SelectHero(selectedHero);
                    GameManager.Instance.LoadGameplay();
                }
            };
        }
        
        /// <summary>
        /// Handles keyboard/gamepad navigation.
        /// </summary>
        private void HandleNavigation(Vector2 navigation)
        {
            if (heroCards.Count == 0) return;
            
            int newIndex = selectedIndex;
            
            if (navigation.y > 0.5f) // Up
            {
                newIndex = (selectedIndex - 1 + heroCards.Count) % heroCards.Count;
            }
            else if (navigation.y < -0.5f) // Down
            {
                newIndex = (selectedIndex + 1) % heroCards.Count;
            }
            
            if (newIndex != selectedIndex)
            {
                SelectHero(newIndex);
            }
        }
        
        /// <summary>
        /// Handles submit input (Enter/Space/A button).
        /// </summary>
        private void HandleSubmit()
        {
            if (selectedHero != null && startButton.enabledSelf)
            {
                PlaySound(confirmSound);
                GameManager.Instance.SelectHero(selectedHero);
                GameManager.Instance.LoadGameplay();
            }
        }
        
        /// <summary>
        /// Handles cancel input (Escape/B button).
        /// </summary>
        private void HandleCancel()
        {
            PlaySound(cancelSound);
            GameManager.Instance.LoadMainMenu();
        }
        
        /// <summary>
        /// Handles mouse click input.
        /// </summary>
        private void HandleClick()
        {
            // Click handling is done through individual element callbacks
        }
        
        /// <summary>
        /// Selects a hero by index and updates the preview.
        /// </summary>
        private void SelectHero(int index)
        {
            if (index < 0 || index >= heroCards.Count) return;
            
            // Update selection state
            selectedIndex = index;
            selectedHero = heroCards[index].Hero;
            
            // Update visual selection
            UpdateHeroCardSelection();
            
            // Update preview panel
            UpdateHeroPreview();
            
            // Enable start button
            startButton.SetEnabled(true);
            
            // Play selection sound
            PlaySound(selectionSound);
            
            // Focus the selected card
            heroCards[index].Element.Focus();
        }
        
        /// <summary>
        /// Updates the visual selection state of hero cards.
        /// </summary>
        private void UpdateHeroCardSelection()
        {
            for (int i = 0; i < heroCards.Count; i++)
            {
                var card = heroCards[i];
                if (i == selectedIndex)
                {
                    card.Element.AddToClassList("selected");
                }
                else
                {
                    card.Element.RemoveFromClassList("selected");
                }
            }
        }
        
        /// <summary>
        /// Updates the hero preview panel with selected hero data.
        /// </summary>
        private void UpdateHeroPreview()
        {
            if (selectedHero == null) return;
            
            // Update portrait
            if (selectedHero.Portrait != null)
            {
                heroPortrait.style.backgroundImage = new StyleBackground(selectedHero.Portrait);
            }
            
            // Update text information
            heroName.text = selectedHero.HeroName;
            heroClass.text = $"Class: {selectedHero.HeroClass}";
            heroFaction.text = $"Faction: {selectedHero.Faction}";
            heroType.text = $"Type: {selectedHero.HeroType}";
            heroDescription.text = selectedHero.Description;
            
            // Update stats
            var stats = selectedHero.BaseStats;
            strengthValue.text = stats.strength.ToString();
            agilityValue.text = stats.agility.ToString();
            intelligenceValue.text = stats.intelligence.ToString();
            
            // Update abilities
            UpdateAbilitiesDisplay();
        }
        
        /// <summary>
        /// Updates the abilities display in the preview panel.
        /// </summary>
        private void UpdateAbilitiesDisplay()
        {
            abilitiesGrid.Clear();
            
            if (selectedHero.Abilities == null) return;
            
            foreach (var ability in selectedHero.Abilities)
            {
                var abilityIcon = new VisualElement();
                abilityIcon.AddToClassList("ability-icon");
                
                if (ability.icon != null)
                {
                    abilityIcon.style.backgroundImage = new StyleBackground(ability.icon);
                }
                
                // Add tooltip functionality
                abilityIcon.tooltip = $"{ability.abilityName}\n{ability.description}\nCooldown: {ability.cooldown}s\nMana: {ability.manaCost}";
                
                abilitiesGrid.Add(abilityIcon);
            }
        }
        
        /// <summary>
        /// Plays a sound effect if audio source is available.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Data structure for hero card UI elements.
        /// </summary>
        private class HeroCard
        {
            public VisualElement Element { get; set; }
            public HeroData Hero { get; set; }
            public int Index { get; set; }
        }
    }
}