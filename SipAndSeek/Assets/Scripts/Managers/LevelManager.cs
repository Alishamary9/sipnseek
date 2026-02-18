using UnityEngine;
using System;
using SipAndSeek;
using SipAndSeek.Data;
using SipAndSeek.Gameplay;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Orchestrates single-level gameplay flow: setup, play, and completion.
    /// Creates grid, spawns items, initializes obstacles, and tracks win/loss.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private RevealManager _revealManager;
        [SerializeField] private ObstacleManager _obstacleManager;
        [SerializeField] private ItemGenerator _itemGenerator;

        [Header("Level Data")]
        [SerializeField] private LevelConfig _currentConfig;

        // ===============================
        // Properties
        // ===============================
        public LevelConfig CurrentConfig => _currentConfig;
        public int MoveLimit => _currentConfig != null ? _currentConfig.moveLimit : -1;
        public bool HasMoveLimit => _currentConfig != null && _currentConfig.HasMoveLimit;

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when level setup is complete and ready to play.</summary>
        public static event Action<LevelConfig> OnLevelReady;

        /// <summary>Fired with progress update (revealed/total).</summary>
        public static event Action<float> OnProgressUpdated;

        // ===============================
        // Lifecycle
        // ===============================
        private void OnEnable()
        {
            RevealManager.OnTileRevealed += HandleTileRevealed;
            RevealManager.OnTargetReached += HandleTargetReached;
            RevealManager.OnImageFullyRevealed += HandleImageFullyRevealed;
        }

        private void OnDisable()
        {
            RevealManager.OnTileRevealed -= HandleTileRevealed;
            RevealManager.OnTargetReached -= HandleTargetReached;
            RevealManager.OnImageFullyRevealed -= HandleImageFullyRevealed;
        }

        // ===============================
        // Level Setup
        // ===============================

        /// <summary>
        /// Load and start a level by its number.
        /// Loads the LevelConfig from Resources and calls SetupLevel.
        /// </summary>
        public void LoadLevel(int levelNumber)
        {
            string path = $"Data/LevelConfigs/Level_{levelNumber}";
            LevelConfig config = Resources.Load<LevelConfig>(path);

            if (config == null)
            {
                Debug.LogError($"[LevelManager] LevelConfig not found at Resources/{path}");
                return;
            }

            SetupLevel(config);
        }

        /// <summary>
        /// Set up a level from a specific LevelConfig asset.
        /// </summary>
        public void SetupLevel(LevelConfig config)
        {
            _currentConfig = config;
            Debug.Log($"[LevelManager] === Setting up Level {config.levelNumber} ===");
            Debug.Log($"  Grid: {config.gridRows}x{config.gridCols}, " +
                      $"Image: {config.imageGridRows}x{config.imageGridCols}, " +
                      $"Obstacles: {config.TotalObstacles}, " +
                      $"Moves: {(config.HasMoveLimit ? config.moveLimit.ToString() : "∞")}");

            // Find managers if not assigned
            FindManagers();

            // Step 1: Create the grid
            _gridManager.CreateGrid(config);

            // Step 2: Initialize merge manager
            _mergeManager.ResetForLevel();

            // Step 3: Initialize reveal system (hidden image)
            _revealManager.Initialize(config);

            // Step 4: Place obstacles
            _obstacleManager.InitializeObstacles(config);

            // Step 5: Set up item generator
            GridCell generatorCell = _gridManager.GetRandomEmptyCell();
            if (_itemGenerator != null && generatorCell != null)
            {
                _itemGenerator.Initialize(config.availableChains, generatorCell);
            }

            // Step 6: Spawn initial items
            SpawnInitialItems(config);

            Debug.Log($"[LevelManager] ✅ Level {config.levelNumber} ready to play!");
            OnLevelReady?.Invoke(config);

            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReadyToPlay();
            }
        }

        private void FindManagers()
        {
            if (_gridManager == null) _gridManager = FindFirstObjectByType<GridManager>();
            if (_mergeManager == null) _mergeManager = FindFirstObjectByType<MergeManager>();
            if (_revealManager == null) _revealManager = FindFirstObjectByType<RevealManager>();
            if (_obstacleManager == null) _obstacleManager = FindFirstObjectByType<ObstacleManager>();
            if (_itemGenerator == null) _itemGenerator = FindFirstObjectByType<ItemGenerator>();

            // Create managers if not found
            if (_gridManager == null) _gridManager = gameObject.AddComponent<GridManager>();
            if (_mergeManager == null) _mergeManager = gameObject.AddComponent<MergeManager>();
            if (_revealManager == null) _revealManager = gameObject.AddComponent<RevealManager>();
            if (_obstacleManager == null) _obstacleManager = gameObject.AddComponent<ObstacleManager>();
            if (_itemGenerator == null) _itemGenerator = gameObject.AddComponent<ItemGenerator>();
        }

        /// <summary>
        /// Spawn initial items on empty cells at level start.
        /// </summary>
        private void SpawnInitialItems(LevelConfig config)
        {
            var database = GameDatabase.Instance;
            if (database == null) return;

            // Get Level 1 items from available chains
            var emptyCells = _gridManager.GetEmptyCells();
            int itemsToSpawn = Mathf.Min(emptyCells.Count / 3, config.availableChains.Count * 3);

            int spawned = 0;
            foreach (var chain in config.availableChains)
            {
                // Spawn 2-3 Level 1 items per chain
                int perChain = Mathf.Max(2, itemsToSpawn / config.availableChains.Count);

                for (int i = 0; i < perChain && spawned < itemsToSpawn; i++)
                {
                    // Find Level 1 data for this chain
                    MergeChainItemData itemData = null;
                    foreach (var data in database.mergeChainItems)
                    {
                        if (data.chainId == chain && data.level == 1)
                        {
                            itemData = data;
                            break;
                        }
                    }

                    if (itemData == null) continue;

                    GridCell cell = _gridManager.GetRandomEmptyCell();
                    if (cell == null) break;

                    MergeItem item = ItemGenerator.CreateMergeItemObject(itemData, cell.WorldPosition);
                    cell.PlaceItem(item);
                    spawned++;
                }
            }

            Debug.Log($"[LevelManager] Spawned {spawned} initial items");
        }

        // ===============================
        // Event Handlers
        // ===============================

        private void HandleTileRevealed(int row, int col, float progress)
        {
            OnProgressUpdated?.Invoke(progress);
        }

        private void HandleTargetReached(float progress)
        {
            Debug.Log($"[LevelManager] 🎯 Target reached! Progress: {progress * 100:F0}%");

            // Level complete — calculate stars
            if (_revealManager != null)
            {
                float targetPercent = _currentConfig != null ? _currentConfig.targetPercent : 0.8f;
                int stars = _revealManager.CalculateStars(targetPercent);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CompleteLevel(stars, progress * 100f);
                }

                // Award rewards based on GDD formula
                AwardLevelRewards(stars);
            }
        }

        private void HandleImageFullyRevealed()
        {
            // Bonus for 100% reveal — guaranteed 3 stars
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState != GameState.LevelComplete)
            {
                GameManager.Instance.CompleteLevel(3, 100f);
                AwardLevelRewards(3);
            }
        }

        // ===============================
        // Rewards (GDD Section 7 Economy)
        // ===============================

        private void AwardLevelRewards(int stars)
        {
            if (PlayerDataManager.Instance == null || _currentConfig == null) return;

            int level = _currentConfig.levelNumber;

            // Coin formula from GDD: Coins_Per_Level(100%) = 50 + (Level × 25)
            int baseCoins = 50 + (level * 25);
            float starMultiplier = stars switch
            {
                3 => 1.0f,
                2 => 0.75f,
                1 => 0.5f,
                _ => 0.25f
            };
            int finalCoins = Mathf.RoundToInt(baseCoins * starMultiplier);

            // Gem formula from GDD: Gems_Per_Level(100%) = 5 + (Level ÷ 2)
            int baseGems = 5 + (level / 2);
            int finalGems = stars >= 3 ? baseGems : (stars >= 2 ? baseGems / 2 : 0);

            // XP award
            int xp = 50 + (level * 10);

            PlayerDataManager.Instance.AddCoins(finalCoins);
            PlayerDataManager.Instance.AddGems(finalGems);
            PlayerDataManager.Instance.AddXP(xp);
            PlayerDataManager.Instance.FlushStats();

            Debug.Log($"[LevelManager] 🏆 Rewards: {finalCoins} coins, {finalGems} gems, {xp} XP");
        }

        // ===============================
        // Level Control
        // ===============================

        /// <summary>
        /// Clean up the current level (for retry or exit).
        /// </summary>
        public void CleanupLevel()
        {
            if (_gridManager != null) _gridManager.ClearGrid();

            Debug.Log("[LevelManager] Level cleaned up.");
        }
    }
}
