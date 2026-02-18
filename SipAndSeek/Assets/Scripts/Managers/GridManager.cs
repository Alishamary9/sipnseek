using UnityEngine;
using System;
using System.Collections.Generic;
using SipAndSeek.Gameplay;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Creates and manages the 2D grid of cells.
    /// Handles cell creation, item placement, and drop validation.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private float _cellSize = 1.0f;
        [SerializeField] private float _cellSpacing = 0.1f;
        [SerializeField] private GameObject _cellPrefab;

        // ===============================
        // State
        // ===============================
        private GridCell[,] _grid;
        private int _rows;
        private int _cols;
        private Transform _gridParent;

        // ===============================
        // Properties
        // ===============================
        public int Rows => _rows;
        public int Cols => _cols;
        public float CellSize => _cellSize;

        // ===============================
        // Events
        // ===============================
        public static event Action<GridCell> OnCellRevealed;
        public static event Action<MergeItem, GridCell> OnItemPlaced;

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
        // Grid Creation
        // ===============================

        /// <summary>
        /// Build the grid based on level configuration.
        /// </summary>
        public void CreateGrid(LevelConfig config)
        {
            ClearGrid();

            _rows = config.gridRows;
            _cols = config.gridCols;
            _grid = new GridCell[_rows, _cols];

            // Create parent container
            _gridParent = new GameObject("Grid").transform;
            _gridParent.SetParent(transform);

            // Calculate offset to center the grid
            float totalWidth = _cols * (_cellSize + _cellSpacing) - _cellSpacing;
            float totalHeight = _rows * (_cellSize + _cellSpacing) - _cellSpacing;
            Vector3 offset = new Vector3(-totalWidth / 2f + _cellSize / 2f,
                                         -totalHeight / 2f + _cellSize / 2f, 0);

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    Vector3 pos = new Vector3(
                        c * (_cellSize + _cellSpacing),
                        r * (_cellSize + _cellSpacing),
                        0) + offset;

                    GridCell cell = CreateCell(r, c, pos);
                    _grid[r, c] = cell;
                }
            }

            Debug.Log($"[GridManager] Grid created: {_rows}x{_cols} ({_rows * _cols} cells)");
        }

        private GridCell CreateCell(int row, int col, Vector3 position)
        {
            GameObject cellObj;

            if (_cellPrefab != null)
            {
                cellObj = Instantiate(_cellPrefab, position, Quaternion.identity, _gridParent);
            }
            else
            {
                // Create default cell if no prefab assigned
                cellObj = new GameObject();
                cellObj.transform.position = position;
                cellObj.transform.SetParent(_gridParent);

                // Add a simple square sprite
                SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.85f, 0.80f, 0.75f, 1f); // Cream beige
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
                sr.sortingOrder = 0;
                cellObj.transform.localScale = Vector3.one * _cellSize * 0.95f;

                // Add collider for drop detection
                BoxCollider2D col2d = cellObj.AddComponent<BoxCollider2D>();
                col2d.size = Vector2.one;
            }

            GridCell cell = cellObj.GetComponent<GridCell>();
            if (cell == null)
            {
                cell = cellObj.AddComponent<GridCell>();
            }
            cell.Initialize(row, col);

            return cell;
        }

        /// <summary>
        /// Destroy the current grid and all cells.
        /// </summary>
        public void ClearGrid()
        {
            if (_gridParent != null)
            {
                Destroy(_gridParent.gameObject);
                _gridParent = null;
            }
            _grid = null;
        }

        // ===============================
        // Cell Access
        // ===============================

        /// <summary>
        /// Get a cell by row and column index.
        /// </summary>
        public GridCell GetCell(int row, int col)
        {
            if (_grid == null || row < 0 || row >= _rows || col < 0 || col >= _cols)
                return null;
            return _grid[row, col];
        }

        /// <summary>
        /// Get the cell closest to a world position.
        /// </summary>
        public GridCell GetCellAtWorldPosition(Vector3 worldPos)
        {
            if (_grid == null) return null;

            float closestDist = float.MaxValue;
            GridCell closest = null;

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    float dist = Vector3.Distance(worldPos, _grid[r, c].WorldPosition);
                    if (dist < closestDist && dist < _cellSize)
                    {
                        closestDist = dist;
                        closest = _grid[r, c];
                    }
                }
            }

            return closest;
        }

        /// <summary>
        /// Get all neighbors of a cell (up, down, left, right).
        /// </summary>
        public List<GridCell> GetNeighbors(GridCell cell)
        {
            List<GridCell> neighbors = new List<GridCell>();
            if (cell == null || _grid == null) return neighbors;

            int r = cell.Row;
            int c = cell.Col;

            if (r > 0) neighbors.Add(_grid[r - 1, c]);          // Down
            if (r < _rows - 1) neighbors.Add(_grid[r + 1, c]);  // Up
            if (c > 0) neighbors.Add(_grid[r, c - 1]);          // Left
            if (c < _cols - 1) neighbors.Add(_grid[r, c + 1]);  // Right

            return neighbors;
        }

        /// <summary>
        /// Get all empty cells in the grid.
        /// </summary>
        public List<GridCell> GetEmptyCells()
        {
            List<GridCell> empty = new List<GridCell>();
            if (_grid == null) return empty;

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    if (_grid[r, c].IsEmpty)
                    {
                        empty.Add(_grid[r, c]);
                    }
                }
            }
            return empty;
        }

        /// <summary>
        /// Get a random empty cell, or null if none exist.
        /// </summary>
        public GridCell GetRandomEmptyCell()
        {
            var empty = GetEmptyCells();
            if (empty.Count == 0) return null;
            return empty[UnityEngine.Random.Range(0, empty.Count)];
        }

        // ===============================
        // Item Drop Handling
        // ===============================

        /// <summary>
        /// Handle an item being dropped on a target cell.
        /// Returns true if the action was handled (merge or move).
        /// </summary>
        public bool HandleItemDrop(MergeItem draggedItem, GridCell targetCell)
        {
            if (draggedItem == null || targetCell == null) return false;

            // Check if target cell has an item — try merge
            if (targetCell.IsOccupied && targetCell.CurrentItem != null)
            {
                MergeItem targetItem = targetCell.CurrentItem;

                if (draggedItem.CanMergeWith(targetItem))
                {
                    // Delegate to MergeManager
                    if (MergeManager.Instance != null)
                    {
                        MergeManager.Instance.PerformMerge(draggedItem, targetItem, targetCell);
                        return true;
                    }
                }

                // Cannot merge — return to original position
                draggedItem.ReturnToStartPosition();
                return false;
            }

            // Target cell is empty — move item there
            if (targetCell.IsEmpty)
            {
                GridCell sourceCell = draggedItem.CurrentCell;
                if (sourceCell != null)
                {
                    sourceCell.RemoveItem();
                }

                targetCell.PlaceItem(draggedItem);
                OnItemPlaced?.Invoke(draggedItem, targetCell);
                return true;
            }

            draggedItem.ReturnToStartPosition();
            return false;
        }

        // ===============================
        // Reveal
        // ===============================

        /// <summary>
        /// Reveal a specific cell (called by RevealManager after merge threshold).
        /// </summary>
        public void RevealCell(int row, int col)
        {
            GridCell cell = GetCell(row, col);
            if (cell != null && !cell.IsRevealed)
            {
                cell.RevealTile();
                OnCellRevealed?.Invoke(cell);
            }
        }

        /// <summary>
        /// Count how many cells have been revealed.
        /// </summary>
        public int CountRevealedCells()
        {
            int count = 0;
            if (_grid == null) return 0;

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    if (_grid[r, c].IsRevealed) count++;
                }
            }
            return count;
        }
    }
}
