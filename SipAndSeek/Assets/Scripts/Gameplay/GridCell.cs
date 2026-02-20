using UnityEngine;
using SipAndSeek;
using SipAndSeek.Managers;

namespace SipAndSeek.Gameplay
{
    /// <summary>
    /// Represents a single cell in the game grid.
    /// Manages its visual state, item reference, and obstacle data.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class GridCell : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private int _row;
        [SerializeField] private int _col;

        [Header("Visual References")]
        [SerializeField] private SpriteRenderer _backgroundRenderer;
        [SerializeField] private SpriteRenderer _imageRenderer;   // Hidden image tile piece
        [SerializeField] private SpriteRenderer _overlayRenderer;  // Obstacle overlay (lock, ice, etc.)

        // ===============================
        // State
        // ===============================
        private TileState _state = TileState.Empty;
        private MergeItem _currentItem;
        private int _frozenHitsRemaining; // For Frozen tiles: needs 2 hits

        // ===============================
        // Properties
        // ===============================
        public int Row => _row;
        public int Col => _col;
        public TileState State => _state;
        public MergeItem CurrentItem => _currentItem;
        public bool IsEmpty => _state == TileState.Empty && _currentItem == null;
        public bool IsOccupied => _currentItem != null;
        public bool IsRevealed => _state == TileState.Revealed;
        public bool HasObstacle => _state == TileState.Locked || _state == TileState.Frozen ||
                                   _state == TileState.KeyLocked || _state == TileState.Dark;
        public bool IsGolden => _state == TileState.Golden;

        /// <summary>
        /// Sprite for the hidden image tile assigned to this cell.
        /// </summary>
        public Sprite ImageTileSprite { get; set; }

        // ===============================
        // Initialization
        // ===============================

        /// <summary>
        /// Initialize the cell with its grid position.
        /// </summary>
        public void Initialize(int row, int col)
        {
            _row = row;
            _col = col;
            _state = TileState.Empty;
            _currentItem = null;
            _frozenHitsRemaining = 0;
            gameObject.name = $"Cell_{row}_{col}";

            // Auto-find renderers if not assigned
            if (_backgroundRenderer == null)
                _backgroundRenderer = GetComponent<SpriteRenderer>();

            // Ensure collider exists for raycast detection
            BoxCollider2D col2d = GetComponent<BoxCollider2D>();
            if (col2d == null)
                col2d = gameObject.AddComponent<BoxCollider2D>();

            UpdateVisuals();
        }

        // ===============================
        // Item Management
        // ===============================

        /// <summary>
        /// Place a merge item in this cell.
        /// </summary>
        public bool PlaceItem(MergeItem item)
        {
            if (_currentItem != null || HasObstacle)
            {
                return false;
            }

            _currentItem = item;
            _state = TileState.Occupied;

            if (item != null)
            {
                item.SetCell(this);
                item.MoveToPosition(transform.position);
            }

            UpdateVisuals();
            return true;
        }

        /// <summary>
        /// Remove the current item from this cell.
        /// </summary>
        public MergeItem RemoveItem()
        {
            MergeItem item = _currentItem;
            _currentItem = null;
            _state = TileState.Empty;
            UpdateVisuals();
            return item;
        }

        // ===============================
        // Obstacle Management
        // ===============================

        /// <summary>
        /// Set an obstacle on this cell.
        /// </summary>
        public void SetObstacle(TileState obstacleType)
        {
            _state = obstacleType;
            if (obstacleType == TileState.Frozen)
            {
                _frozenHitsRemaining = 2;
            }
            UpdateVisuals();
        }

        /// <summary>
        /// Attempt to unlock this cell's obstacle. Returns true if obstacle was fully removed.
        /// </summary>
        public bool TryUnlockObstacle(int mergeLevel = 0, bool hasKey = false, bool hasLight = false)
        {
            switch (_state)
            {
                case TileState.Locked:
                    if (mergeLevel >= 3)
                    {
                        ClearObstacle();
                        return true;
                    }
                    return false;

                case TileState.Frozen:
                    _frozenHitsRemaining--;
                    if (_frozenHitsRemaining <= 0)
                    {
                        _state = TileState.Locked;
                        UpdateVisuals();
                    }
                    else
                    {
                        UpdateVisuals();
                    }
                    return false;

                case TileState.KeyLocked:
                    if (hasKey)
                    {
                        ClearObstacle();
                        return true;
                    }
                    return false;

                case TileState.Dark:
                    if (hasLight)
                    {
                        ClearObstacle();
                        return true;
                    }
                    return false;

                case TileState.Golden:
                    ClearObstacle();
                    return true;

                default:
                    return false;
            }
        }

        private void ClearObstacle()
        {
            _state = TileState.Empty;
            _frozenHitsRemaining = 0;
            UpdateVisuals();
        }

        // ===============================
        // Reveal
        // ===============================

        /// <summary>
        /// Reveal the hidden image tile on this cell.
        /// </summary>
        public void RevealTile()
        {
            if (_state == TileState.Revealed) return;

            _state = TileState.Revealed;

            // Show the hidden image piece
            if (_imageRenderer != null && ImageTileSprite != null)
            {
                _imageRenderer.sprite = ImageTileSprite;
                _imageRenderer.enabled = true;
            }

            // Remove any existing item
            if (_currentItem != null)
            {
                Destroy(_currentItem.gameObject);
                _currentItem = null;
            }

            UpdateVisuals();
        }

        // ===============================
        // Visuals
        // ===============================
        private void UpdateVisuals()
        {
            UpdateBackgroundColor();
            UpdateOverlay();
        }

        private void UpdateBackgroundColor()
        {
            if (_backgroundRenderer == null) return;

            switch (_state)
            {
                case TileState.Empty:
                    _backgroundRenderer.color = new Color(0.85f, 0.80f, 0.75f, 1f); // Cream beige
                    break;
                case TileState.Occupied:
                    _backgroundRenderer.color = new Color(0.85f, 0.80f, 0.75f, 1f); // Same as empty
                    break;
                case TileState.Revealed:
                    _backgroundRenderer.color = Color.white; // Show image clearly
                    break;
                case TileState.Locked:
                    _backgroundRenderer.color = new Color(0.6f, 0.5f, 0.4f, 1f); // Dark brown
                    break;
                case TileState.Frozen:
                    _backgroundRenderer.color = new Color(0.7f, 0.85f, 0.95f, 1f); // Ice blue
                    break;
                case TileState.KeyLocked:
                    _backgroundRenderer.color = new Color(0.75f, 0.65f, 0.4f, 1f); // Gold-brown
                    break;
                case TileState.Dark:
                    _backgroundRenderer.color = new Color(0.15f, 0.12f, 0.1f, 1f); // Near black
                    break;
                case TileState.Golden:
                    _backgroundRenderer.color = new Color(1f, 0.84f, 0.31f, 1f); // Gold #FFD54F
                    break;
            }
        }

        private void UpdateOverlay()
        {
            if (_overlayRenderer == null) return;
            _overlayRenderer.enabled = HasObstacle || IsGolden;
        }

        /// <summary>
        /// Reset cell color to default (used by MergeItem highlight system).
        /// </summary>
        public void ResetColor()
        {
            UpdateBackgroundColor();
        }

        /// <summary>
        /// Get the world position of this cell (for item positioning).
        /// </summary>
        public Vector3 WorldPosition => transform.position;
    }
}
