using UnityEngine;
using System.Collections.Generic;
using SipAndSeek.Data;
using SipAndSeek.Managers;

namespace SipAndSeek.Gameplay
{
    /// <summary>
    /// Generates new merge items based on available chains and spawn weights.
    /// Placed at the edge of the grid, produces Level 1 items on cooldown.
    /// </summary>
    public class ItemGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _cooldownTime = 3f;
        [SerializeField] private float _skipCooldownCost = 50f; // Coins to skip cooldown

        // ===============================
        // State
        // ===============================
        private float _cooldownRemaining;
        private bool _isReady = true;
        private List<string> _availableChains = new List<string>();
        private GridCell _outputCell; // Cell where new items appear

        // ===============================
        // Properties
        // ===============================
        public bool IsReady => _isReady;
        public float CooldownRemaining => _cooldownRemaining;
        public float CooldownPercent => _cooldownTime > 0 ? _cooldownRemaining / _cooldownTime : 0f;

        // ===============================
        // Initialization
        // ===============================

        /// <summary>
        /// Set up the generator with available chains for this level.
        /// </summary>
        public void Initialize(List<string> availableChains, GridCell outputCell)
        {
            _availableChains = availableChains ?? new List<string>();
            _outputCell = outputCell;
            _cooldownRemaining = 0f;
            _isReady = true;
        }

        // ===============================
        // Update Loop
        // ===============================
        private void Update()
        {
            if (_isReady) return;

            _cooldownRemaining -= Time.deltaTime;
            if (_cooldownRemaining <= 0f)
            {
                _cooldownRemaining = 0f;
                _isReady = true;
            }
        }

        // ===============================
        // Item Generation
        // ===============================

        /// <summary>
        /// Generate a new Level 1 item based on spawn weights.
        /// Returns the selected MergeChainItemData, or null if not ready.
        /// </summary>
        public MergeChainItemData GenerateItemData()
        {
            if (!_isReady || _availableChains.Count == 0) return null;

            // Get all Level 1 items from available chains
            var database = GameDatabase.Instance;
            if (database == null) return null;

            List<MergeChainItemData> candidates = new List<MergeChainItemData>();
            List<float> weights = new List<float>();

            foreach (var item in database.mergeChainItems)
            {
                if (item.level == 1 && _availableChains.Contains(item.chainId) && item.spawnWeight > 0)
                {
                    candidates.Add(item);
                    weights.Add(item.spawnWeight);
                }
            }

            if (candidates.Count == 0) return null;

            // Weighted random selection
            MergeChainItemData selected = WeightedRandomSelect(candidates, weights);

            // Start cooldown
            _isReady = false;
            _cooldownRemaining = _cooldownTime;

            return selected;
        }

        /// <summary>
        /// Generate and spawn a new item on the output cell.
        /// Returns the spawned MergeItem, or null if failed.
        /// </summary>
        public MergeItem GenerateAndSpawn()
        {
            MergeChainItemData data = GenerateItemData();
            if (data == null) return null;

            if (_outputCell != null && _outputCell.IsEmpty)
            {
                MergeItem item = CreateMergeItemObject(data, _outputCell.WorldPosition);
                _outputCell.PlaceItem(item);
                return item;
            }

            return null;
        }

        /// <summary>
        /// Skip the cooldown by spending coins.
        /// </summary>
        public bool SkipCooldown()
        {
            if (_isReady) return false; // Already ready

            var playerData = PlayerDataManager.Instance;
            if (playerData == null) return false;

            if (playerData.SpendCoins((int)_skipCooldownCost))
            {
                _cooldownRemaining = 0f;
                _isReady = true;
                Debug.Log("[ItemGenerator] Cooldown skipped!");
                return true;
            }

            return false;
        }

        // ===============================
        // Helpers
        // ===============================

        private MergeChainItemData WeightedRandomSelect(List<MergeChainItemData> items, List<float> weights)
        {
            float totalWeight = 0f;
            foreach (float w in weights) totalWeight += w;

            float random = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < items.Count; i++)
            {
                cumulative += weights[i];
                if (random <= cumulative)
                {
                    return items[i];
                }
            }

            return items[items.Count - 1]; // Fallback
        }

        /// <summary>
        /// Generates an item and spawns it on the grid.
        /// Finds a random empty cell if one isn't specified.
        /// </summary>
        public MergeItem GenerateAndSpawn(GridCell targetCell = null)
        {
            MergeChainItemData data = GenerateItemData();
            if (data == null) return null;

            if (targetCell == null)
            {
                if (GridManager.Instance == null) return null;
                targetCell = GridManager.Instance.GetRandomEmptyCell();
            }

            if (targetCell == null) return null; // No empty space

            // Create item object at the cell's position
            MergeItem item = CreateMergeItemObject(data, targetCell.WorldPosition);
            
            // Place logical reference on cell
            targetCell.PlaceItem(item);
            
            return item;
        }

        /// <summary>
        /// Create a MergeItem GameObject in the scene.
        /// </summary>
        public static MergeItem CreateMergeItemObject(MergeChainItemData data, Vector3 position)
        {
            GameObject obj = new GameObject($"Item_{data.chainId}_Lv{data.level}");
            obj.transform.position = position;

            // Add SpriteRenderer (required by MergeItem)
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10;

            // Create a simple colored square sprite as placeholder
            Sprite placeholder = CreatePlaceholderSprite();
            sr.sprite = placeholder;

            // Add BoxCollider2D for raycasting/drag detection
            BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
            col.size = Vector2.one * 0.8f;

            // Add MergeItem component
            MergeItem item = obj.AddComponent<MergeItem>();
            item.Initialize(data);

            return item;
        }

        private static Sprite CreatePlaceholderSprite()
        {
            Texture2D tex = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), Vector2.one * 0.5f, 64f);
        }
    }
}
