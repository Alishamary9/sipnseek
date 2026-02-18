using UnityEngine;
using System;
using System.Collections.Generic;
using SipAndSeek;
using SipAndSeek.Gameplay;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Manages obstacle placement and clearing during gameplay.
    /// Spawns obstacles based on LevelConfig and handles obstacle removal events.
    /// </summary>
    public class ObstacleManager : MonoBehaviour
    {
        public static ObstacleManager Instance { get; private set; }

        // ===============================
        // State
        // ===============================
        private int _totalObstacles;
        private int _clearedObstacles;
        private Dictionary<TileState, int> _obstacleCount = new Dictionary<TileState, int>();

        // ===============================
        // Properties
        // ===============================
        public int TotalObstacles => _totalObstacles;
        public int ClearedObstacles => _clearedObstacles;
        public int RemainingObstacles => _totalObstacles - _clearedObstacles;

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when an obstacle is cleared (type, remaining).</summary>
        public static event Action<TileState, int> OnObstacleCleared;

        /// <summary>Fired when all obstacles of a specific type are cleared.</summary>
        public static event Action<TileState> OnObstacleTypeCleared;

        /// <summary>Fired when ALL obstacles are cleared.</summary>
        public static event Action OnAllObstaclesCleared;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ===============================
        // Initialization
        // ===============================

        /// <summary>
        /// Place obstacles on the grid based on level configuration.
        /// </summary>
        public void InitializeObstacles(LevelConfig config)
        {
            _totalObstacles = 0;
            _clearedObstacles = 0;
            _obstacleCount.Clear();

            if (GridManager.Instance == null)
            {
                Debug.LogError("[ObstacleManager] GridManager not found!");
                return;
            }

            // Collect available cells (not image center cells)
            List<GridCell> availableCells = new List<GridCell>();
            for (int r = 0; r < config.gridRows; r++)
            {
                for (int c = 0; c < config.gridCols; c++)
                {
                    GridCell cell = GridManager.Instance.GetCell(r, c);
                    if (cell != null && cell.IsEmpty)
                    {
                        // Don't place obstacles in the center where the image is
                        if (RevealManager.Instance == null || !RevealManager.Instance.IsImagePosition(r, c))
                        {
                            availableCells.Add(cell);
                        }
                    }
                }
            }

            // Shuffle available cells for random placement
            ShuffleList(availableCells);

            int cellIndex = 0;

            // Place Locked tiles
            cellIndex = PlaceObstacleType(availableCells, cellIndex, config.lockedTiles, TileState.Locked);

            // Place Frozen tiles
            cellIndex = PlaceObstacleType(availableCells, cellIndex, config.frozenTiles, TileState.Frozen);

            // Place KeyLock tiles
            cellIndex = PlaceObstacleType(availableCells, cellIndex, config.keyLockTiles, TileState.KeyLocked);

            // Place Dark tiles
            cellIndex = PlaceObstacleType(availableCells, cellIndex, config.darkTiles, TileState.Dark);

            // Place Golden tiles (bonus, not obstacles)
            cellIndex = PlaceObstacleType(availableCells, cellIndex, config.goldenTiles, TileState.Golden);

            Debug.Log($"[ObstacleManager] Placed {_totalObstacles} obstacles " +
                      $"(🔒{config.lockedTiles} 🧊{config.frozenTiles} 🔑{config.keyLockTiles} " +
                      $"🌑{config.darkTiles} ⭐{config.goldenTiles})");
        }

        private int PlaceObstacleType(List<GridCell> cells, int startIndex, int count, TileState type)
        {
            int placed = 0;
            for (int i = startIndex; i < cells.Count && placed < count; i++, placed++)
            {
                cells[i].SetObstacle(type);
                _totalObstacles++;
                startIndex = i + 1;

                if (!_obstacleCount.ContainsKey(type))
                    _obstacleCount[type] = 0;
                _obstacleCount[type]++;
            }
            return startIndex;
        }

        // ===============================
        // Obstacle Clearing
        // ===============================

        /// <summary>
        /// Notify that an obstacle was cleared from a cell.
        /// </summary>
        public void OnObstacleRemoved(TileState type)
        {
            _clearedObstacles++;

            if (_obstacleCount.ContainsKey(type))
            {
                _obstacleCount[type]--;

                OnObstacleCleared?.Invoke(type, _obstacleCount[type]);

                if (_obstacleCount[type] <= 0)
                {
                    Debug.Log($"[ObstacleManager] All {type} obstacles cleared!");
                    OnObstacleTypeCleared?.Invoke(type);
                }
            }

            // Check if all obstacles done
            if (_clearedObstacles >= _totalObstacles)
            {
                Debug.Log("[ObstacleManager] 🎉 All obstacles cleared!");
                OnAllObstaclesCleared?.Invoke();
            }

            // Play obstacle clear SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("sfx_obstacle_clear");
            }
        }

        /// <summary>
        /// Try to unlock an obstacle on a specific cell using context.
        /// Returns true if the obstacle was fully cleared.
        /// </summary>
        public bool TryUnlockObstacle(GridCell cell, int mergeLevel = 0, bool hasKey = false, bool hasLight = false)
        {
            if (cell == null || !cell.HasObstacle) return false;

            TileState obstacleType = cell.State;
            bool cleared = cell.TryUnlockObstacle(mergeLevel, hasKey, hasLight);

            if (cleared)
            {
                OnObstacleRemoved(obstacleType);
            }

            return cleared;
        }

        // ===============================
        // Queries
        // ===============================

        /// <summary>
        /// Get count of remaining obstacles of a specific type.
        /// </summary>
        public int GetObstacleCount(TileState type)
        {
            return _obstacleCount.ContainsKey(type) ? _obstacleCount[type] : 0;
        }

        /// <summary>
        /// Check if there are any Key Lock obstacles remaining.
        /// </summary>
        public bool HasKeyLocks => GetObstacleCount(TileState.KeyLocked) > 0;

        /// <summary>
        /// Check if there are any Dark obstacles remaining.
        /// </summary>
        public bool HasDarkTiles => GetObstacleCount(TileState.Dark) > 0;

        // ===============================
        // Helpers
        // ===============================

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
