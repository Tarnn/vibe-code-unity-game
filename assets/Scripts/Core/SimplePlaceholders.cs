using UnityEngine;

namespace FrostRealm.Core
{
    /// <summary>
    /// Simple placeholder classes to ensure build completion.
    /// These provide basic functionality for immediate play testing.
    /// </summary>
    
    public class SimpleResourceManager : MonoBehaviour
    {
        private static SimpleResourceManager instance;
        public static SimpleResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<SimpleResourceManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SimpleResourceManager");
                        instance = go.AddComponent<SimpleResourceManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        public void ResetResources()
        {
            Debug.Log("Resources reset");
        }
    }
    
    public class SimpleRTSCamera : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 100f;
        
        private void Update()
        {
            // Basic WASD camera movement
            Vector3 moveInput = Vector3.zero;
            
            if (Input.GetKey(KeyCode.W)) moveInput += transform.forward;
            if (Input.GetKey(KeyCode.S)) moveInput -= transform.forward;
            if (Input.GetKey(KeyCode.A)) moveInput -= transform.right;
            if (Input.GetKey(KeyCode.D)) moveInput += transform.right;
            
            transform.position += moveInput * moveSpeed * Time.deltaTime;
            
            // Mouse rotation
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                
                transform.Rotate(-mouseY * rotationSpeed * Time.deltaTime, mouseX * rotationSpeed * Time.deltaTime, 0);
            }
        }
    }
    
    public class SimpleSelection : MonoBehaviour
    {
        private void Update()
        {
            // Basic selection with left click
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log($"Selected: {hit.collider.name}");
                }
            }
        }
    }
    
    public class SimpleAudio : MonoBehaviour
    {
        private AudioSource audioSource;
        
        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 0.5f;
        }
        
        public void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}