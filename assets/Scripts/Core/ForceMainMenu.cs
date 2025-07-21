using UnityEngine;

namespace FrostRealm.Core
{
    /// <summary>
    /// Forces the main menu to appear immediately on game start.
    /// This is a debug/fallback script to ensure the menu always shows.
    /// </summary>
    [DefaultExecutionOrder(-2000)]
    public class ForceMainMenu : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log("ForceMainMenu: Game started, forcing main menu creation");
            
            // Ensure we have a main menu
            if (FindFirstObjectByType<FrostRealm.UI.RuntimeMainMenu>() == null)
            {
                Debug.Log("ForceMainMenu: No RuntimeMainMenu found, creating one now");
                GameObject menuGO = new GameObject("RuntimeMainMenu");
                menuGO.AddComponent<FrostRealm.UI.RuntimeMainMenu>();
                Debug.Log("ForceMainMenu: RuntimeMainMenu created successfully");
            }
            else
            {
                Debug.Log("ForceMainMenu: RuntimeMainMenu already exists");
            }
            
            // Configure camera for proper rendering
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.Log("ForceMainMenu: No main camera found, creating one");
                GameObject camGO = new GameObject("Main Camera");
                cam = camGO.AddComponent<Camera>();
                cam.tag = "MainCamera";
            }
            
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.2f); // Dark blue background
            cam.orthographic = false;
            cam.fieldOfView = 60f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 1000f;
            
            Debug.Log("ForceMainMenu: Camera configured for UI rendering");
        }
    }
}