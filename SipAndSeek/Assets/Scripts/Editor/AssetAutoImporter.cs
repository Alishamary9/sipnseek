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
        private const string SOURCE_FOLDER = @"e:\game_test\pics";
        private const string SPRITES_FOLDER = "Assets/Art/Sprites/Phase1";
        private const string DATA_FOLDER = "Assets/Data/Phase1";

        // Map original ID -> (FileName, Type, NameEN, NameAR)
        private static readonly System.Collections.Generic.Dictionary<string, (string newName, string type, string en, string ar)> _assetMap = new()
        {
            // Backgrounds
            { "1", ("Hidden_Level1", "Background", "", "") },
            { "2", ("Hidden_Level2", "Background", "", "") },
            { "3", ("Hidden_Level3", "Background", "", "") },
            { "20", ("Hidden_WindowExtra", "Background", "", "") },

            // Characters
            { "7", ("Char_Laith", "Character", "Laith", "ليث") },
            { "8", ("Char_Grandma", "Character", "Grandma", "الجدة") },

            // Coffee Chain (ChainId: coffee)
            { "4", ("Item_Coffee_Lv1_Bean", "Merge", "Coffee Bean", "حبة قهوة") },
            { "9", ("Item_Coffee_Lv2_Bag", "Merge", "Coffee Bag", "كيس بن") },
            { "10", ("Item_Coffee_Lv3_Grinder", "Merge", "Grinder", "مطحنة") },
            { "11", ("Item_Coffee_Lv4_Dallah", "Merge", "Dallah", "دلة") },
            { "6", ("Item_Coffee_Lv5_Cup", "Merge", "Coffee Cup", "كوب قهوة") },

            // Travel Chain (ChainId: travel)
            { "12", ("Item_Travel_Lv1_Ticket", "Merge", "Ticket", "تذكرة") },
            { "13", ("Item_Travel_Lv2_Suitcase", "Merge", "Suitcase", "حقيبة") },
            { "18", ("Item_Travel_Lv3_Compass", "Merge", "Compass", "بوصلة") },

            // UI & Special
            { "14", ("Logo_Variant1", "UI", "", "") },
            { "15", ("Logo_Variant2", "UI", "", "") },
            { "16", ("Icon_Coin", "UI", "", "") },
            { "17", ("Icon_Gem", "UI", "", "") },
            { "19", ("Item_Special_Spoon", "Item", "Silver Spoon", "ملعقة فضية") }
        };

        [MenuItem("SipAndSeek/Auto Import Phase 1 Assets")]
        public static void ImportAssets()
        {
            EnsureFolders();

            Debug.Log("<color=cyan>Step 1: Copying and configuring Sprites...</color>");
            CopyAndConfigureSprites();

            Debug.Log("<color=cyan>Step 2: Linking data assets...</color>");
            CreateDataAssets();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"<b><color=green>🎉 Phase 1 Assets Integrated Successfully!</color></b>");
        }

        private static void EnsureFolders()
        {
            EnsureFolder(SPRITES_FOLDER);
            EnsureFolder(DATA_FOLDER);
            EnsureFolder("Assets/Resources");
            AssetDatabase.Refresh();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder($"{current}/{parts[i]}"))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current += $"/{parts[i]}";
            }
        }

        private static void CopyAndConfigureSprites()
        {
            if (!Directory.Exists(SOURCE_FOLDER))
            {
                Debug.LogError($"Source folder missing: {SOURCE_FOLDER}");
                return;
            }

            int count = 0;
            foreach (var kvp in _assetMap)
            {
                string sourceFile = Path.Combine(SOURCE_FOLDER, $"{kvp.Key}.jpeg");
                if (!File.Exists(sourceFile))
                {
                    Debug.LogWarning($"Missing file: {sourceFile}");
                    continue;
                }

                string targetPath = $"{SPRITES_FOLDER}/{kvp.Value.newName}.jpeg";
                File.Copy(sourceFile, targetPath, true);
                count++;
            }
            AssetDatabase.Refresh();

            // Configure
            foreach (var kvp in _assetMap)
            {
                string targetPath = $"{SPRITES_FOLDER}/{kvp.Value.newName}.jpeg";
                TextureImporter importer = AssetImporter.GetAtPath(targetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;
                    importer.maxTextureSize = kvp.Value.type == "Background" ? 2048 : 512;
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                }
            }
            AssetDatabase.Refresh();
        }

        private static void CreateDataAssets()
        {
            GameDatabase db = GameDatabase.Instance;
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<GameDatabase>();
                AssetDatabase.CreateAsset(db, "Assets/Resources/GameDatabase.asset");
            }

            foreach (var kvp in _assetMap)
            {
                string spritePath = $"{SPRITES_FOLDER}/{kvp.Value.newName}.jpeg";
                
                // Refresh specific asset to ensure its representations are updated
                AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                
                if (sprite == null) 
                {
                    // Fallback to checking all representations
                    Object[] allAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(spritePath);
                    foreach (var asset in allAssets)
                    {
                        if (asset is Sprite s)
                        {
                            sprite = s;
                            break;
                        }
                    }

                    if (sprite == null)
                    {
                        Debug.LogWarning($"Could not load sprite for {spritePath}. Check if the image was imported as a Sprite.");
                        continue;
                    }
                }

                var data = kvp.Value;
                if (data.type == "Background")
                {
                    CreateHiddenImage(kvp.Key, data.newName, sprite, db);
                }
                else if (data.type == "Merge")
                {
                    CreateMergeItem(kvp.Key, data.newName, sprite, db, data.en, data.ar);
                }
            }
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets(); // Force save to disk so Play Mode doesn't lose data
        }

        private static void CreateMergeItem(string id, string assetName, Sprite sprite, GameDatabase db, string en, string ar)
        {
            string path = $"{DATA_FOLDER}/{assetName}.asset";
            MergeChainItemData data = AssetDatabase.LoadAssetAtPath<MergeChainItemData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MergeChainItemData>();
                AssetDatabase.CreateAsset(data, path);
            }

            // Assign variables based on name
            data.icon = sprite;
            data.itemNameEN = en;
            data.itemNameAR = ar;
            data.visualDescription = en;
            
            if (assetName.Contains("Coffee")) data.chainId = "coffee";
            else if (assetName.Contains("Travel")) data.chainId = "travel";

            // Extract level from name (e.g., Lv2)
            var match = System.Text.RegularExpressions.Regex.Match(assetName, @"Lv(\d+)");
            if (match.Success) 
            {
                data.level = int.Parse(match.Groups[1].Value);
                data.sellPrice = data.level * 2;
                data.spawnWeight = (data.level == 1) ? 1.0f : 0.0f;
            }

            EditorUtility.SetDirty(data);
            if (!db.mergeChainItems.Contains(data)) db.mergeChainItems.Add(data);
        }

        private static void CreateHiddenImage(string id, string assetName, Sprite sprite, GameDatabase db)
        {
            string path = $"{DATA_FOLDER}/{assetName}.asset";
            HiddenImageData data = AssetDatabase.LoadAssetAtPath<HiddenImageData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<HiddenImageData>();
                AssetDatabase.CreateAsset(data, path);
            }

            data.backgroundSprite = sprite;
            data.imageId = assetName;
            
            EditorUtility.SetDirty(data);
            if (!db.hiddenImages.Contains(data)) db.hiddenImages.Add(data);
        }
    }
}
