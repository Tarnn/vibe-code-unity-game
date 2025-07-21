using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace FrostRealm.UI
{
    /// <summary>
    /// Runtime main menu UI for FrostRealm Chronicles.
    /// Creates a complete Warcraft III-style menu interface at runtime.
    /// </summary>
    public class RuntimeMainMenu : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color primaryColor = new Color(0.1f, 0.2f, 0.4f);
        [SerializeField] private Color accentColor = new Color(0.3f, 0.5f, 0.8f);
        [SerializeField] private float animationSpeed = 0.5f;
        
        private Canvas mainCanvas;
        private GameObject menuContainer;
        private AudioSource audioSource;
        
        void Awake()
        {
            Debug.Log("RuntimeMainMenu: Awake called - Creating menu immediately");
            CreateCompleteMenu();
        }
        
        void Start()
        {
            Debug.Log("RuntimeMainMenu: Start called - Animating menu entry");
            AnimateMenuEntry();
        }
        
        private void CreateCompleteMenu()
        {
            Debug.Log("RuntimeMainMenu: Creating complete menu...");
            
            // Create Canvas
            GameObject canvasGO = new GameObject("Main Menu Canvas");
            mainCanvas = canvasGO.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create EventSystem
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventGO = new GameObject("EventSystem");
                eventGO.AddComponent<EventSystem>();
                eventGO.AddComponent<StandaloneInputModule>();
            }
            
            // Create audio source
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            
            // Build menu structure
            CreateBackground();
            CreateLogoAndTitle();
            CreateMenuButtons();
            CreateFooter();
            CreateSnowEffect();
        }
        
        private void CreateBackground()
        {
            // Main background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            Image bgImage = bg.AddComponent<Image>();
            
            // Create gradient texture
            Texture2D gradientTex = new Texture2D(2, 256);
            for (int y = 0; y < 256; y++)
            {
                float t = y / 255f;
                Color gradColor = Color.Lerp(
                    new Color(0.02f, 0.05f, 0.1f),
                    new Color(0.05f, 0.1f, 0.2f),
                    t
                );
                gradientTex.SetPixel(0, y, gradColor);
                gradientTex.SetPixel(1, y, gradColor);
            }
            gradientTex.Apply();
            
            bgImage.sprite = Sprite.Create(gradientTex, new Rect(0, 0, 2, 256), Vector2.one * 0.5f);
            bgImage.type = Image.Type.Sliced;
            
            // Ice overlay pattern
            GameObject iceOverlay = new GameObject("Ice Overlay");
            iceOverlay.transform.SetParent(bg.transform, false);
            
            RectTransform iceRect = iceOverlay.AddComponent<RectTransform>();
            iceRect.anchorMin = Vector2.zero;
            iceRect.anchorMax = Vector2.one;
            iceRect.offsetMin = Vector2.zero;
            iceRect.offsetMax = Vector2.zero;
            
            Image iceImage = iceOverlay.AddComponent<Image>();
            iceImage.color = new Color(0.5f, 0.7f, 1f, 0.05f);
        }
        
        private void CreateLogoAndTitle()
        {
            // Title container
            GameObject titleContainer = new GameObject("Title Container");
            titleContainer.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform titleRect = titleContainer.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.6f);
            titleRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleRect.sizeDelta = new Vector2(800, 300);
            titleRect.anchoredPosition = Vector2.zero;
            
            // Main title
            GameObject titleGO = new GameObject("Game Title");
            titleGO.transform.SetParent(titleContainer.transform, false);
            
            RectTransform textRect = titleGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "FROSTREALM\nCHRONICLES";
            titleText.fontSize = 86;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            
            // Gradient color
            titleText.enableVertexGradient = true;
            titleText.colorGradient = new VertexGradient(
                new Color(0.7f, 0.85f, 1f),
                new Color(0.9f, 0.95f, 1f),
                new Color(0.4f, 0.6f, 0.9f),
                new Color(0.5f, 0.7f, 0.95f)
            );
            
            // Outline
            titleText.outlineWidth = 0.4f;
            titleText.outlineColor = new Color(0, 0.1f, 0.3f);
            
            // Shadow
            Shadow shadow = titleGO.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0.2f, 0.8f);
            shadow.effectDistance = new Vector2(4, -4);
            
            // Subtitle
            GameObject subtitleGO = new GameObject("Subtitle");
            subtitleGO.transform.SetParent(titleContainer.transform, false);
            
            RectTransform subRect = subtitleGO.AddComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0.5f, 0);
            subRect.anchorMax = new Vector2(0.5f, 0);
            subRect.sizeDelta = new Vector2(600, 50);
            subRect.anchoredPosition = new Vector2(0, 20);
            
            TextMeshProUGUI subtitle = subtitleGO.AddComponent<TextMeshProUGUI>();
            subtitle.text = "A Warcraft III Inspired RTS Experience";
            subtitle.fontSize = 22;
            subtitle.alignment = TextAlignmentOptions.Center;
            subtitle.color = new Color(0.6f, 0.7f, 0.9f, 0.8f);
            subtitle.fontStyle = FontStyles.Italic;
        }
        
        private void CreateMenuButtons()
        {
            menuContainer = new GameObject("Menu Container");
            menuContainer.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform containerRect = menuContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.15f);
            containerRect.anchorMax = new Vector2(0.5f, 0.55f);
            containerRect.sizeDelta = new Vector2(400, 400);
            containerRect.anchoredPosition = Vector2.zero;
            
            VerticalLayoutGroup layout = menuContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.padding = new RectOffset(0, 0, 20, 20);
            
            // Create menu buttons
            CreateStyledButton("SINGLE PLAYER", OnSinglePlayerClick, true);
            CreateStyledButton("MULTIPLAYER", OnMultiplayerClick, false);
            CreateStyledButton("MAP EDITOR", OnMapEditorClick, false);
            CreateStyledButton("OPTIONS", OnOptionsClick, true);
            CreateStyledButton("CREDITS", OnCreditsClick, true);
            CreateStyledButton("EXIT GAME", OnExitClick, true);
        }
        
        private GameObject CreateStyledButton(string text, UnityEngine.Events.UnityAction action, bool enabled)
        {
            GameObject buttonGO = new GameObject($"Button_{text}");
            buttonGO.transform.SetParent(menuContainer.transform, false);
            
            RectTransform rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(350, 65);
            
            // Button background
            Image bgImage = buttonGO.AddComponent<Image>();
            bgImage.color = enabled ? primaryColor : new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Add gradient overlay
            GameObject gradientOverlay = new GameObject("Gradient");
            gradientOverlay.transform.SetParent(buttonGO.transform, false);
            
            RectTransform gradRect = gradientOverlay.AddComponent<RectTransform>();
            gradRect.anchorMin = Vector2.zero;
            gradRect.anchorMax = Vector2.one;
            gradRect.offsetMin = Vector2.zero;
            gradRect.offsetMax = Vector2.zero;
            
            Image gradImage = gradientOverlay.AddComponent<Image>();
            gradImage.color = new Color(1, 1, 1, 0.1f);
            gradImage.raycastTarget = false;
            
            // Button component
            Button button = buttonGO.AddComponent<Button>();
            button.targetGraphic = bgImage;
            button.interactable = enabled;
            
            if (enabled)
            {
                button.onClick.AddListener(action);
                button.onClick.AddListener(() => PlayClickSound());
            }
            
            // Button states
            ColorBlock colors = button.colors;
            colors.normalColor = primaryColor;
            colors.highlightedColor = Color.Lerp(primaryColor, accentColor, 0.5f);
            colors.pressedColor = accentColor;
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            button.colors = colors;
            
            // Border
            Outline outline = buttonGO.AddComponent<Outline>();
            outline.effectColor = accentColor;
            outline.effectDistance = new Vector2(2, 2);
            
            // Button text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20, 0);
            textRect.offsetMax = new Vector2(-20, 0);
            
            TextMeshProUGUI buttonText = textGO.AddComponent<TextMeshProUGUI>();
            buttonText.text = text;
            buttonText.fontSize = 26;
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = enabled ? Color.white : new Color(0.6f, 0.6f, 0.6f);
            
            if (!enabled)
            {
                buttonText.text += " (Coming Soon)";
                buttonText.fontSize = 22;
            }
            
            // Add hover animation
            AddButtonAnimation(buttonGO);
            
            return buttonGO;
        }
        
        private void AddButtonAnimation(GameObject button)
        {
            EventTrigger trigger = button.AddComponent<EventTrigger>();
            
            // On hover enter
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => {
                button.transform.localScale = Vector3.one * 1.05f;
                PlayHoverSound();
            });
            trigger.triggers.Add(enterEntry);
            
            // On hover exit
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => {
                button.transform.localScale = Vector3.one;
            });
            trigger.triggers.Add(exitEntry);
        }
        
        private void CreateFooter()
        {
            GameObject footer = new GameObject("Footer");
            footer.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform footerRect = footer.AddComponent<RectTransform>();
            footerRect.anchorMin = new Vector2(0, 0);
            footerRect.anchorMax = new Vector2(1, 0);
            footerRect.sizeDelta = new Vector2(0, 60);
            footerRect.anchoredPosition = Vector2.zero;
            
            // Version text
            GameObject versionGO = new GameObject("Version");
            versionGO.transform.SetParent(footer.transform, false);
            
            RectTransform versionRect = versionGO.AddComponent<RectTransform>();
            versionRect.anchorMin = new Vector2(0, 0.5f);
            versionRect.anchorMax = new Vector2(0, 0.5f);
            versionRect.sizeDelta = new Vector2(300, 30);
            versionRect.anchoredPosition = new Vector2(20, 0);
            versionRect.pivot = new Vector2(0, 0.5f);
            
            TextMeshProUGUI versionText = versionGO.AddComponent<TextMeshProUGUI>();
            versionText.text = "Version 1.0.0 - Unity 6.1 LTS";
            versionText.fontSize = 16;
            versionText.color = new Color(0.5f, 0.6f, 0.8f, 0.5f);
            
            // Copyright text
            GameObject copyrightGO = new GameObject("Copyright");
            copyrightGO.transform.SetParent(footer.transform, false);
            
            RectTransform copyRect = copyrightGO.AddComponent<RectTransform>();
            copyRect.anchorMin = new Vector2(1, 0.5f);
            copyRect.anchorMax = new Vector2(1, 0.5f);
            copyRect.sizeDelta = new Vector2(400, 30);
            copyRect.anchoredPosition = new Vector2(-20, 0);
            copyRect.pivot = new Vector2(1, 0.5f);
            
            TextMeshProUGUI copyText = copyrightGO.AddComponent<TextMeshProUGUI>();
            copyText.text = "© 2025 FrostRealm Studios";
            copyText.fontSize = 16;
            copyText.alignment = TextAlignmentOptions.Right;
            copyText.color = new Color(0.5f, 0.6f, 0.8f, 0.5f);
        }
        
        private void CreateSnowEffect()
        {
            GameObject snowContainer = new GameObject("Snow Effect");
            snowContainer.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform snowRect = snowContainer.AddComponent<RectTransform>();
            snowRect.anchorMin = Vector2.zero;
            snowRect.anchorMax = Vector2.one;
            snowRect.offsetMin = Vector2.zero;
            snowRect.offsetMax = Vector2.zero;
            
            // Create multiple snow particles
            for (int i = 0; i < 50; i++)
            {
                CreateSnowParticle(snowContainer.transform);
            }
        }
        
        private void CreateSnowParticle(Transform parent)
        {
            GameObject particle = new GameObject("Snow Particle");
            particle.transform.SetParent(parent, false);
            
            RectTransform rect = particle.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(Random.Range(0f, 1f), 1f);
            rect.anchorMax = rect.anchorMin;
            rect.sizeDelta = Vector2.one * Random.Range(4, 12);
            rect.anchoredPosition = new Vector2(0, Random.Range(0, 1080));
            
            Image img = particle.AddComponent<Image>();
            img.color = new Color(1, 1, 1, Random.Range(0.1f, 0.3f));
            
            // Add falling animation
            StartCoroutine(AnimateSnowParticle(rect));
        }
        
        private System.Collections.IEnumerator AnimateSnowParticle(RectTransform particle)
        {
            float fallSpeed = Random.Range(20f, 60f);
            float swayAmount = Random.Range(10f, 30f);
            float swaySpeed = Random.Range(0.5f, 2f);
            float startX = particle.anchoredPosition.x;
            
            while (particle != null)
            {
                Vector2 pos = particle.anchoredPosition;
                pos.y -= fallSpeed * Time.deltaTime;
                pos.x = startX + Mathf.Sin(Time.time * swaySpeed) * swayAmount;
                
                if (pos.y < -50)
                {
                    pos.y = 1130;
                    pos.x = Random.Range(-960, 960);
                    startX = pos.x;
                }
                
                particle.anchoredPosition = pos;
                yield return null;
            }
        }
        
        private void AnimateMenuEntry()
        {
            // Animate menu buttons sliding in
            if (menuContainer != null)
            {
                Vector2 startPos = menuContainer.GetComponent<RectTransform>().anchoredPosition;
                startPos.x = -600;
                menuContainer.GetComponent<RectTransform>().anchoredPosition = startPos;
                
                StartCoroutine(SlideIn(menuContainer.GetComponent<RectTransform>(), 0.5f));
            }
        }
        
        private System.Collections.IEnumerator SlideIn(RectTransform rect, float duration)
        {
            Vector2 startPos = rect.anchoredPosition;
            Vector2 endPos = new Vector2(0, startPos.y);
            
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
            
            rect.anchoredPosition = endPos;
        }
        
        #region Button Actions
        
        private void OnSinglePlayerClick()
        {
            Debug.Log("Loading Character Selection...");
            if (Core.SceneLoader.Instance != null)
            {
                Core.SceneLoader.Instance.LoadCharacterSelection();
            }
            else
            {
                SceneManager.LoadScene("CharacterSelection");
            }
        }
        
        private void OnMultiplayerClick()
        {
            Debug.Log("Multiplayer - Coming Soon!");
        }
        
        private void OnMapEditorClick()
        {
            Debug.Log("Map Editor - Coming Soon!");
        }
        
        private void OnOptionsClick()
        {
            Debug.Log("Opening Options...");
            CreateOptionsOverlay();
        }
        
        private void OnCreditsClick()
        {
            Debug.Log("Showing Credits...");
            CreateCreditsOverlay();
        }
        
        private void OnExitClick()
        {
            Debug.Log("Exiting Game...");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        #endregion
        
        #region Overlays
        
        private void CreateOptionsOverlay()
        {
            GameObject overlay = CreateOverlay("Options");
            
            GameObject content = overlay.transform.Find("Panel/Content").gameObject;
            
            // Add options content
            CreateOptionSlider(content.transform, "Master Volume", 0, 1, 0.7f);
            CreateOptionSlider(content.transform, "Music Volume", 0, 1, 0.5f);
            CreateOptionSlider(content.transform, "Effects Volume", 0, 1, 0.8f);
            CreateOptionToggle(content.transform, "Fullscreen", true);
            CreateOptionDropdown(content.transform, "Resolution", new string[] { "1920x1080", "1600x900", "1280x720" });
        }
        
        private void CreateCreditsOverlay()
        {
            GameObject overlay = CreateOverlay("Credits");
            
            GameObject content = overlay.transform.Find("Panel/Content").gameObject;
            
            TextMeshProUGUI creditsText = content.AddComponent<TextMeshProUGUI>();
            creditsText.text = "FROSTREALM CHRONICLES\n\n" +
                              "A Warcraft III: The Frozen Throne inspired RTS\n\n" +
                              "Developed by: Solo Developer\n" +
                              "Engine: Unity 6.1 LTS\n" +
                              "AI Tools: Claude & Cursor\n\n" +
                              "Special Thanks:\n" +
                              "- Blizzard Entertainment for the inspiration\n" +
                              "- Unity Technologies\n" +
                              "- Anthropic Claude AI\n\n" +
                              "© 2025 FrostRealm Studios";
            creditsText.fontSize = 20;
            creditsText.alignment = TextAlignmentOptions.Center;
            creditsText.color = new Color(0.8f, 0.9f, 1f);
        }
        
        private GameObject CreateOverlay(string title)
        {
            GameObject overlay = new GameObject($"{title} Overlay");
            overlay.transform.SetParent(mainCanvas.transform, false);
            
            RectTransform overlayRect = overlay.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            
            // Background
            Image bg = overlay.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);
            
            Button bgButton = overlay.AddComponent<Button>();
            bgButton.onClick.AddListener(() => Destroy(overlay));
            
            // Panel
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(overlay.transform, false);
            
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(800, 600);
            
            Image panelImg = panel.AddComponent<Image>();
            panelImg.color = primaryColor;
            
            Outline panelOutline = panel.AddComponent<Outline>();
            panelOutline.effectColor = accentColor;
            panelOutline.effectDistance = new Vector2(3, 3);
            
            // Title
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(panel.transform, false);
            
            RectTransform titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1);
            titleRect.anchorMax = new Vector2(0.5f, 1);
            titleRect.sizeDelta = new Vector2(600, 80);
            titleRect.anchoredPosition = new Vector2(0, -40);
            
            TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = title.ToUpper();
            titleText.fontSize = 36;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            
            // Content area
            GameObject content = new GameObject("Content");
            content.transform.SetParent(panel.transform, false);
            
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = new Vector2(40, 40);
            contentRect.offsetMax = new Vector2(-40, -100);
            
            // Close button
            GameObject closeBtn = new GameObject("Close Button");
            closeBtn.transform.SetParent(panel.transform, false);
            
            RectTransform closeRect = closeBtn.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 1);
            closeRect.anchorMax = new Vector2(1, 1);
            closeRect.sizeDelta = new Vector2(60, 60);
            closeRect.anchoredPosition = new Vector2(-10, -10);
            
            Button close = closeBtn.AddComponent<Button>();
            close.onClick.AddListener(() => Destroy(overlay));
            
            Image closeImg = closeBtn.AddComponent<Image>();
            closeImg.color = Color.red;
            
            TextMeshProUGUI closeText = closeBtn.AddComponent<TextMeshProUGUI>();
            closeText.text = "×";
            closeText.fontSize = 40;
            closeText.alignment = TextAlignmentOptions.Center;
            closeText.color = Color.white;
            
            return overlay;
        }
        
        private void CreateOptionSlider(Transform parent, string label, float min, float max, float defaultValue)
        {
            GameObject option = new GameObject($"Option_{label}");
            option.transform.SetParent(parent, false);
            
            RectTransform rect = option.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 40);
            
            // Add to layout
            LayoutElement layout = option.AddComponent<LayoutElement>();
            layout.preferredHeight = 60;
            
            // Label
            GameObject labelGO = new GameObject("Label");
            labelGO.transform.SetParent(option.transform, false);
            
            RectTransform labelRect = labelGO.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 0.5f);
            labelRect.sizeDelta = new Vector2(200, 30);
            labelRect.anchoredPosition = new Vector2(100, 0);
            
            TextMeshProUGUI labelText = labelGO.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 18;
            labelText.color = Color.white;
            
            // Slider
            GameObject sliderGO = new GameObject("Slider");
            sliderGO.transform.SetParent(option.transform, false);
            
            RectTransform sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
            sliderRect.anchorMax = new Vector2(1, 0.5f);
            sliderRect.offsetMin = new Vector2(0, -10);
            sliderRect.offsetMax = new Vector2(-50, 10);
            
            Slider slider = sliderGO.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = defaultValue;
        }
        
        private void CreateOptionToggle(Transform parent, string label, bool defaultValue)
        {
            GameObject option = new GameObject($"Option_{label}");
            option.transform.SetParent(parent, false);
            
            RectTransform rect = option.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 40);
            
            // Add layout
            HorizontalLayoutGroup layout = option.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.padding = new RectOffset(100, 100, 10, 10);
            
            // Toggle
            GameObject toggleGO = new GameObject("Toggle");
            toggleGO.transform.SetParent(option.transform, false);
            
            Toggle toggle = toggleGO.AddComponent<Toggle>();
            toggle.isOn = defaultValue;
            
            // Label
            GameObject labelGO = new GameObject("Label");
            labelGO.transform.SetParent(option.transform, false);
            
            TextMeshProUGUI labelText = labelGO.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 18;
            labelText.color = Color.white;
        }
        
        private void CreateOptionDropdown(Transform parent, string label, string[] options)
        {
            GameObject option = new GameObject($"Option_{label}");
            option.transform.SetParent(parent, false);
            
            RectTransform rect = option.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 40);
            
            // Implementation would add TMP_Dropdown here
        }
        
        #endregion
        
        #region Audio
        
        private void PlayClickSound()
        {
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.volume = 0.5f;
                // audioSource.PlayOneShot(clickSound);
            }
        }
        
        private void PlayHoverSound()
        {
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.98f, 1.02f);
                audioSource.volume = 0.3f;
                // audioSource.PlayOneShot(hoverSound);
            }
        }
        
        #endregion
    }
}