using UnityEngine;
using Cinemachine;

namespace FrostRealm.Core
{
    /// <summary>
    /// RTS-style camera controller for FrostRealm Chronicles.
    /// Supports WASD movement, mouse edge scrolling, zoom, and rotation.
    /// Optimized for Warcraft III: The Frozen Throne style isometric gameplay.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class RTSCameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 20f;
        [SerializeField] private float fastMoveSpeed = 40f;
        [SerializeField] private float edgeScrollSpeed = 15f;
        [SerializeField] private bool enableEdgeScrolling = true;
        [SerializeField] private float edgeScrollMargin = 50f;
        
        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 25f;
        [SerializeField] private bool invertZoom = false;
        
        [Header("Rotation Settings")]
        [SerializeField] private bool enableRotation = true;
        [SerializeField] private float rotationSpeed = 100f;
        [SerializeField] private KeyCode rotationKey = KeyCode.Q;
        [SerializeField] private KeyCode reverseRotationKey = KeyCode.E;
        
        [Header("Boundary Settings")]
        [SerializeField] private bool enableBoundaries = true;
        [SerializeField] private Vector2 minBounds = new Vector2(-100, -100);
        [SerializeField] private Vector2 maxBounds = new Vector2(100, 100);
        
        [Header("Input Settings")]
        [SerializeField] private KeyCode fastMoveKey = KeyCode.LeftShift;
        [SerializeField] private bool useMouseForMovement = true;
        [SerializeField] private int middleMouseButton = 2;
        
        [Header("Smoothing")]
        [SerializeField] private bool enableSmoothing = true;
        [SerializeField] private float movementSmoothing = 5f;
        [SerializeField] private float zoomSmoothing = 5f;
        
        [Header("Performance")]
        [SerializeField] private bool enableFrustumCulling = true;
        [SerializeField] private LayerMask cullingLayers = -1;
        
        // Internal state
        private Camera cameraComponent;
        private Vector3 targetPosition;
        private float targetZoom;
        private Vector3 lastMousePosition;
        private bool isDragging = false;
        
        // Input tracking
        private Vector2 keyboardInput;
        private Vector2 mouseEdgeInput;
        private float zoomInput;
        private float rotationInput;
        
        // Movement bounds
        private Vector3 velocity = Vector3.zero;
        
        // Properties
        public Camera Camera => cameraComponent;
        public Vector3 Position => transform.position;
        public float CurrentZoom => cameraComponent.orthographicSize;
        public bool IsWithinBounds => IsPositionWithinBounds(transform.position);
        
        // Events
        public System.Action<Vector3> OnCameraPositionChanged;
        public System.Action<float> OnCameraZoomChanged;
        public System.Action<bool> OnBoundaryHit;
        
        void Awake()
        {
            cameraComponent = GetComponent<Camera>();
            if (cameraComponent == null)
            {
                Debug.LogError("RTSCameraController requires a Camera component!");
                enabled = false;
                return;
            }
            
            // Initialize target values
            targetPosition = transform.position;
            targetZoom = cameraComponent.orthographicSize;
            
            // Clamp initial zoom
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            cameraComponent.orthographicSize = targetZoom;
        }
        
        void Start()
        {
            // Ensure camera is set to orthographic for RTS view
            if (!cameraComponent.orthographic)
            {
                cameraComponent.orthographic = true;
                Debug.Log("Camera set to orthographic mode for RTS gameplay");
            }
            
            // Apply initial boundaries
            if (enableBoundaries)
            {
                ClampPositionToBounds();
            }
        }
        
        void Update()
        {
            HandleInput();
            UpdateCameraMovement();
            UpdateCameraZoom();
            
            if (enableRotation)
            {
                UpdateCameraRotation();
            }
        }
        
        void LateUpdate()
        {
            // Apply frustum culling optimizations
            if (enableFrustumCulling)
            {
                UpdateFrustumCulling();
            }
        }
        
        /// <summary>
        /// Handles all input for camera control.
        /// </summary>
        private void HandleInput()
        {
            // Keyboard movement (WASD)
            keyboardInput.x = Input.GetAxis("Horizontal");
            keyboardInput.y = Input.GetAxis("Vertical");
            
            // Mouse edge scrolling
            if (enableEdgeScrolling)
            {
                HandleEdgeScrolling();
            }
            
            // Mouse drag movement
            if (useMouseForMovement)
            {
                HandleMouseDragMovement();
            }
            
            // Zoom input
            zoomInput = Input.GetAxis("Mouse ScrollWheel");
            if (invertZoom)
                zoomInput = -zoomInput;
            
            // Rotation input
            if (enableRotation)
            {
                rotationInput = 0f;
                if (Input.GetKey(rotationKey))
                    rotationInput = -1f;
                if (Input.GetKey(reverseRotationKey))
                    rotationInput = 1f;
            }
        }
        
        /// <summary>
        /// Handles mouse edge scrolling.
        /// </summary>
        private void HandleEdgeScrolling()
        {
            mouseEdgeInput = Vector2.zero;
            Vector3 mousePosition = Input.mousePosition;
            
            // Check screen edges
            if (mousePosition.x <= edgeScrollMargin)
                mouseEdgeInput.x = -1f;
            else if (mousePosition.x >= Screen.width - edgeScrollMargin)
                mouseEdgeInput.x = 1f;
                
            if (mousePosition.y <= edgeScrollMargin)
                mouseEdgeInput.y = -1f;
            else if (mousePosition.y >= Screen.height - edgeScrollMargin)
                mouseEdgeInput.y = 1f;
        }
        
        /// <summary>
        /// Handles mouse drag movement.
        /// </summary>
        private void HandleMouseDragMovement()
        {
            if (Input.GetMouseButtonDown(middleMouseButton))
            {
                lastMousePosition = Input.mousePosition;
                isDragging = true;
            }
            else if (Input.GetMouseButtonUp(middleMouseButton))
            {
                isDragging = false;
            }
            
            if (isDragging && Input.GetMouseButton(middleMouseButton))
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
                Vector3 worldDelta = cameraComponent.ScreenToWorldPoint(new Vector3(mouseDelta.x, mouseDelta.y, cameraComponent.nearClipPlane));
                worldDelta = transform.InverseTransformDirection(worldDelta);
                
                targetPosition -= new Vector3(worldDelta.x, worldDelta.y, 0) * 0.1f;
                lastMousePosition = Input.mousePosition;
            }
        }
        
        /// <summary>
        /// Updates camera movement based on input.
        /// </summary>
        private void UpdateCameraMovement()
        {
            Vector3 moveDirection = Vector3.zero;
            
            // Combine keyboard and edge scroll input
            Vector2 totalInput = keyboardInput + mouseEdgeInput;
            
            // Convert 2D input to 3D movement
            moveDirection = new Vector3(totalInput.x, 0, totalInput.y);
            
            // Apply current movement speed
            float currentMoveSpeed = Input.GetKey(fastMoveKey) ? fastMoveSpeed : moveSpeed;
            if (mouseEdgeInput.magnitude > 0)
                currentMoveSpeed = edgeScrollSpeed;
            
            // Scale movement by zoom level (closer zoom = slower movement for precision)
            float zoomSpeedMultiplier = Mathf.Lerp(0.5f, 2f, (targetZoom - minZoom) / (maxZoom - minZoom));
            currentMoveSpeed *= zoomSpeedMultiplier;
            
            // Apply movement to target position
            targetPosition += transform.TransformDirection(moveDirection) * currentMoveSpeed * Time.deltaTime;
            
            // Apply boundaries
            if (enableBoundaries)
            {
                ClampTargetPositionToBounds();
            }
            
            // Smooth movement or instant
            if (enableSmoothing)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / movementSmoothing);
            }
            else
            {
                transform.position = targetPosition;
            }
            
            // Notify position change
            OnCameraPositionChanged?.Invoke(transform.position);
        }
        
        /// <summary>
        /// Updates camera zoom based on input.
        /// </summary>
        private void UpdateCameraZoom()
        {
            if (Mathf.Abs(zoomInput) > 0.01f)
            {
                targetZoom -= zoomInput * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
            
            // Smooth zoom or instant
            if (enableSmoothing)
            {
                cameraComponent.orthographicSize = Mathf.Lerp(cameraComponent.orthographicSize, targetZoom, zoomSmoothing * Time.deltaTime);
            }
            else
            {
                cameraComponent.orthographicSize = targetZoom;
            }
            
            // Notify zoom change
            if (!Mathf.Approximately(cameraComponent.orthographicSize, targetZoom))
            {
                OnCameraZoomChanged?.Invoke(cameraComponent.orthographicSize);
            }
        }
        
        /// <summary>
        /// Updates camera rotation based on input.
        /// </summary>
        private void UpdateCameraRotation()
        {
            if (Mathf.Abs(rotationInput) > 0.01f)
            {
                float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotationAmount, 0, Space.World);
            }
        }
        
        /// <summary>
        /// Clamps the current position to boundaries.
        /// </summary>
        private void ClampPositionToBounds()
        {
            Vector3 pos = transform.position;
            bool hitBoundary = false;
            
            if (pos.x < minBounds.x)
            {
                pos.x = minBounds.x;
                hitBoundary = true;
            }
            else if (pos.x > maxBounds.x)
            {
                pos.x = maxBounds.x;
                hitBoundary = true;
            }
            
            if (pos.z < minBounds.y)
            {
                pos.z = minBounds.y;
                hitBoundary = true;
            }
            else if (pos.z > maxBounds.y)
            {
                pos.z = maxBounds.y;
                hitBoundary = true;
            }
            
            transform.position = pos;
            targetPosition = pos;
            
            if (hitBoundary)
            {
                OnBoundaryHit?.Invoke(true);
            }
        }
        
        /// <summary>
        /// Clamps the target position to boundaries.
        /// </summary>
        private void ClampTargetPositionToBounds()
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.y, maxBounds.y);
        }
        
        /// <summary>
        /// Checks if a position is within the defined boundaries.
        /// </summary>
        private bool IsPositionWithinBounds(Vector3 position)
        {
            return position.x >= minBounds.x && position.x <= maxBounds.x &&
                   position.z >= minBounds.y && position.z <= maxBounds.y;
        }
        
        /// <summary>
        /// Updates frustum culling for performance optimization.
        /// </summary>
        private void UpdateFrustumCulling()
        {
            // This would be expanded to work with ECS/DOTS for optimal performance
            // For now, it's a placeholder for future optimization
        }
        
        /// <summary>
        /// Sets the camera position instantly without smoothing.
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            targetPosition = position;
            
            if (enableBoundaries)
            {
                ClampPositionToBounds();
            }
            
            OnCameraPositionChanged?.Invoke(transform.position);
        }
        
        /// <summary>
        /// Sets the camera zoom instantly without smoothing.
        /// </summary>
        public void SetZoom(float zoom)
        {
            targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            cameraComponent.orthographicSize = targetZoom;
            OnCameraZoomChanged?.Invoke(targetZoom);
        }
        
        /// <summary>
        /// Focuses the camera on a specific world position.
        /// </summary>
        public void FocusOnPosition(Vector3 worldPosition, float? zoom = null)
        {
            targetPosition = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
            
            if (zoom.HasValue)
            {
                targetZoom = Mathf.Clamp(zoom.Value, minZoom, maxZoom);
            }
            
            if (enableBoundaries)
            {
                ClampTargetPositionToBounds();
            }
        }
        
        /// <summary>
        /// Gets the current camera bounds in world space.
        /// </summary>
        public Bounds GetCameraBounds()
        {
            float height = cameraComponent.orthographicSize * 2f;
            float width = height * cameraComponent.aspect;
            
            return new Bounds(transform.position, new Vector3(width, 0, height));
        }
        
        /// <summary>
        /// Converts screen position to world position on the ground plane (Y=0).
        /// </summary>
        public Vector3 ScreenToWorldPosition(Vector3 screenPosition)
        {
            screenPosition.z = cameraComponent.nearClipPlane;
            Vector3 worldPosition = cameraComponent.ScreenToWorldPoint(screenPosition);
            
            // Project onto ground plane (Y=0)
            if (transform.eulerAngles.x != 0)
            {
                float distanceToGround = transform.position.y / Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad);
                Vector3 direction = (worldPosition - transform.position).normalized;
                worldPosition = transform.position + direction * distanceToGround;
            }
            
            worldPosition.y = 0;
            return worldPosition;
        }
        
        /// <summary>
        /// Enables or disables camera boundaries.
        /// </summary>
        public void SetBoundariesEnabled(bool enabled)
        {
            enableBoundaries = enabled;
            if (enabled)
            {
                ClampPositionToBounds();
            }
        }
        
        /// <summary>
        /// Sets new boundary limits.
        /// </summary>
        public void SetBoundaries(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            
            if (enableBoundaries)
            {
                ClampPositionToBounds();
            }
        }
        
        void OnDrawGizmosSelected()
        {
            if (enableBoundaries)
            {
                // Draw boundary rectangle
                Gizmos.color = Color.yellow;
                Vector3 center = new Vector3((minBounds.x + maxBounds.x) * 0.5f, 0, (minBounds.y + maxBounds.y) * 0.5f);
                Vector3 size = new Vector3(maxBounds.x - minBounds.x, 0.1f, maxBounds.y - minBounds.y);
                Gizmos.DrawWireCube(center, size);
            }
            
            // Draw camera bounds
            if (cameraComponent != null)
            {
                Gizmos.color = Color.green;
                Bounds bounds = GetCameraBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }
}