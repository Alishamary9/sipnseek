using UnityEngine;
using System;
using SipAndSeek.Data;
using SipAndSeek.Gameplay;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Manages powerup activation during gameplay.
    /// Handles inventory checks, purchases, and applying effects to the grid.
    /// Powerup types: Hint, Shuffle, Hammer, FogClearer, Bomb.
    /// </summary>
    public class PowerupManager : MonoBehaviour
    {
        public static PowerupManager Instance { get; private set; }

        // ===============================
        // Constants — Powerup IDs (match PowerupData.powerupId)
        // ===============================
        public const string HINT = "hint";
        public const string SHUFFLE = "shuffle";
        public const string HAMMER = "hammer";
        public const string FOG_CLEARER = "fog_clearer";
        public const string BOMB = "bomb";

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when a powerup is successfully activated (id, remaining count).</summary>
        public static event Action<string, int> OnPowerupActivated;

        /// <summary>Fired when a powerup is purchased.</summary>
        public static event Action<string> OnPowerupPurchased;

        /// <summary>Fired when activation fails (not enough stock, wrong state, etc).</summary>
        public static event Action<string, string> OnPowerupFailed; // id, reason

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[PowerupManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ===============================
        // Activation
        // ===============================

        /// <summary>
        /// Attempt to activate a powerup by ID. Consumes one from inventory.
        /// Only works during Playing state.
        /// </summary>
        public bool ActivatePowerup(string powerupId)
        {
            // Validate game state
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            {
                Debug.LogWarning("[PowerupManager] Cannot use powerups outside of Playing state.");
                OnPowerupFailed?.Invoke(powerupId, "Not playing");
                return false;
            }

            // Check inventory
            if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.GetPowerupCount(powerupId) <= 0)
            {
                Debug.LogWarning($"[PowerupManager] No {powerupId} in inventory.");
                OnPowerupFailed?.Invoke(powerupId, "No stock");
                return false;
            }

            // Apply effect
            bool success = ApplyEffect(powerupId);
            if (!success)
            {
                OnPowerupFailed?.Invoke(powerupId, "Effect failed");
                return false;
            }

            // Consume from inventory
            PlayerDataManager.Instance.UsePowerup(powerupId);
            int remaining = PlayerDataManager.Instance.GetPowerupCount(powerupId);

            Debug.Log($"[PowerupManager] ✨ Activated {powerupId} — {remaining} remaining");
            OnPowerupActivated?.Invoke(powerupId, remaining);
            return true;
        }

        /// <summary>
        /// Apply the powerup effect to the current game state.
        /// </summary>
        private bool ApplyEffect(string powerupId)
        {
            switch (powerupId)
            {
                case HINT:
                    return ApplyHint();
                case SHUFFLE:
                    return ApplyShuffle();
                case HAMMER:
                    return ApplyHammer();
                case FOG_CLEARER:
                    return ApplyFogClearer();
                case BOMB:
                    return ApplyBomb();
                default:
                    Debug.LogWarning($"[PowerupManager] Unknown powerup: {powerupId}");
                    return false;
            }
        }

        // ===============================
        // Effect Implementations
        // ===============================

        /// <summary>Hint: Reveal one random unrevealed tile.</summary>
        private bool ApplyHint()
        {
            if (RevealManager.Instance == null) return false;

            int before = RevealManager.Instance.RevealedCount;
            RevealManager.Instance.RevealNextTile();
            bool revealed = RevealManager.Instance.RevealedCount > before;

            if (revealed)
                Debug.Log("[PowerupManager] 💡 Hint — Revealed a tile");

            return revealed;
        }

        /// <summary>Shuffle: Rearrange all items on the grid randomly.</summary>
        private bool ApplyShuffle()
        {
            if (GridManager.Instance == null) return false;

            var emptyCells = GridManager.Instance.GetEmptyCells();
            // Collect all items, then redistribute
            var allItems = new System.Collections.Generic.List<MergeItem>();

            for (int r = 0; r < GridManager.Instance.Rows; r++)
            {
                for (int c = 0; c < GridManager.Instance.Cols; c++)
                {
                    GridCell cell = GridManager.Instance.GetCell(r, c);
                    if (cell != null && cell.CurrentItem != null)
                    {
                        allItems.Add(cell.CurrentItem);
                        cell.RemoveItem();
                    }
                }
            }

            if (allItems.Count == 0) return false;

            // Shuffle items
            for (int i = allItems.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                var temp = allItems[i];
                allItems[i] = allItems[j];
                allItems[j] = temp;
            }

            // Redistribute to any available cell
            var availableCells = new System.Collections.Generic.List<GridCell>();
            for (int r = 0; r < GridManager.Instance.Rows; r++)
            {
                for (int c = 0; c < GridManager.Instance.Cols; c++)
                {
                    GridCell cell = GridManager.Instance.GetCell(r, c);
                    if (cell != null && cell.CurrentItem == null &&
                        cell.State != TileState.Locked && cell.State != TileState.Frozen)
                    {
                        availableCells.Add(cell);
                    }
                }
            }

            int placed = Mathf.Min(allItems.Count, availableCells.Count);
            for (int i = 0; i < placed; i++)
            {
                availableCells[i].PlaceItem(allItems[i]);
            }

            Debug.Log($"[PowerupManager] 🔀 Shuffle — Redistributed {placed} items");
            return true;
        }

        /// <summary>Hammer: Remove one random item from the grid.</summary>
        private bool ApplyHammer()
        {
            if (GridManager.Instance == null) return false;

            // Find a random occupied cell
            for (int r = 0; r < GridManager.Instance.Rows; r++)
            {
                for (int c = 0; c < GridManager.Instance.Cols; c++)
                {
                    GridCell cell = GridManager.Instance.GetCell(r, c);
                    if (cell != null && cell.CurrentItem != null)
                    {
                        MergeItem item = cell.CurrentItem;
                        cell.RemoveItem();
                        Destroy(item.gameObject);
                        Debug.Log($"[PowerupManager] 🔨 Hammer — Removed item at ({r},{c})");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>FogClearer: Reveal a 3x3 area of tiles.</summary>
        private bool ApplyFogClearer()
        {
            if (RevealManager.Instance == null) return false;

            int before = RevealManager.Instance.RevealedCount;
            RevealManager.Instance.RevealMultipleTiles(9);
            int revealed = RevealManager.Instance.RevealedCount - before;

            Debug.Log($"[PowerupManager] 🌫️ Fog Clearer — Revealed {revealed} tiles");
            return revealed > 0;
        }

        /// <summary>Bomb: Clear one random obstacle from the grid.</summary>
        private bool ApplyBomb()
        {
            if (ObstacleManager.Instance == null || GridManager.Instance == null) return false;

            // Find a random cell with an obstacle and force-clear it
            for (int r = 0; r < GridManager.Instance.Rows; r++)
            {
                for (int c = 0; c < GridManager.Instance.Cols; c++)
                {
                    GridCell cell = GridManager.Instance.GetCell(r, c);
                    if (cell != null && cell.HasObstacle)
                    {
                        // Force unlock with max level + all keys
                        bool cleared = ObstacleManager.Instance.TryUnlockObstacle(cell, 99, true, true);
                        if (cleared)
                        {
                            Debug.Log($"[PowerupManager] 💣 Bomb — Cleared obstacle at ({r},{c})");
                            return true;
                        }
                    }
                }
            }

            Debug.Log("[PowerupManager] 💣 Bomb — No obstacles to clear");
            return false;
        }

        // ===============================
        // Purchase
        // ===============================

        /// <summary>
        /// Purchase a powerup using coins. Adds to inventory.
        /// </summary>
        public bool BuyPowerup(string powerupId)
        {
            if (GameDatabase.Instance == null || EconomyManager.Instance == null) return false;

            PowerupData data = GameDatabase.Instance.GetPowerup(powerupId);
            if (data == null)
            {
                Debug.LogWarning($"[PowerupManager] Unknown powerup: {powerupId}");
                return false;
            }

            // Check max hold
            int current = PlayerDataManager.Instance != null
                ? PlayerDataManager.Instance.GetPowerupCount(powerupId)
                : 0;

            if (data.maxHold > 0 && current >= data.maxHold)
            {
                Debug.Log($"[PowerupManager] Already at max hold ({data.maxHold}) for {powerupId}");
                OnPowerupFailed?.Invoke(powerupId, "Max hold reached");
                return false;
            }

            // Try purchase with coins first, fallback to gems
            bool purchased = false;
            if (data.coinPrice > 0)
            {
                purchased = EconomyManager.Instance.PurchaseWithCoins(data.coinPrice, data.nameEN);
            }
            else if (data.gemPrice > 0)
            {
                purchased = EconomyManager.Instance.PurchaseWithGems(data.gemPrice, data.nameEN);
            }

            if (purchased)
            {
                PlayerDataManager.Instance?.AddPowerup(powerupId);
                OnPowerupPurchased?.Invoke(powerupId);
                Debug.Log($"[PowerupManager] 🛒 Purchased {powerupId}");
            }

            return purchased;
        }

        /// <summary>
        /// Get the count of a powerup in inventory.
        /// </summary>
        public int GetCount(string powerupId)
        {
            return PlayerDataManager.Instance != null
                ? PlayerDataManager.Instance.GetPowerupCount(powerupId)
                : 0;
        }
    }
}
