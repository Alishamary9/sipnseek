using UnityEngine;
using SipAndSeek;
using SipAndSeek.Data;
using SipAndSeek.Managers;
using TMPro;

namespace SipAndSeek.Gameplay
{
    /// <summary>
    /// A draggable merge item on the grid.
    /// Uses camera-based input for reliable drag & drop on 2D sprites.
    /// Compatible with both mouse and touch input.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class MergeItem : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string _chainId;
        [SerializeField] private int _level;

        [Header("Drag Settings")]
        [SerializeField] private float _dragScale = 1.15f;
        [SerializeField] private float _dragAlpha = 0.8f;
        [SerializeField] private int _dragSortOrder = 100;
        [SerializeField] private float _snapSpeed = 15f;

        // ===============================
        // References
        // ===============================
        private MergeChainItemData _data;
        private GridCell _currentCell;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;

        // ===============================
        // Drag State
        // ===============================
        private bool _isDragging;
        private Vector3 _startPosition;
        private GridCell _startCell;
        private int _originalSortingOrder;
        private Vector3 _dragOffset;
        private bool _isSnapping;
        private Vector3 _snapTarget;

        // ===============================
        // Properties
        // ===============================
        public MergeChainItemData Data => _data;
        public GridCell CurrentCell => _currentCell;
        public string ChainId => _data != null ? _data.chainId : _chainId;
        public int Level => _data != null ? _data.level : _level;
        public bool IsDragging => _isDragging;

        // ===============================
        // Initialization
        // ===============================

        /// <summary>
        /// Initialize this merge item with its data.
        /// </summary>
        public void Initialize(MergeChainItemData data)
        {
            _data = data;
            _chainId = data.chainId;
            _level = data.level;

            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_collider == null)
                _collider = GetComponent<BoxCollider2D>();

            UpdateVisual();
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();

            // Ensure collider is appropriately sized
            if (_collider != null && _collider.size == Vector2.one)
            {
                _collider.size = Vector2.one * 0.9f;
            }
        }

        /// <summary>
        /// Set the cell this item belongs to.
        /// </summary>
        public void SetCell(GridCell cell)
        {
            _currentCell = cell;
        }

        // ===============================
        // Visual
        // ===============================
        private void UpdateVisual()
        {
            if (_spriteRenderer == null) return;

            if (_data != null)
            {
                if (_data.icon != null)
                {
                    _spriteRenderer.sprite = _data.icon;
                    _spriteRenderer.color = Color.white;
                    ClearFallbackText();
                    
                    // Fit sprite to 1x1 cell size
                    float targetSize = 0.9f; // slightly smaller than cell for padding
                    float maxSpriteDimension = Mathf.Max(_spriteRenderer.sprite.bounds.size.x, _spriteRenderer.sprite.bounds.size.y);
                    if (maxSpriteDimension > 0)
                    {
                        float scaleFactor = targetSize / maxSpriteDimension;
                        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

                        // Fix collider bounds so it remains 0.8x0.8 in world space, preserving clickability
                        if (_collider != null)
                        {
                            float inverseScale = 1f / scaleFactor;
                            _collider.size = new Vector2(0.8f * inverseScale, 0.8f * inverseScale);
                        }
                    }
                }
                else
                {
                    _spriteRenderer.color = GetRarityColor(_data.rarity);
                    CreateFallbackText();
                }

                // Set name for debugging
                string itemName = LocalizationManager.Instance != null &&
                    LocalizationManager.Instance.CurrentLanguage == LocalizationManager.Language.Arabic
                    ? _data.itemNameAR
                    : _data.itemNameEN;
                gameObject.name = $"Item_{_data.chainId}_Lv{_data.level}_{itemName}";
            }
        }

        private TextMeshPro _fallbackText;

        private void CreateFallbackText()
        {
            if (_fallbackText != null) return;

            GameObject textObj = new GameObject("FallbackText");
            textObj.transform.SetParent(transform, false);
            textObj.transform.localPosition = new Vector3(0, 0, -0.1f);

            _fallbackText = textObj.AddComponent<TextMeshPro>();
            _fallbackText.text = $"Lv{_level}\n{_chainId}";
            _fallbackText.fontSize = 2.5f;
            _fallbackText.alignment = TextAlignmentOptions.Center;
            _fallbackText.color = Color.black;
            _fallbackText.sortingOrder = _spriteRenderer.sortingOrder + 1;
            
            // Make sure it fits in the square
            RectTransform rt = _fallbackText.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1f, 1f);
        }

        private void ClearFallbackText()
        {
            if (_fallbackText != null)
            {
                Destroy(_fallbackText.gameObject);
                _fallbackText = null;
            }
        }

        private Color GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:    return new Color(0.75f, 0.75f, 0.75f); // Light gray
                case ItemRarity.Uncommon:  return new Color(0.3f, 0.8f, 0.3f);    // Green
                case ItemRarity.Rare:      return new Color(0.3f, 0.5f, 0.9f);    // Blue
                case ItemRarity.Epic:      return new Color(0.6f, 0.3f, 0.9f);    // Purple
                case ItemRarity.Legendary: return new Color(1f, 0.84f, 0.0f);     // Gold
                default:                   return Color.white;
            }
        }

        // ===============================
        // Input Handling (Camera-Based)
        // ===============================

        private void OnMouseDown()
        {
            if (!CanInteract()) return;

            // Calculate drag offset so item doesn't jump to cursor center
            Vector3 mouseWorld = GetMouseWorldPosition();
            _dragOffset = transform.position - mouseWorld;

            BeginDrag();
        }

        private void OnMouseDrag()
        {
            if (!_isDragging) return;

            Vector3 mouseWorld = GetMouseWorldPosition();
            Vector3 targetPos = mouseWorld + _dragOffset;
            targetPos.z = -1f; // Keep item above grid
            transform.position = targetPos;

            // Highlight the cell under the cursor
            HighlightTargetCell(mouseWorld);
        }

        private void OnMouseUp()
        {
            if (!_isDragging) return;

            EndDrag();
        }

        // ===============================
        // Drag Logic
        // ===============================

        private Vector3 _originalScale = Vector3.one;

        private void BeginDrag()
        {
            _isDragging = true;
            _isSnapping = false;
            _startPosition = transform.position;
            _startCell = _currentCell;

            // Bring to front while dragging
            if (_spriteRenderer != null)
            {
                _originalSortingOrder = _spriteRenderer.sortingOrder;
                _spriteRenderer.sortingOrder = _dragSortOrder;
            }

            // Scale up for tactile feedback, based on current local scale
            _originalScale = transform.localScale;
            transform.localScale = _originalScale * _dragScale;

            // Semi-transparent
            SetAlpha(_dragAlpha);

            Debug.Log($"[MergeItem] 🖐️ Started dragging {gameObject.name}");
        }

        private void EndDrag()
        {
            _isDragging = false;

            // Reset visual feedback
            transform.localScale = _originalScale;
            SetAlpha(1f);

            if (_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = _originalSortingOrder;
            }

            // Find the cell under the drop position
            Vector3 mouseWorld = GetMouseWorldPosition();
            GridCell targetCell = FindTargetCell(mouseWorld);

            if (targetCell != null && targetCell != _startCell)
            {
                // Attempt merge or move via GridManager
                if (GridManager.Instance != null)
                {
                    bool handled = GridManager.Instance.HandleItemDrop(this, targetCell);
                    if (handled)
                    {
                        Debug.Log($"[MergeItem] ✅ Drop handled on Cell_{targetCell.Row}_{targetCell.Col}");
                        ClearHighlight();
                        return;
                    }
                }
            }

            // Drop failed — snap back
            Debug.Log($"[MergeItem] ↩️ Returning to start position");
            SnapToPosition(_startPosition);
            ClearHighlight();
        }

        // ===============================
        // Cell Finding
        // ===============================

        /// <summary>
        /// Find the target cell using both raycast and distance fallback.
        /// </summary>
        private GridCell FindTargetCell(Vector3 worldPos)
        {
            // Method 1: Raycast for precise hit detection
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
            foreach (var hit in hits)
            {
                GridCell cell = hit.collider.GetComponent<GridCell>();
                if (cell != null) return cell;
            }

            // Method 2: Fallback to nearest cell by distance
            if (GridManager.Instance != null)
            {
                return GridManager.Instance.GetCellAtWorldPosition(worldPos);
            }

            return null;
        }

        // ===============================
        // Cell Highlighting
        // ===============================
        private GridCell _highlightedCell;

        private void HighlightTargetCell(Vector3 worldPos)
        {
            GridCell cell = FindTargetCell(worldPos);

            if (cell != _highlightedCell)
            {
                ClearHighlight();
                _highlightedCell = cell;

                // Show visual feedback on potential drop target
                if (_highlightedCell != null && _highlightedCell != _startCell)
                {
                    SpriteRenderer cellRenderer = _highlightedCell.GetComponent<SpriteRenderer>();
                    if (cellRenderer != null)
                    {
                        // Green if can merge, blue if empty, red if blocked
                        if (_highlightedCell.IsOccupied && _highlightedCell.CurrentItem != null
                            && CanMergeWith(_highlightedCell.CurrentItem))
                        {
                            cellRenderer.color = new Color(0.5f, 0.9f, 0.5f, 1f); // Green — can merge
                        }
                        else if (_highlightedCell.IsEmpty)
                        {
                            cellRenderer.color = new Color(0.5f, 0.7f, 0.9f, 1f); // Blue — can place
                        }
                        else
                        {
                            cellRenderer.color = new Color(0.9f, 0.5f, 0.5f, 1f); // Red — blocked
                        }
                    }
                }
            }
        }

        private void ClearHighlight()
        {
            if (_highlightedCell != null)
            {
                // Reset cell color
                SpriteRenderer cellRenderer = _highlightedCell.GetComponent<SpriteRenderer>();
                if (cellRenderer != null)
                {
                    cellRenderer.color = new Color(0.85f, 0.80f, 0.75f, 1f); // Default cream
                }
                _highlightedCell = null;
            }
        }

        // ===============================
        // Movement
        // ===============================

        /// <summary>
        /// Snap back to a position with smooth animation.
        /// </summary>
        public void ReturnToStartPosition()
        {
            SnapToPosition(_startPosition);
        }

        /// <summary>
        /// Move smoothly to a target position.
        /// </summary>
        public void MoveToPosition(Vector3 targetPos)
        {
            targetPos.z = -1f;
            SnapToPosition(targetPos);
        }

        private void SnapToPosition(Vector3 target)
        {
            _snapTarget = target;
            _isSnapping = true;
        }

        private void Update()
        {
            // Smooth snap animation
            if (_isSnapping)
            {
                transform.position = Vector3.Lerp(transform.position, _snapTarget, Time.deltaTime * _snapSpeed);
                if (Vector3.Distance(transform.position, _snapTarget) < 0.01f)
                {
                    transform.position = _snapTarget;
                    _isSnapping = false;
                }
            }
        }

        // ===============================
        // Merge Check
        // ===============================

        /// <summary>
        /// Check if this item can merge with another item.
        /// Same chain, same level, and next level exists.
        /// </summary>
        public bool CanMergeWith(MergeItem other)
        {
            if (other == null || _data == null || other._data == null) return false;
            return _data.chainId == other._data.chainId &&
                   _data.level == other._data.level;
        }

        // ===============================
        // Helpers
        // ===============================

        private bool CanInteract()
        {
            // Don't allow interaction during pause or non-playing states
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState != GameState.Playing)
            {
                return false;
            }
            return true;
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        private void SetAlpha(float alpha)
        {
            if (_spriteRenderer != null)
            {
                Color c = _spriteRenderer.color;
                _spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            }
        }
    }
}
