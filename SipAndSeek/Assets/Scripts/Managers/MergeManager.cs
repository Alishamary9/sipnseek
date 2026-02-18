using UnityEngine;
using System;
using System.Collections.Generic;
using SipAndSeek.Gameplay;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Handles merge logic: validates merges, creates upgraded items,
    /// tracks merge counts, and triggers reveals.
    /// </summary>
    public class MergeManager : MonoBehaviour
    {
        public static MergeManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int _mergesPerReveal = 3; // Every N merges reveals a tile

        // ===============================
        // State
        // ===============================
        private int _mergesSinceLastReveal;
        private int _totalMergesThisLevel;
        private int _movesUsed;

        // ===============================
        // Properties
        // ===============================
        public int TotalMergesThisLevel => _totalMergesThisLevel;
        public int MovesUsed => _movesUsed;
        public int MergesUntilReveal => Mathf.Max(0, _mergesPerReveal - _mergesSinceLastReveal);

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when a successful merge occurs (mergedItem, newItem).</summary>
        public static event Action<MergeItem, MergeItem> OnMergePerformed;

        /// <summary>Fired when enough merges trigger a tile reveal.</summary>
        public static event Action<int> OnMergeThresholdReached; // total merges

        /// <summary>Fired when a merge fails (invalid combo).</summary>
        public static event Action OnMergeFailed;

        /// <summary>Fired when moves update (current, limit). Limit = -1 means unlimited.</summary>
        public static event Action<int, int> OnMovesChanged;

        /// <summary>Fired when max-level merge produces coins.</summary>
        public static event Action<int> OnSellItem; // coins earned

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

        /// <summary>
        /// Reset merge state for a new level.
        /// </summary>
        public void ResetForLevel()
        {
            _mergesSinceLastReveal = 0;
            _totalMergesThisLevel = 0;
            _movesUsed = 0;
        }

        // ===============================
        // Core Merge Logic
        // ===============================

        /// <summary>
        /// Attempt to merge two items at the target cell.
        /// Called by GridManager when a valid merge drop is detected.
        /// </summary>
        public void PerformMerge(MergeItem draggedItem, MergeItem targetItem, GridCell targetCell)
        {
            if (!CanMerge(draggedItem, targetItem))
            {
                OnMergeFailed?.Invoke();
                draggedItem.ReturnToStartPosition();
                return;
            }

            string chainId = draggedItem.ChainId;
            int currentLevel = draggedItem.Level;
            int nextLevel = currentLevel + 1;

            // Find the next level item data
            MergeChainItemData nextLevelData = FindChainItem(chainId, nextLevel);

            // Track merge
            _totalMergesThisLevel++;
            _mergesSinceLastReveal++;
            _movesUsed++;

            // Update player stats
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.AddMergeCount(1);
            }

            // Remove source items
            GridCell sourceCell = draggedItem.CurrentCell;
            if (sourceCell != null) sourceCell.RemoveItem();
            targetCell.RemoveItem();

            // Destroy old item objects
            Destroy(draggedItem.gameObject);
            Destroy(targetItem.gameObject);

            if (nextLevelData != null)
            {
                // Create upgraded item
                MergeItem newItem = ItemGenerator.CreateMergeItemObject(nextLevelData, targetCell.WorldPosition);
                targetCell.PlaceItem(newItem);

                Debug.Log($"[MergeManager] ✅ Merged {chainId} Lv{currentLevel} → Lv{nextLevel}");
                OnMergePerformed?.Invoke(targetItem, newItem);

                // Play merge SFX
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX("sfx_merge");
                }
            }
            else
            {
                // Max level reached — sell for coins
                int sellPrice = draggedItem.Data != null ? draggedItem.Data.sellPrice : 10;
                int totalCoins = sellPrice * 2; // Bonus for reaching max level

                if (PlayerDataManager.Instance != null)
                {
                    PlayerDataManager.Instance.AddCoins(totalCoins);
                }

                Debug.Log($"[MergeManager] 💰 Max level! Sold {chainId} for {totalCoins} coins");
                OnSellItem?.Invoke(totalCoins);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX("sfx_sell");
                }
            }

            // Check reveal threshold
            if (_mergesSinceLastReveal >= _mergesPerReveal)
            {
                _mergesSinceLastReveal = 0;
                OnMergeThresholdReached?.Invoke(_totalMergesThisLevel);

                // Trigger reveal via RevealManager
                if (RevealManager.Instance != null)
                {
                    RevealManager.Instance.RevealNextTile();
                }
            }

            // Fire moves changed event
            int moveLimit = -1;
            if (GameManager.Instance != null)
            {
                var levelMgr = FindFirstObjectByType<LevelManager>();
                if (levelMgr != null)
                {
                    moveLimit = levelMgr.MoveLimit;
                }
            }
            OnMovesChanged?.Invoke(_movesUsed, moveLimit);

            // Check if out of moves
            if (moveLimit > 0 && _movesUsed >= moveLimit)
            {
                Debug.Log("[MergeManager] ❌ Out of moves!");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.FailLevel();
                }
            }
        }

        // ===============================
        // Validation
        // ===============================

        /// <summary>
        /// Check if two items can merge (same chain, same level).
        /// </summary>
        public bool CanMerge(MergeItem a, MergeItem b)
        {
            if (a == null || b == null) return false;
            return a.CanMergeWith(b);
        }

        // ===============================
        // Data Lookup
        // ===============================

        /// <summary>
        /// Find a specific chain item by chain ID and level.
        /// </summary>
        private MergeChainItemData FindChainItem(string chainId, int level)
        {
            var database = GameDatabase.Instance;
            if (database == null) return null;

            foreach (var item in database.mergeChainItems)
            {
                if (item.chainId == chainId && item.level == level)
                {
                    return item;
                }
            }

            return null; // No next level — max reached
        }

        /// <summary>
        /// Get the maximum level in a specific chain.
        /// </summary>
        public int GetMaxLevel(string chainId)
        {
            var database = GameDatabase.Instance;
            if (database == null) return 1;

            int maxLevel = 1;
            foreach (var item in database.mergeChainItems)
            {
                if (item.chainId == chainId && item.level > maxLevel)
                {
                    maxLevel = item.level;
                }
            }
            return maxLevel;
        }
    }
}
