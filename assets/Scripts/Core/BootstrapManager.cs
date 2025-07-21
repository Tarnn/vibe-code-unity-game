using UnityEngine;

namespace FrostRealm.Core
{
    /// <summary>
    /// Bootstrap manager that ensures the game starts with all required components.
    /// This creates all managers and systems at runtime.
    /// </summary>
    public class BootstrapManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            Debug.Log("BootstrapManager: Bootstrapping game...");
            
            // Create bootstrap GameObject
            GameObject bootstrap = new GameObject("Bootstrap");
            DontDestroyOnLoad(bootstrap);
            
            // Add all essential components
            bootstrap.AddComponent<DebugLogger>();
            bootstrap.AddComponent<AutoSetup>(); 
            bootstrap.AddComponent<SimpleTestMenu>();
            
            Debug.Log("BootstrapManager: All components added to bootstrap object");
        }
    }
}