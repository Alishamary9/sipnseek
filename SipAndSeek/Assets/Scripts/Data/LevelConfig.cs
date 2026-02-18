using UnityEngine;
using System.Collections.Generic;
using SipAndSeek;

namespace SipAndSeek.Data
{
    /// <summary>
    /// Configuration data for a single level.
    /// Created via LevelConfigGenerator editor tool or manually.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevelConfig", menuName = "SipAndSeek/Data/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Basic Info")]
        public int levelNumber;
        public Difficulty difficulty;

        [Header("Grid Setup")]
        public int gridRows = 5;
        public int gridCols = 5;

        [Header("Hidden Image")]
        public int imageGridRows = 3;
        public int imageGridCols = 3;
        public Sprite hiddenImage;
        public string imageId;

        [Header("Objectives")]
        [Range(0.5f, 1f)]
        public float targetPercent = 0.8f; // 80% to pass
        public int moveLimit = -1; // -1 = unlimited

        [Header("Available Merge Chains")]
        public List<string> availableChains = new List<string>();

        [Header("Obstacle Distribution")]
        public int lockedTiles;
        public int frozenTiles;
        public int keyLockTiles;
        public int darkTiles;
        public int goldenTiles;

        [Header("Narrative")]
        public string narrativeId;

        /// <summary>
        /// Total number of obstacles in this level.
        /// </summary>
        public int TotalObstacles =>
            lockedTiles + frozenTiles + keyLockTiles + darkTiles + goldenTiles;

        /// <summary>
        /// Total number of image tiles to reveal.
        /// </summary>
        public int TotalImageTiles => imageGridRows * imageGridCols;

        /// <summary>
        /// Total number of grid cells.
        /// </summary>
        public int TotalGridCells => gridRows * gridCols;

        /// <summary>
        /// Whether this level has a move limit.
        /// </summary>
        public bool HasMoveLimit => moveLimit > 0;
    }
}
