using UnityEngine;
using UnityEditor;
using System.IO;

namespace SipAndSeek.Editor
{
    public class AssetCleaner : UnityEditor.EditorWindow
    {
        [MenuItem("SipAndSeek/Safe Clean Old Dummy Data")]
        public static void CleanDummyData()
        {
            string searchFolderPath = "Assets/Resources/Data/MergeChains";
            
            if (!AssetDatabase.IsValidFolder(searchFolderPath))
            {
                Debug.LogWarning($"[AssetCleaner] Folder not found: {searchFolderPath}");
                return;
            }

            string[] assetGuids = AssetDatabase.FindAssets("t:MergeChainItemData", new[] { searchFolderPath });
            int deletedCount = 0;

            foreach (string guid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                // Delete ALL old dummy chains in Resources/Data/MergeChains
                // because all Phase 1 real assets are now stored safely in Assets/Data/Phase1
                if (AssetDatabase.DeleteAsset(path))
                {
                    Debug.Log($"[AssetCleaner] 🗑️ Deleted obsolete dummy asset: {path}");
                    deletedCount++;
                }
            }

            // Also scrub nulls from GameDatabase so it doesn't try to load deleted items
            var db = Managers.GameDatabase.Instance;
            if (db != null)
            {
                int removed = db.mergeChainItems.RemoveAll(item => item == null);
                if (removed > 0)
                {
                    Debug.Log($"[AssetCleaner] 🧹 Removed {removed} empty references from GameDatabase.");
                    EditorUtility.SetDirty(db);
                }
            }

            if (deletedCount > 0)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"<color=green>[AssetCleaner] ✅ Safely removed {deletedCount} old dummy files.</color>");
            }
            else
            {
                Debug.Log("[AssetCleaner] No obsolete dummy assets found to delete.");
            }
        }
    }
}
