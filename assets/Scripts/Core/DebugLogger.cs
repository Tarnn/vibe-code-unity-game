using UnityEngine;
using System.IO;

namespace FrostRealm.Core
{
    /// <summary>
    /// Debug logger that writes to a file to help debug runtime issues
    /// </summary>
    [DefaultExecutionOrder(-5000)]
    public class DebugLogger : MonoBehaviour
    {
        private string logPath;
        
        void Awake()
        {
            logPath = Path.Combine(Application.persistentDataPath, "debug_runtime.log");
            
            // Clear previous log
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            
            WriteLog("=== FROSTREALM CHRONICLES DEBUG LOG ===");
            WriteLog($"Unity Version: {Application.unityVersion}");
            WriteLog($"Platform: {Application.platform}");
            WriteLog($"Screen: {Screen.width}x{Screen.height}");
            WriteLog($"Graphics Device: {SystemInfo.graphicsDeviceName}");
            WriteLog($"Graphics API: {SystemInfo.graphicsDeviceType}");
            WriteLog($"Render Pipeline: {QualitySettings.renderPipeline?.name ?? "Built-in"}");
            WriteLog($"Data Path: {Application.dataPath}");
            WriteLog($"Persistent Data Path: {Application.persistentDataPath}");
            WriteLog("===========================================");
            
            // Hook into Unity's log system
            Application.logMessageReceived += OnLogReceived;
        }
        
        void Start()
        {
            WriteLog("DebugLogger: Start() called");
            WriteLog($"Active Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            WriteLog($"Scene Path: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().path}");
            
            // Check for UI components
            var canvas = FindObjectOfType<Canvas>();
            WriteLog($"Canvas found: {canvas != null}");
            
            var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            WriteLog($"EventSystem found: {eventSystem != null}");
            
            var camera = Camera.main;
            WriteLog($"Main Camera found: {camera != null}");
            if (camera != null)
            {
                WriteLog($"Camera position: {camera.transform.position}");
                WriteLog($"Camera rotation: {camera.transform.rotation}");
                WriteLog($"Camera clear flags: {camera.clearFlags}");
                WriteLog($"Camera background color: {camera.backgroundColor}");
            }
        }
        
        private void OnLogReceived(string logString, string stackTrace, LogType type)
        {
            WriteLog($"[{type}] {logString}");
            if (type == LogType.Error || type == LogType.Exception)
            {
                WriteLog($"Stack: {stackTrace}");
            }
        }
        
        private void WriteLog(string message)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] {message}";
            
            try
            {
                File.AppendAllText(logPath, logEntry + "\n");
                Debug.Log(logEntry); // Also log to Unity console
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to write to log file: {e.Message}");
            }
        }
        
        void OnDestroy()
        {
            Application.logMessageReceived -= OnLogReceived;
            WriteLog("DebugLogger: OnDestroy() called");
        }
    }
}