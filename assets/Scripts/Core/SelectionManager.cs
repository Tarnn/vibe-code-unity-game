using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrostRealm.Core
{
    /// <summary>
    /// Manages unit and building selection for RTS gameplay.
    /// Supports single selection, multi-selection with drag box, and group selection.
    /// Implements Warcraft III-style selection mechanics.
    /// </summary>
    public class SelectionManager : MonoBehaviour
    {
        [Header("Selection Settings")]
        [SerializeField] private LayerMask selectableLayerMask = -1;
        [SerializeField] private bool enableMultiSelection = true;
        [SerializeField] private bool enableDragBoxSelection = true;
        [SerializeField] private int maxSelectionCount = 12; // Warcraft III limit
        
        [Header("Drag Box Settings")]
        [SerializeField] private float minDragDistance = 5f;
        [SerializeField] private Color dragBoxColor = new Color(0, 1, 0, 0.2f);
        [SerializeField] private Color dragBoxBorderColor = Color.green;
        [SerializeField] private float dragBoxBorderWidth = 2f;
        
        [Header("Selection Visual Settings")]
        [SerializeField] private bool showSelectionIndicators = true;
        [SerializeField] private Material selectionMaterial;
        [SerializeField] private Color selectionColor = Color.green;
        [SerializeField] private float selectionIndicatorHeight = 0.1f;
        
        [Header("Audio")]
        [SerializeField] private bool enableSelectionAudio = true;
        [SerializeField] private string selectionSfx = "UnitSelect";
        [SerializeField] private string multiSelectionSfx = "MultiSelect";
        
        // Selection state
        private List<ISelectable> selectedObjects = new List<ISelectable>();
        private Dictionary<ISelectable, GameObject> selectionIndicators = new Dictionary<ISelectable, GameObject>();
        
        // Drag box state
        private bool isDragBoxActive = false;
        private Vector3 dragBoxStartPosition;
        private Vector3 dragBoxEndPosition;
        private Camera cameraComponent;
        
        // Input state
        private bool isMousePressed = false;
        private Vector3 lastMousePosition;
        
        // Group selection (1-9 keys like Warcraft III)
        private Dictionary<int, List<ISelectable>> controlGroups = new Dictionary<int, List<ISelectable>>();
        
        // Events
        public System.Action<List<ISelectable>> OnSelectionChanged;
        public System.Action<ISelectable> OnObjectSelected;
        public System.Action<ISelectable> OnObjectDeselected;
        public System.Action<int, List<ISelectable>> OnControlGroupSet;
        public System.Action<int> OnControlGroupSelected;
        
        // Singleton pattern
        private static SelectionManager instance;
        public static SelectionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SelectionManager>();
                    if (instance == null)
                    {
                        var go = new GameObject("Selection Manager");
                        instance = go.AddComponent<SelectionManager>();
                    }
                }
                return instance;
            }
        }
        
        // Properties
        public List<ISelectable> SelectedObjects => selectedObjects.ToList();
        public int SelectionCount => selectedObjects.Count;
        public bool HasSelection => selectedObjects.Count > 0;
        public ISelectable PrimarySelection => selectedObjects.Count > 0 ? selectedObjects[0] : null;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                if (transform.parent == null)
                    DontDestroyOnLoad(gameObject);
                InitializeControlGroups();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            cameraComponent = Camera.main;
            if (cameraComponent == null)
            {
                cameraComponent = FindObjectOfType<Camera>();
            }
            
            if (cameraComponent == null)
            {
                Debug.LogError("SelectionManager requires a camera to function!");
                enabled = false;
            }
        }
        
        void Update()
        {
            HandleSelectionInput();
            HandleControlGroupInput();
            UpdateDragBox();
        }
        
        void OnGUI()
        {
            if (isDragBoxActive && enableDragBoxSelection)
            {
                DrawDragBox();
            }
        }
        
        /// <summary>
        /// Initializes control groups for 1-9 keys.
        /// </summary>
        private void InitializeControlGroups()
        {
            for (int i = 1; i <= 9; i++)
            {
                controlGroups[i] = new List<ISelectable>();
            }
        }
        
        /// <summary>
        /// Handles mouse input for selection.
        /// </summary>
        private void HandleSelectionInput()
        {
            // Mouse down - start potential selection
            if (Input.GetMouseButtonDown(0))
            {
                isMousePressed = true;
                lastMousePosition = Input.mousePosition;
                dragBoxStartPosition = Input.mousePosition;
                
                // Check for immediate single selection
                if (!enableDragBoxSelection || !enableMultiSelection)
                {
                    PerformSingleSelection();
                }
            }
            
            // Mouse up - complete selection
            if (Input.GetMouseButtonUp(0) && isMousePressed)
            {
                isMousePressed = false;
                
                if (isDragBoxActive)
                {
                    CompleteDragBoxSelection();
                }
                else if (enableDragBoxSelection)
                {
                    // Single click selection
                    PerformSingleSelection();
                }
                
                isDragBoxActive = false;
            }
            
            // Mouse drag - update drag box
            if (isMousePressed && enableDragBoxSelection)
            {
                float dragDistance = Vector3.Distance(Input.mousePosition, dragBoxStartPosition);
                
                if (dragDistance > minDragDistance && !isDragBoxActive)
                {
                    isDragBoxActive = true;
                }
                
                if (isDragBoxActive)
                {
                    dragBoxEndPosition = Input.mousePosition;
                }
            }
        }
        
        /// <summary>
        /// Handles control group input (1-9 keys for setting/selecting groups).
        /// </summary>
        private void HandleControlGroupInput()
        {
            for (int i = 1; i <= 9; i++)
            {
                KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i);
                
                if (Input.GetKeyDown(keyCode))
                {
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        // Ctrl + Number: Set control group
                        SetControlGroup(i, selectedObjects);
                    }
                    else
                    {
                        // Number: Select control group
                        SelectControlGroup(i);
                    }
                }
            }
        }
        
        /// <summary>
        /// Performs single object selection at mouse position.
        /// </summary>
        private void PerformSingleSelection()
        {
            Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayerMask))
            {
                ISelectable selectable = hit.collider.GetComponent<ISelectable>();
                
                if (selectable != null)
                {
                    bool addToSelection = enableMultiSelection && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    
                    if (addToSelection)
                    {
                        ToggleSelection(selectable);
                    }
                    else
                    {
                        SelectSingle(selectable);
                    }
                }
                else
                {
                    // Clicked on non-selectable object - clear selection if not holding shift
                    if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                    {
                        ClearSelection();
                    }
                }
            }
            else
            {
                // Clicked on empty space - clear selection if not holding shift
                if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                {
                    ClearSelection();
                }
            }
        }
        
        /// <summary>
        /// Completes drag box selection.
        /// </summary>
        private void CompleteDragBoxSelection()
        {
            List<ISelectable> objectsInBox = GetObjectsInDragBox();
            
            if (objectsInBox.Count > 0)
            {
                bool addToSelection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                
                if (addToSelection)
                {
                    foreach (var obj in objectsInBox)
                    {
                        AddToSelection(obj);
                    }
                }
                else
                {
                    SelectMultiple(objectsInBox);
                }
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                ClearSelection();
            }
        }
        
        /// <summary>
        /// Gets all selectable objects within the current drag box.
        /// </summary>
        private List<ISelectable> GetObjectsInDragBox()
        {
            List<ISelectable> objectsInBox = new List<ISelectable>();
            
            // Convert screen positions to viewport coordinates
            Vector2 min = new Vector2(
                Mathf.Min(dragBoxStartPosition.x, dragBoxEndPosition.x) / Screen.width,
                Mathf.Min(dragBoxStartPosition.y, dragBoxEndPosition.y) / Screen.height
            );
            
            Vector2 max = new Vector2(
                Mathf.Max(dragBoxStartPosition.x, dragBoxEndPosition.x) / Screen.width,
                Mathf.Max(dragBoxStartPosition.y, dragBoxEndPosition.y) / Screen.height
            );
            
            // Find all selectable objects in the scene
            ISelectable[] allSelectables = FindObjectsOfType<MonoBehaviour>().OfType<ISelectable>().ToArray();
            
            foreach (var selectable in allSelectables)
            {
                if (selectable.IsSelectable)
                {
                    Vector3 screenPos = cameraComponent.WorldToViewportPoint(selectable.Position);
                    
                    if (screenPos.x >= min.x && screenPos.x <= max.x &&
                        screenPos.y >= min.y && screenPos.y <= max.y &&
                        screenPos.z > 0) // In front of camera
                    {
                        objectsInBox.Add(selectable);
                    }
                }
            }
            
            return objectsInBox;
        }
        
        /// <summary>
        /// Updates the drag box visualization.
        /// </summary>
        private void UpdateDragBox()
        {
            // Drag box updating is handled in OnGUI for rendering
        }
        
        /// <summary>
        /// Draws the drag box on screen.
        /// </summary>
        private void DrawDragBox()
        {
            Vector2 start = new Vector2(dragBoxStartPosition.x, Screen.height - dragBoxStartPosition.y);
            Vector2 end = new Vector2(dragBoxEndPosition.x, Screen.height - dragBoxEndPosition.y);
            
            Rect dragRect = new Rect(
                Mathf.Min(start.x, end.x),
                Mathf.Min(start.y, end.y),
                Mathf.Abs(end.x - start.x),
                Mathf.Abs(end.y - start.y)
            );
            
            // Draw filled rectangle
            GUI.color = dragBoxColor;
            GUI.DrawTexture(dragRect, Texture2D.whiteTexture);
            
            // Draw border
            GUI.color = dragBoxBorderColor;
            DrawRectBorder(dragRect, dragBoxBorderWidth);
            
            GUI.color = Color.white; // Reset color
        }
        
        /// <summary>
        /// Draws a rectangle border.
        /// </summary>
        private void DrawRectBorder(Rect rect, float borderWidth)
        {
            // Top
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, borderWidth), Texture2D.whiteTexture);
            // Bottom
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - borderWidth, rect.width, borderWidth), Texture2D.whiteTexture);
            // Left
            GUI.DrawTexture(new Rect(rect.x, rect.y, borderWidth, rect.height), Texture2D.whiteTexture);
            // Right
            GUI.DrawTexture(new Rect(rect.x + rect.width - borderWidth, rect.y, borderWidth, rect.height), Texture2D.whiteTexture);
        }
        
        /// <summary>
        /// Selects a single object, clearing previous selection.
        /// </summary>
        public void SelectSingle(ISelectable selectable)
        {
            if (selectable == null || !selectable.IsSelectable) return;
            
            ClearSelection();
            AddToSelection(selectable);
            
            PlaySelectionAudio(false);
        }
        
        /// <summary>
        /// Selects multiple objects, clearing previous selection.
        /// </summary>
        public void SelectMultiple(List<ISelectable> selectables)
        {
            ClearSelection();
            
            foreach (var selectable in selectables.Take(maxSelectionCount))
            {
                if (selectable != null && selectable.IsSelectable)
                {
                    AddToSelection(selectable);
                }
            }
            
            if (selectedObjects.Count > 1)
            {
                PlaySelectionAudio(true);
            }
            else if (selectedObjects.Count == 1)
            {
                PlaySelectionAudio(false);
            }
        }
        
        /// <summary>
        /// Adds an object to the current selection.
        /// </summary>
        public void AddToSelection(ISelectable selectable)
        {
            if (selectable == null || !selectable.IsSelectable || selectedObjects.Contains(selectable))
                return;
                
            if (selectedObjects.Count >= maxSelectionCount)
                return;
            
            selectedObjects.Add(selectable);
            selectable.OnSelected();
            
            if (showSelectionIndicators)
            {
                CreateSelectionIndicator(selectable);
            }
            
            OnObjectSelected?.Invoke(selectable);
            OnSelectionChanged?.Invoke(selectedObjects);
        }
        
        /// <summary>
        /// Removes an object from the current selection.
        /// </summary>
        public void RemoveFromSelection(ISelectable selectable)
        {
            if (selectable == null || !selectedObjects.Contains(selectable))
                return;
            
            selectedObjects.Remove(selectable);
            selectable.OnDeselected();
            
            if (selectionIndicators.ContainsKey(selectable))
            {
                DestroySelectionIndicator(selectable);
            }
            
            OnObjectDeselected?.Invoke(selectable);
            OnSelectionChanged?.Invoke(selectedObjects);
        }
        
        /// <summary>
        /// Toggles selection state of an object.
        /// </summary>
        public void ToggleSelection(ISelectable selectable)
        {
            if (selectedObjects.Contains(selectable))
            {
                RemoveFromSelection(selectable);
            }
            else
            {
                AddToSelection(selectable);
            }
        }
        
        /// <summary>
        /// Clears all selected objects.
        /// </summary>
        public void ClearSelection()
        {
            var objectsToDeselect = selectedObjects.ToList();
            
            foreach (var selectable in objectsToDeselect)
            {
                selectable.OnDeselected();
                OnObjectDeselected?.Invoke(selectable);
                
                if (selectionIndicators.ContainsKey(selectable))
                {
                    DestroySelectionIndicator(selectable);
                }
            }
            
            selectedObjects.Clear();
            OnSelectionChanged?.Invoke(selectedObjects);
        }
        
        /// <summary>
        /// Sets a control group to the current selection.
        /// </summary>
        public void SetControlGroup(int groupNumber, List<ISelectable> objects)
        {
            if (groupNumber < 1 || groupNumber > 9) return;
            
            controlGroups[groupNumber] = objects.ToList();
            OnControlGroupSet?.Invoke(groupNumber, objects);
            
            Debug.Log($"Control group {groupNumber} set with {objects.Count} objects");
        }
        
        /// <summary>
        /// Selects a control group.
        /// </summary>
        public void SelectControlGroup(int groupNumber)
        {
            if (groupNumber < 1 || groupNumber > 9) return;
            
            var group = controlGroups[groupNumber];
            
            // Remove null or invalid objects from the group
            group.RemoveAll(obj => obj == null || !obj.IsSelectable);
            
            if (group.Count > 0)
            {
                SelectMultiple(group);
                OnControlGroupSelected?.Invoke(groupNumber);
            }
        }
        
        /// <summary>
        /// Creates a visual indicator for a selected object.
        /// </summary>
        private void CreateSelectionIndicator(ISelectable selectable)
        {
            if (selectionIndicators.ContainsKey(selectable))
                return;
            
            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            indicator.name = "Selection Indicator";
            
            // Remove collider to avoid interference
            Destroy(indicator.GetComponent<Collider>());
            
            // Position and scale the indicator
            indicator.transform.position = selectable.Position + Vector3.up * selectionIndicatorHeight;
            indicator.transform.localScale = new Vector3(selectable.SelectionSize, 0.1f, selectable.SelectionSize);
            
            // Apply selection material/color
            Renderer renderer = indicator.GetComponent<Renderer>();
            if (selectionMaterial != null)
            {
                renderer.material = selectionMaterial;
            }
            else
            {
                renderer.material.color = selectionColor;
            }
            
            selectionIndicators[selectable] = indicator;
        }
        
        /// <summary>
        /// Destroys the selection indicator for an object.
        /// </summary>
        private void DestroySelectionIndicator(ISelectable selectable)
        {
            if (selectionIndicators.TryGetValue(selectable, out GameObject indicator))
            {
                Destroy(indicator);
                selectionIndicators.Remove(selectable);
            }
        }
        
        /// <summary>
        /// Plays appropriate selection audio.
        /// </summary>
        private void PlaySelectionAudio(bool isMultiSelection)
        {
            if (!enableSelectionAudio || AudioManager.Instance == null)
                return;
            
            string audioClip = isMultiSelection ? multiSelectionSfx : selectionSfx;
            AudioManager.Instance.PlayUiSfx(audioClip);
        }
        
        /// <summary>
        /// Gets all selected objects of a specific type.
        /// </summary>
        public List<T> GetSelectedObjectsOfType<T>() where T : class, ISelectable
        {
            return selectedObjects.OfType<T>().ToList();
        }
        
        /// <summary>
        /// Checks if a specific object is selected.
        /// </summary>
        public bool IsSelected(ISelectable selectable)
        {
            return selectedObjects.Contains(selectable);
        }
        
        void OnDestroy()
        {
            // Clean up selection indicators
            foreach (var indicator in selectionIndicators.Values)
            {
                if (indicator != null)
                    Destroy(indicator);
            }
            selectionIndicators.Clear();
        }
    }
    
    /// <summary>
    /// Interface for objects that can be selected in the RTS.
    /// </summary>
    public interface ISelectable
    {
        Vector3 Position { get; }
        float SelectionSize { get; }
        bool IsSelectable { get; }
        
        void OnSelected();
        void OnDeselected();
    }
}