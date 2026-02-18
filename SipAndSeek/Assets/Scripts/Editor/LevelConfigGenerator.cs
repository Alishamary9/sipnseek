#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SipAndSeek;
using SipAndSeek.Data;
using System.Collections.Generic;

namespace SipAndSeek.Editor
{
    /// <summary>
    /// Editor tool that generates LevelConfig ScriptableObjects for Levels 1-3
    /// based on the GDD difficulty curve (Section 5).
    /// </summary>
    public class LevelConfigGenerator
    {
        [MenuItem("Tools/Sip & Seek/Generate Level Configs")]
        public static void GenerateAllLevelConfigs()
        {
            string basePath = "Assets/Resources/Data/LevelConfigs";

            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Data/LevelConfigs"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Data"))
                {
                    AssetDatabase.CreateFolder("Assets/Resources", "Data");
                }
                AssetDatabase.CreateFolder("Assets/Resources/Data", "LevelConfigs");
            }

            // Generate Levels 1-3 from GDD Section 5 difficulty table
            GenerateLevel1(basePath);
            GenerateLevel2(basePath);
            GenerateLevel3(basePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("🎉 Level Configs generated for Levels 1-3!");
        }

        // =============================================
        // Level 1: Discovery — Tutorial / Introduction
        // Grid 5x5, Image 3x3, No obstacles, 2 chains
        // =============================================
        private static void GenerateLevel1(string basePath)
        {
            LevelConfig config = ScriptableObject.CreateInstance<LevelConfig>();
            config.levelNumber = 1;
            config.difficulty = Difficulty.VeryEasy;

            // Grid
            config.gridRows = 5;
            config.gridCols = 5;

            // Hidden Image
            config.imageGridRows = 3;
            config.imageGridCols = 3;
            config.imageId = "img_001";

            // Objectives
            config.targetPercent = 0.8f; // 80% to pass
            config.moveLimit = -1; // Unlimited moves

            // Merge chains: Coffee & Tea (2 chains, 4 levels each)
            config.availableChains = new List<string> { "coffee", "tea" };

            // No obstacles in Level 1
            config.lockedTiles = 0;
            config.frozenTiles = 0;
            config.keyLockTiles = 0;
            config.darkTiles = 0;
            config.goldenTiles = 0;

            // Narrative
            config.narrativeId = "level_1";

            CreateAsset(config, $"{basePath}/Level_1.asset");
        }

        // =============================================
        // Level 2: Still Easy — Lock introduced
        // Grid 5x5, Image 3x3, 2 locked tiles, 2 chains
        // =============================================
        private static void GenerateLevel2(string basePath)
        {
            LevelConfig config = ScriptableObject.CreateInstance<LevelConfig>();
            config.levelNumber = 2;
            config.difficulty = Difficulty.Easy;

            // Grid
            config.gridRows = 5;
            config.gridCols = 5;

            // Hidden Image
            config.imageGridRows = 3;
            config.imageGridCols = 3;
            config.imageId = "img_002";

            // Objectives
            config.targetPercent = 0.8f;
            config.moveLimit = -1;

            // Merge chains: Coffee, Tea + Sweets
            config.availableChains = new List<string> { "coffee", "tea", "sweets" };

            // Obstacles: 2 Locked tiles
            config.lockedTiles = 2;
            config.frozenTiles = 0;
            config.keyLockTiles = 0;
            config.darkTiles = 0;
            config.goldenTiles = 0;

            // Narrative
            config.narrativeId = "level_2";

            CreateAsset(config, $"{basePath}/Level_2.asset");
        }

        // =============================================
        // Level 3: Growing — Ice introduced
        // Grid 6x6, Image 4x4, 3 locked + 2 frozen, 3 chains
        // =============================================
        private static void GenerateLevel3(string basePath)
        {
            LevelConfig config = ScriptableObject.CreateInstance<LevelConfig>();
            config.levelNumber = 3;
            config.difficulty = Difficulty.MediumEasy;

            // Grid
            config.gridRows = 6;
            config.gridCols = 6;

            // Hidden Image
            config.imageGridRows = 4;
            config.imageGridCols = 4;
            config.imageId = "img_003";

            // Objectives
            config.targetPercent = 0.8f;
            config.moveLimit = -1;

            // Merge chains: Coffee, Tea, Sweets
            config.availableChains = new List<string> { "coffee", "tea", "sweets" };

            // Obstacles: 3 Locked + 2 Frozen
            config.lockedTiles = 3;
            config.frozenTiles = 2;
            config.keyLockTiles = 0;
            config.darkTiles = 0;
            config.goldenTiles = 0;

            // Narrative
            config.narrativeId = "level_3";

            CreateAsset(config, $"{basePath}/Level_3.asset");
        }

        // ===============================
        // Helper
        // ===============================
        private static void CreateAsset<T>(T asset, string path) where T : ScriptableObject
        {
            T existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                EditorUtility.CopySerialized(asset, existing);
                Debug.Log($"  ✅ Updated: {path}");
            }
            else
            {
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"  ✅ Created: {path}");
            }
        }
    }
}
#endif
