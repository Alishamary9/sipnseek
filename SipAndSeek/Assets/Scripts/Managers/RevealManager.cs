using UnityEngine;
using System;
using System.Collections.Generic;
using SipAndSeek.Gameplay;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Manages the hidden image reveal system.
    /// Tracks which tiles are revealed and triggers reveals when merge threshold is met.
    /// </summary>
    public class RevealManager : MonoBehaviour
    {
        public static RevealManager Instance { get; private set; }

        // ===============================
        // State
        // ===============================
        private List<Vector2Int> _imagePositions = new List<Vector2Int>(); // Grid positions for image tiles
        private List<Vector2Int> _unrevealed = new List<Vector2Int>();     // Not yet revealed
        private List<Vector2Int> _revealed = new List<Vector2Int>();       // Already revealed
        private Sprite _fullImage;   // The complete hidden image
        private Sprite[] _imageTiles; // Sliced tiles of the image

        private int _imageRows;
        private int _imageCols;
        private int _gridRows;
        private int _gridCols;

        // ===============================
        // Properties
        // ===============================
        public int TotalImageTiles => _imagePositions.Count;
        public int RevealedCount => _revealed.Count;
        public int RemainingCount => _unrevealed.Count;
        public float RevealProgress => TotalImageTiles > 0 ? (float)RevealedCount / TotalImageTiles : 0f;

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when a tile is revealed (row, col, progress%).</summary>
        public static event Action<int, int, float> OnTileRevealed;

        /// <summary>Fired when all tiles are revealed — image fully uncovered.</summary>
        public static event Action OnImageFullyRevealed;

        /// <summary>Fired when the target percentage is reached — level can complete.</summary>
        public static event Action<float> OnTargetReached;

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
        /// Set up the reveal system for a level.
        /// </summary>
        public void Initialize(LevelConfig config)
        {
            _imageRows = config.imageGridRows;
            _imageCols = config.imageGridCols;
            _gridRows = config.gridRows;
            _gridCols = config.gridCols;
            _fullImage = config.hiddenImage;

            _imagePositions.Clear();
            _unrevealed.Clear();
            _revealed.Clear();

            // Calculate image placement within the grid (centered)
            int startRow = (_gridRows - _imageRows) / 2;
            int startCol = (_gridCols - _imageCols) / 2;

            for (int r = 0; r < _imageRows; r++)
            {
                for (int c = 0; c < _imageCols; c++)
                {
                    Vector2Int gridPos = new Vector2Int(startRow + r, startCol + c);
                    _imagePositions.Add(gridPos);
                    _unrevealed.Add(gridPos);
                }
            }

            // Slice the image into tiles if available
            if (_fullImage != null)
            {
                SliceImage(_fullImage);
            }

            // Assign image tiles to grid cells
            AssignImageTilesToCells();

            Debug.Log($"[RevealManager] Initialized: {_imageRows}x{_imageCols} image " +
                      $"({TotalImageTiles} tiles) centered in {_gridRows}x{_gridCols} grid");
        }

        // ===============================
        // Image Slicing
        // ===============================

        private void SliceImage(Sprite image)
        {
            if (image == null || image.texture == null) return;

            Texture2D tex = image.texture;
            _imageTiles = new Sprite[_imageRows * _imageCols];

            int tileWidth = tex.width / _imageCols;
            int tileHeight = tex.height / _imageRows;

            for (int r = 0; r < _imageRows; r++)
            {
                for (int c = 0; c < _imageCols; c++)
                {
                    // Unity textures have origin at bottom-left, so flip row
                    int texRow = _imageRows - 1 - r;
                    Rect rect = new Rect(c * tileWidth, texRow * tileHeight, tileWidth, tileHeight);
                    Sprite tile = Sprite.Create(tex, rect, Vector2.one * 0.5f, Mathf.Max(tileWidth, tileHeight));
                    _imageTiles[r * _imageCols + c] = tile;
                }
            }
        }

        private void AssignImageTilesToCells()
        {
            if (GridManager.Instance == null) return;

            for (int i = 0; i < _imagePositions.Count; i++)
            {
                Vector2Int pos = _imagePositions[i];
                GridCell cell = GridManager.Instance.GetCell(pos.x, pos.y);
                if (cell != null && _imageTiles != null && i < _imageTiles.Length)
                {
                    cell.ImageTileSprite = _imageTiles[i];
                }
            }
        }

        // ===============================
        // Reveal Logic
        // ===============================

        /// <summary>
        /// Reveal the next unrevealed tile (random selection).
        /// Called by MergeManager when merge threshold is reached.
        /// </summary>
        public void RevealNextTile()
        {
            if (_unrevealed.Count == 0)
            {
                Debug.Log("[RevealManager] All tiles already revealed!");
                return;
            }

            // Random unrevealed tile
            int index = UnityEngine.Random.Range(0, _unrevealed.Count);
            Vector2Int pos = _unrevealed[index];

            RevealTileAt(pos);
        }

        /// <summary>
        /// Reveal a specific tile at grid position.
        /// </summary>
        public void RevealTileAt(Vector2Int gridPos)
        {
            if (!_unrevealed.Contains(gridPos)) return;

            _unrevealed.Remove(gridPos);
            _revealed.Add(gridPos);

            // Update grid cell
            if (GridManager.Instance != null)
            {
                GridManager.Instance.RevealCell(gridPos.x, gridPos.y);
            }

            float progress = RevealProgress;
            Debug.Log($"[RevealManager] 🖼️ Revealed tile at ({gridPos.x},{gridPos.y}) — " +
                      $"{RevealedCount}/{TotalImageTiles} ({progress * 100:F0}%)");

            OnTileRevealed?.Invoke(gridPos.x, gridPos.y, progress);

            // Update player stats
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.AddTilesRevealed(1);
            }

            // Play reveal SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("sfx_reveal");
            }

            // Check if target percentage reached
            LevelConfig currentConfig = null;
            LevelManager levelMgr = FindFirstObjectByType<LevelManager>();
            if (levelMgr != null)
            {
                currentConfig = levelMgr.CurrentConfig;
            }

            float targetPercent = currentConfig != null ? currentConfig.targetPercent : 0.8f;
            if (progress >= targetPercent && _revealed.Count > 0)
            {
                OnTargetReached?.Invoke(progress);
            }

            // Check if fully revealed
            if (_unrevealed.Count == 0)
            {
                Debug.Log("[RevealManager] 🎉 Image fully revealed!");
                OnImageFullyRevealed?.Invoke();
            }
        }

        /// <summary>
        /// Reveal multiple tiles at once (e.g., from a powerup).
        /// </summary>
        public void RevealMultipleTiles(int count)
        {
            for (int i = 0; i < count && _unrevealed.Count > 0; i++)
            {
                RevealNextTile();
            }
        }

        // ===============================
        // Queries
        // ===============================

        /// <summary>
        /// Check if a grid position is part of the hidden image.
        /// </summary>
        public bool IsImagePosition(int row, int col)
        {
            return _imagePositions.Contains(new Vector2Int(row, col));
        }

        /// <summary>
        /// Check if a grid position has been revealed.
        /// </summary>
        public bool IsRevealed(int row, int col)
        {
            return _revealed.Contains(new Vector2Int(row, col));
        }

        /// <summary>
        /// Calculate star rating based on reveal percentage and moves used.
        /// 3 Stars: 100% revealed, within move limit
        /// 2 Stars: 90%+ revealed
        /// 1 Star: Target% reached
        /// </summary>
        public int CalculateStars(float targetPercent)
        {
            float progress = RevealProgress;

            if (progress >= 1f)
                return 3;
            else if (progress >= 0.9f)
                return 2;
            else if (progress >= targetPercent)
                return 1;
            else
                return 0;
        }

    }
}
