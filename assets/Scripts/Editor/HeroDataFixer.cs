using UnityEngine;
using UnityEditor;
using FrostRealm.Data;
using System.IO;
using System.Linq;

namespace FrostRealm.Editor
{
    /// <summary>
    /// Editor utility to automatically fix hero data references.
    /// </summary>
    public class HeroDataFixer : EditorWindow
    {
        [MenuItem("FrostRealm/Fix Hero Data")]
        public static void ShowWindow()
        {
            GetWindow<HeroDataFixer>("Hero Data Fixer");
        }

        void OnGUI()
        {
            GUILayout.Label("Hero Data Fixer", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Fix All Hero Assets"))
            {
                FixAllHeroAssets();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Refresh Hero Registry"))
            {
                RefreshHeroRegistry();
            }
        }

        static void FixAllHeroAssets()
        {
            // Find all hero data assets
            string[] guids = AssetDatabase.FindAssets("t:HeroData");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                HeroData heroData = AssetDatabase.LoadAssetAtPath<HeroData>(path);
                
                if (heroData == null) continue;

                bool wasModified = false;

                // Try to assign portrait if missing
                if (heroData.Portrait == null)
                {
                    Sprite portrait = FindPortraitForHero(heroData.HeroName);
                    if (portrait != null)
                    {
                        SerializedObject so = new SerializedObject(heroData);
                        so.FindProperty("portrait").objectReferenceValue = portrait;
                        so.ApplyModifiedProperties();
                        wasModified = true;
                        Debug.Log($"Assigned portrait to {heroData.HeroName}: {portrait.name}");
                    }
                }

                // Try to assign model prefab if missing
                if (heroData.ModelPrefab == null)
                {
                    GameObject model = FindModelForHero(heroData.HeroName);
                    if (model != null)
                    {
                        SerializedObject so = new SerializedObject(heroData);
                        so.FindProperty("modelPrefab").objectReferenceValue = model;
                        so.ApplyModifiedProperties();
                        wasModified = true;
                        Debug.Log($"Assigned model to {heroData.HeroName}: {model.name}");
                    }
                }

                if (wasModified)
                {
                    EditorUtility.SetDirty(heroData);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Hero data fixing complete!");
        }

        static Sprite FindPortraitForHero(string heroName)
        {
            // Look for PNG files in Art/Heroes folder
            string[] portraitGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art/Heroes" });
            
            foreach (string guid in portraitGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".png"))
                {
                    // Load as sprite
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (sprite != null)
                    {
                        return sprite; // Return the first available sprite for now
                    }
                }
            }
            
            return null;
        }

        static GameObject FindModelForHero(string heroName)
        {
            // Look for FBX files in Art/Heroes folder
            string[] modelGuids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/Art/Heroes" });
            
            foreach (string guid in modelGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".fbx"))
                {
                    GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (model != null)
                    {
                        return model; // Return the first available model for now
                    }
                }
            }
            
            return null;
        }

        static void RefreshHeroRegistry()
        {
            HeroRegistry heroRegistry = Resources.Load<HeroRegistry>("HeroRegistry");
            if (heroRegistry == null)
            {
                Debug.LogError("HeroRegistry not found in Resources folder!");
                return;
            }

            // Find all valid hero data assets
            string[] guids = AssetDatabase.FindAssets("t:HeroData");
            var validHeroes = new System.Collections.Generic.List<HeroData>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                HeroData heroData = AssetDatabase.LoadAssetAtPath<HeroData>(path);
                
                if (heroData != null && heroData.IsValid())
                {
                    validHeroes.Add(heroData);
                }
            }

            // Update the hero registry
            SerializedObject so = new SerializedObject(heroRegistry);
            SerializedProperty availableHeroesProp = so.FindProperty("availableHeroes");
            
            availableHeroesProp.arraySize = validHeroes.Count;
            
            for (int i = 0; i < validHeroes.Count; i++)
            {
                availableHeroesProp.GetArrayElementAtIndex(i).objectReferenceValue = validHeroes[i];
            }

            // Set default hero if we have any
            if (validHeroes.Count > 0)
            {
                SerializedProperty defaultHeroProp = so.FindProperty("defaultHero");
                defaultHeroProp.objectReferenceValue = validHeroes[0];
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(heroRegistry);

            Debug.Log($"Hero registry updated with {validHeroes.Count} valid heroes!");
        }
    }
}