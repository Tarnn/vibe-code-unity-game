using UnityEngine;
using UnityEngine.InputSystem;

namespace FrostRealm.Core
{
    /// <summary>
    /// Manages input handling for the game using Unity's Input System.
    /// Provides centralized input management for UI navigation and game controls.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private bool enableGamepadSupport = true;
        [SerializeField] private float gamepadSensitivity = 1f;
        
        // Input Actions
        private FrostRealmInputActions inputActions;
        
        // Events for UI navigation
        public System.Action<Vector2> OnNavigate;
        public System.Action OnSubmit;
        public System.Action OnCancel;
        public System.Action OnPause;
        
        // Events for menu interactions
        public System.Action<Vector2> OnPoint;
        public System.Action OnClick;
        public System.Action OnRightClick;
        
        // Singleton instance
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<InputManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("InputManager");
                        instance = go.AddComponent<InputManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        void Awake()
        {
            // Ensure singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInput();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void OnEnable()
        {
            if (inputActions != null)
            {
                inputActions.EnableAllActions();
            }
        }
        
        void OnDisable()
        {
            if (inputActions != null)
            {
                inputActions.DisableAllActions();
            }
        }
        
        void OnDestroy()
        {
            if (inputActions != null)
            {
                inputActions.Dispose();
            }
        }
        
        /// <summary>
        /// Initializes the input system and binds actions.
        /// </summary>
        private void InitializeInput()
        {
            inputActions = new FrostRealmInputActions();
            
            // Bind UI navigation actions
            if (inputActions.NavigateAction != null)
                inputActions.NavigateAction.performed += OnNavigatePerformed;
            if (inputActions.SubmitAction != null)
                inputActions.SubmitAction.performed += OnSubmitPerformed;
            if (inputActions.CancelAction != null)
                inputActions.CancelAction.performed += OnCancelPerformed;
            if (inputActions.SelectAction != null)
                inputActions.SelectAction.performed += OnClickPerformed;
            if (inputActions.SecondarySelectAction != null)
                inputActions.SecondarySelectAction.performed += OnRightClickPerformed;
            
            inputActions.EnableAllActions();
            
            Debug.Log("Input system initialized successfully!");
        }
        
        #region Input Action Callbacks
        
        private void OnNavigatePerformed(InputAction.CallbackContext context)
        {
            Vector2 navigation = context.ReadValue<Vector2>();
            
            // Apply gamepad sensitivity
            if (context.control.device is Gamepad)
            {
                navigation *= gamepadSensitivity;
            }
            
            OnNavigate?.Invoke(navigation);
        }
        
        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            OnSubmit?.Invoke();
        }
        
        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            OnCancel?.Invoke();
        }
        
        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            Vector2 position = context.ReadValue<Vector2>();
            OnPoint?.Invoke(position);
        }
        
        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            OnClick?.Invoke();
        }
        
        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            OnRightClick?.Invoke();
        }
        
        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            OnPause?.Invoke();
        }
        
        #endregion
        
        /// <summary>
        /// Gets the current mouse position in screen space.
        /// </summary>
        public Vector2 GetMousePosition()
        {
            return Mouse.current?.position.ReadValue() ?? Vector2.zero;
        }
        
        /// <summary>
        /// Gets the current gamepad navigation input.
        /// </summary>
        public Vector2 GetGamepadNavigation()
        {
            if (!enableGamepadSupport || Gamepad.current == null)
                return Vector2.zero;
                
            return Gamepad.current.leftStick.ReadValue() * gamepadSensitivity;
        }
        
        /// <summary>
        /// Checks if any key is currently pressed.
        /// </summary>
        public bool IsAnyKeyPressed()
        {
            return Keyboard.current?.anyKey.isPressed == true;
        }
        
        /// <summary>
        /// Enables or disables input processing.
        /// </summary>
        /// <param name="enabled">True to enable input, false to disable</param>
        public void SetInputEnabled(bool enabled)
        {
            if (enabled)
            {
                inputActions?.EnableAllActions();
            }
            else
            {
                inputActions?.DisableAllActions();
            }
        }
        
        /// <summary>
        /// Switches to a specific input action map.
        /// </summary>
        /// <param name="actionMapName">The name of the action map to activate</param>
        public void SwitchActionMap(string actionMapName)
        {
            if (actionMapName == "UI")
            {
                inputActions?.EnableUI();
            }
            else if (actionMapName == "Player" || actionMapName == "Gameplay")
            {
                inputActions?.EnableGameplay();
            }
        }
        
        /// <summary>
        /// Sets the gamepad sensitivity for navigation.
        /// </summary>
        /// <param name="sensitivity">The sensitivity value (0.1 to 3.0)</param>
        public void SetGamepadSensitivity(float sensitivity)
        {
            gamepadSensitivity = Mathf.Clamp(sensitivity, 0.1f, 3f);
        }
    }
}