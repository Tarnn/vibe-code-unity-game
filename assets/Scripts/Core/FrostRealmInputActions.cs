using UnityEngine;
using UnityEngine.InputSystem;

namespace FrostRealm.Core
{
    /// <summary>
    /// Simple wrapper for FrostRealm input actions.
    /// Handles all input for the game without complex interfaces.
    /// </summary>
    public class FrostRealmInputActions : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        
        // Input action maps
        private InputActionMap uiActionMap;
        private InputActionMap playerActionMap;
        private InputActionMap cameraActionMap;
        
        // Common actions
        public InputAction NavigateAction { get; private set; }
        public InputAction SubmitAction { get; private set; }
        public InputAction CancelAction { get; private set; }
        public InputAction MoveAction { get; private set; }
        public InputAction LookAction { get; private set; }
        public InputAction SelectAction { get; private set; }
        public InputAction SecondarySelectAction { get; private set; }
        
        private void Awake()
        {
            // Load the input actions asset if not assigned
            if (inputActionAsset == null)
            {
                inputActionAsset = Resources.Load<InputActionAsset>("FrostRealmInputActions");
            }
            
            // If still null, create basic actions programmatically
            if (inputActionAsset == null)
            {
                CreateBasicInputActions();
            }
            else
            {
                LoadActionsFromAsset();
            }
        }
        
        private void CreateBasicInputActions()
        {
            // Create basic input actions programmatically
            var asset = ScriptableObject.CreateInstance<InputActionAsset>();
            asset.name = "FrostRealmInputActions";
            
            // UI Action Map
            uiActionMap = new InputActionMap("UI");
            NavigateAction = uiActionMap.AddAction("Navigate", InputActionType.PassThrough);
            NavigateAction.AddBinding("<Gamepad>/leftStick").WithProcessor("stickDeadzone");
            NavigateAction.AddBinding("<Gamepad>/dpad");
            NavigateAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            
            SubmitAction = uiActionMap.AddAction("Submit", InputActionType.Button);
            SubmitAction.AddBinding("<Keyboard>/enter");
            SubmitAction.AddBinding("<Keyboard>/space");
            SubmitAction.AddBinding("<Gamepad>/buttonSouth");
            
            CancelAction = uiActionMap.AddAction("Cancel", InputActionType.Button);
            CancelAction.AddBinding("<Keyboard>/escape");
            CancelAction.AddBinding("<Gamepad>/buttonEast");
            
            // Player Action Map
            playerActionMap = new InputActionMap("Player");
            MoveAction = playerActionMap.AddAction("Move", InputActionType.PassThrough);
            MoveAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            MoveAction.AddBinding("<Gamepad>/leftStick").WithProcessor("stickDeadzone");
            
            LookAction = playerActionMap.AddAction("Look", InputActionType.PassThrough);
            LookAction.AddBinding("<Mouse>/delta");
            LookAction.AddBinding("<Gamepad>/rightStick").WithProcessor("stickDeadzone");
            
            SelectAction = playerActionMap.AddAction("Select", InputActionType.Button);
            SelectAction.AddBinding("<Mouse>/leftButton");
            SelectAction.AddBinding("<Gamepad>/buttonSouth");
            
            SecondarySelectAction = playerActionMap.AddAction("SecondarySelect", InputActionType.Button);
            SecondarySelectAction.AddBinding("<Mouse>/rightButton");
            SecondarySelectAction.AddBinding("<Gamepad>/buttonWest");
            
            // Add action maps to asset
            asset.AddActionMap(uiActionMap);
            asset.AddActionMap(playerActionMap);
            
            inputActionAsset = asset;
        }
        
        private void LoadActionsFromAsset()
        {
            // Load actions from the asset
            uiActionMap = inputActionAsset.FindActionMap("UI");
            playerActionMap = inputActionAsset.FindActionMap("Player");
            cameraActionMap = inputActionAsset.FindActionMap("Camera");
            
            // Get individual actions
            NavigateAction = uiActionMap?.FindAction("Navigate");
            SubmitAction = uiActionMap?.FindAction("Submit");
            CancelAction = uiActionMap?.FindAction("Cancel");
            
            MoveAction = playerActionMap?.FindAction("Move");
            LookAction = playerActionMap?.FindAction("Look");
            SelectAction = playerActionMap?.FindAction("Select");
            SecondarySelectAction = playerActionMap?.FindAction("SecondarySelect");
        }
        
        private void OnEnable()
        {
            EnableAllActions();
        }
        
        private void OnDisable()
        {
            DisableAllActions();
        }
        
        public void EnableAllActions()
        {
            uiActionMap?.Enable();
            playerActionMap?.Enable();
            cameraActionMap?.Enable();
        }
        
        public void DisableAllActions()
        {
            uiActionMap?.Disable();
            playerActionMap?.Disable();
            cameraActionMap?.Disable();
        }
        
        public void EnableUI()
        {
            uiActionMap?.Enable();
            playerActionMap?.Disable();
            cameraActionMap?.Disable();
        }
        
        public void EnableGameplay()
        {
            uiActionMap?.Disable();
            playerActionMap?.Enable();
            cameraActionMap?.Enable();
        }
        
        private void OnDestroy()
        {
            DisableAllActions();
        }
        
        public void Dispose()
        {
            DisableAllActions();
            // Clean up any resources if needed
        }
    }
}