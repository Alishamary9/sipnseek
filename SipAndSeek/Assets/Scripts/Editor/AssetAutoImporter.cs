using UnityEngine;
using UnityEditor;
using System.IO;
using SipAndSeek.Data;
using SipAndSeek.Managers;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace SipAndSeek.Editor
{
    public class AssetAutoImporter : OdinEditorWindow
    {
        [MenuItem("SipAndSeek/Auto Import Phase 1 Assets")]
        public static void ImportAssets()
        {
            string spritePath = "Assets/Sprites/Phase1";
            string dataPath = "Assets/Data/Phase1";

            if (!AssetDatabase.IsValidFolder("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");
            
            if (!AssetDatabase.IsValidFolder(dataPath))
                AssetDatabase.CreateFolder("Assets/Data", "Phase1");

            AssetDatabase.Refresh();
            string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { spritePath });
            
            Debug.Log($"<color=cyan>Step 1: Checking {guids.Length} textures for Sprite conversion...</color>");
            bool needsReimport = false;

            // Pass 1: Ensure all are sprites with Single mode
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    bool changed = false;

                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        changed = true;
                    }

                    // Force Single sprite mode (Multiple mode with empty sheet = no sprites)
                    if (importer.spriteImportMode != SpriteImportMode.Single)
                    {
                        importer.spriteImportMode = SpriteImportMode.Single;
                        changed = true;
                    }

                    if (!importer.isReadable)
                    {
                        importer.isReadable = true;
                        changed = true;
                    }

                    if (changed)
                    {
                        importer.SaveAndReimport();
                        needsReimport = true;
                        Debug.Log($"<color=yellow>Fixed import settings for: {Path.GetFileName(path)}</color>");
                    }
                }
            }

            if (needsReimport)
            {
                Debug.Log("<color=cyan>Re-importing assets for Sprite conversion...</color>");
                AssetDatabase.Refresh();
            }

            // Pass 2: Create Data Objects
            GameDatabase db = GameDatabase.Instance;
            if (db == null)
            {
                Debug.Log("GameDatabase not found, creating a new one...");
                db = ScriptableObject.CreateInstance<GameDatabase>();
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.CreateAsset(db, "Assets/Resources/GameDatabase.asset");
            }

            Debug.Log("<color=cyan>Step 2: Linking data assets...</color>");
            int successCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);

                // Try loading sprite directly
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

                // Fallback for sprites in sub-assets
                if (sprite == null)
                {
                    Object[] allAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                    foreach (var sub in allAssets)
                    {
                        if (sub is Sprite s)
                        {
                            sprite = s;
                            break;
                        }
                    }
                }

                if (sprite != null)
                {
                    ProcessAsset(fileName, sprite, db, dataPath);
                    successCount++;
                }
                else
                {
                    Debug.LogError($"<color=red>Could not load Sprite for {fileName}. Make sure it's set as 'Sprite (2D and UI)' in Inspector.</color>");
                }
            }

            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"<b><color=green>🎉 Phase 1 Assets Integrated! Success: {successCount}/20</color></b>");
        }

        private static void ProcessAsset(string id, Sprite sprite, GameDatabase db, string folder)
        {
            if (id == "1" || id == "2" || id == "3" || id == "20")
            {
                CreateHiddenImage(id, sprite, db, folder);
            }
            else
            {
                CreateMergeItem(id, sprite, db, folder);
            }
        }

        private static void CreateMergeItem(string id, Sprite sprite, GameDatabase db, string folder)
        {
            string assetPath = $"{folder}/Item_{id}.asset";
            MergeChainItemData data = AssetDatabase.LoadAssetAtPath<MergeChainItemData>(assetPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MergeChainItemData>();
                AssetDatabase.CreateAsset(data, assetPath);
                Debug.Log($"Created MergeItem: {assetPath}");
            }

            data.icon = sprite;
            data.visualDescription = $"Auto-imported asset {id}";
            
            if (!db.mergeChainItems.Contains(data))
                db.mergeChainItems.Add(data);
        }

        private static void CreateHiddenImage(string id, Sprite sprite, GameDatabase db, string folder)
        {
            string assetPath = $"{folder}/Hidden_{id}.asset";
            HiddenImageData data = AssetDatabase.LoadAssetAtPath<HiddenImageData>(assetPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<HiddenImageData>();
                AssetDatabase.CreateAsset(data, assetPath);
                Debug.Log($"Created HiddenImage: {assetPath}");
            }

            data.backgroundSprite = sprite;
            data.imageId = id;
            
            if (!db.hiddenImages.Contains(data))
                db.hiddenImages.Add(data);
        }
    }
}
