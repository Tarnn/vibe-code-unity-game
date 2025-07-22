using UnityEngine;
using UnityEditor;
using System.IO;

namespace FrostRealm.Editor
{
    /// <summary>
    /// Editor utility to convert textures to sprites for hero portraits.
    /// </summary>
    public class TextureToSpriteConverter : EditorWindow
    {
        [MenuItem("FrostRealm/Convert Hero Textures to Sprites")]
        public static void ShowWindow()
        {
            GetWindow<TextureToSpriteConverter>("Texture to Sprite Converter");
        }

        void OnGUI()
        {
            GUILayout.Label("Hero Portrait Converter", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("This will convert all hero portrait textures in Art/Heroes to sprites for use in UI.", MessageType.Info);
            
            if (GUILayout.Button("Convert All Hero Portraits to Sprites"))
            {
                ConvertHeroPortraits();
            }
        }

        static void ConvertHeroPortraits()
        {
            string[] folders = { "Assets/Art/Heroes", "Assets/heros" };
            int convertedCount = 0;

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                    continue;

                string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });

                foreach (string guid in textureGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    
                    // Only process PNG files
                    if (!path.EndsWith(".png"))
                        continue;

                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer != null)
                    {
                        bool needsUpdate = false;

                        // Convert to sprite
                        if (importer.textureType != TextureImporterType.Sprite)
                        {
                            importer.textureType = TextureImporterType.Sprite;
                            needsUpdate = true;
                        }

                        // Set sprite settings
                        if (importer.spriteImportMode != SpriteImportMode.Single)
                        {
                            importer.spriteImportMode = SpriteImportMode.Single;
                            needsUpdate = true;
                        }

                        // Set texture settings for UI
                        TextureImporterSettings settings = new TextureImporterSettings();
                        importer.ReadTextureSettings(settings);
                        
                        if (settings.spritePixelsPerUnit != 100)
                        {
                            settings.spritePixelsPerUnit = 100;
                            needsUpdate = true;
                        }

                        if (settings.filterMode != FilterMode.Bilinear)
                        {
                            settings.filterMode = FilterMode.Bilinear;
                            needsUpdate = true;
                        }

                        if (needsUpdate)
                        {
                            importer.SetTextureSettings(settings);
                            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                            convertedCount++;
                            Debug.Log($"Converted texture to sprite: {path}");
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"Portrait conversion complete! Converted {convertedCount} textures to sprites.");
        }
    }
}