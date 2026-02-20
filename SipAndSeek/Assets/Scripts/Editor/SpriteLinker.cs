using UnityEngine;
using UnityEditor;
using SipAndSeek.Data;
using System.IO;

namespace SipAndSeek.Editor
{
    public class SpriteLinker : EditorWindow
    {
        [MenuItem("Tools/Sip & Seek/Auto-Link Sprites (Phase 1)")]
        public static void LinkSprites()
        {
            Debug.Log("[SpriteLinker] Starting auto-link process...");

            // 1. Link Hidden Images (1.jpeg -> img_001, 2.jpeg -> img_002, 3.jpeg -> img_003)
            LinkHiddenImage("img_001", "1.jpeg");
            LinkHiddenImage("img_002", "2.jpeg");
            LinkHiddenImage("img_003", "3.jpeg");

            // 2. Link Merge Items (remaining 17 images: 4 through 20)
            int[] availableSpriteIndices = { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            int ptr = 0;

            // Coffee (5)
            for (int i = 1; i <= 5; i++)
            {
                if (ptr < availableSpriteIndices.Length) LinkMergeItem("coffee", i, $"{availableSpriteIndices[ptr++]}.jpeg");
            }

            // Tea (5)
            for (int i = 1; i <= 5; i++)
            {
                if (ptr < availableSpriteIndices.Length) LinkMergeItem("tea", i, $"{availableSpriteIndices[ptr++]}.jpeg");
            }

            // Travel (6)
            for (int i = 1; i <= 6; i++)
            {
                if (ptr < availableSpriteIndices.Length) LinkMergeItem("travel", i, $"{availableSpriteIndices[ptr++]}.jpeg");
            }

            // Tools (6) - Only first 2 will get sprites due to 22 image limit
            for (int i = 1; i <= 6; i++)
            {
                if (ptr < availableSpriteIndices.Length) LinkMergeItem("tools", i, $"{availableSpriteIndices[ptr++]}.jpeg");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[SpriteLinker] ✅ Auto-link complete! All data assets updated.");
        }

        private static void LinkHiddenImage(string assetId, string spriteName)
        {
            string assetPath = $"Assets/Resources/Data/HiddenImages/{assetId}.asset";
            string spritePath = $"Assets/Sprites/Phase1/{spriteName}";

            HiddenImageData data = AssetDatabase.LoadAssetAtPath<HiddenImageData>(assetPath);
            Sprite sprite = LoadSpriteFromPath(spritePath);

            if (data != null && sprite != null)
            {
                data.backgroundSprite = sprite;
                EditorUtility.SetDirty(data);
                Debug.Log($"Linked {spriteName} -> {assetId}");
            }
            else
            {
                Debug.LogWarning($"Failed to link {spriteName} to {assetId}. Data: {data != null}, Sprite: {sprite != null}");
            }
        }

        private static void LinkMergeItem(string chainId, int level, string spriteName)
        {
            string assetPath = $"Assets/Resources/Data/MergeChains/{chainId}_{level}.asset";
            string spritePath = $"Assets/Sprites/Phase1/{spriteName}";

            MergeChainItemData data = AssetDatabase.LoadAssetAtPath<MergeChainItemData>(assetPath);
            Sprite sprite = LoadSpriteFromPath(spritePath);

            if (data != null && sprite != null)
            {
                data.icon = sprite;
                EditorUtility.SetDirty(data);
                Debug.Log($"Linked {spriteName} -> {chainId}_{level}");
            }
            else
            {
                Debug.LogWarning($"Failed to link {spriteName} to {chainId}_{level}. Data: {data != null}, Sprite: {sprite != null}");
            }
        }

        private static Sprite LoadSpriteFromPath(string path)
        {
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var asset in allAssets)
            {
                if (asset is Sprite s)
                {
                    return s;
                }
            }
            return null;
        }
    }
}
