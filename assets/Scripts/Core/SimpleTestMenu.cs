using UnityEngine;
using UnityEngine.UI;

namespace FrostRealm.Core
{
    /// <summary>
    /// Ultra-simple test menu that should definitely work.
    /// Creates basic UI elements using the old UI system.
    /// </summary>
    [DefaultExecutionOrder(-3000)]
    public class SimpleTestMenu : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log("SimpleTestMenu: Starting immediate UI creation");
            CreateSimpleUI();
        }
        
        void CreateSimpleUI()
        {
            Debug.Log("SimpleTestMenu: Creating simple test UI...");
            Debug.Log($"Screen resolution: {Screen.width}x{Screen.height}");
            Debug.Log($"Current scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            
            // Create Canvas
            GameObject canvasGO = new GameObject("Test Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create background
            GameObject bgGO = new GameObject("Background");
            bgGO.transform.SetParent(canvasGO.transform, false);
            
            RectTransform bgRect = bgGO.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            Image bgImage = bgGO.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.3f, 0.5f, 1f); // Blue background
            
            // Create title text
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(canvasGO.transform, false);
            
            RectTransform titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.7f);
            titleRect.anchorMax = new Vector2(0.5f, 0.7f);
            titleRect.sizeDelta = new Vector2(800, 100);
            titleRect.anchoredPosition = Vector2.zero;
            
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = "FROSTREALM CHRONICLES";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 48;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            
            // Create start button
            GameObject buttonGO = new GameObject("Start Button");
            buttonGO.transform.SetParent(canvasGO.transform, false);
            
            RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.4f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.4f);
            buttonRect.sizeDelta = new Vector2(300, 60);
            buttonRect.anchoredPosition = Vector2.zero;
            
            Image buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.5f, 0.8f, 1f);
            
            Button button = buttonGO.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(() => {
                Debug.Log("Start button clicked!");
                // Just log for now since we don't have proper scenes set up
                Debug.Log("Game would transition to character selection here");
            });
            
            // Button text
            GameObject buttonTextGO = new GameObject("Button Text");
            buttonTextGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform buttonTextRect = buttonTextGO.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            Text buttonText = buttonTextGO.AddComponent<Text>();
            buttonText.text = "START GAME";
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 24;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
            
            // Create EventSystem if needed
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            Debug.Log("SimpleTestMenu: Simple test UI created successfully!");
            Debug.Log($"Canvas created: {canvasGO != null}");
            Debug.Log($"Canvas active: {canvasGO.activeInHierarchy}");
            Debug.Log($"Canvas render mode: {canvas.renderMode}");
            Debug.Log($"Canvas sorting order: {canvas.sortingOrder}");
        }
    }
}