using UnityEngine;
using UnityEngine.EventSystems;
using SipAndSeek;
using SipAndSeek.Data;
using SipAndSeek.Managers;

namespace SipAndSeek.Gameplay
{
    /// <summary>
    /// A draggable merge item on the grid.
    /// Handles drag & drop input and visual feedback.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MergeItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [Header("Data")]
        [SerializeField] private string _chainId;
        [SerializeField] private int _level;

        // ===============================
        // References
        // ===============================
        private MergeChainItemData _data;
        private GridCell _currentCell;
        private SpriteRenderer _spriteRenderer;
        private Canvas _canvas;

        // ===============================
        // Drag State
        // ===============================
        private Vector3 _startPosition;
        private GridCell _startCell;
        private bool _isDragging;
        private int _originalSortingOrder;

        // ===============================
        // Properties
        // ===============================
        public MergeChainItemData Data => _data;
        public GridCell CurrentCell => _currentCell;
        public string ChainId => _data != null ? _data.chainId : _chainId;
        public int Level => _data != null ? _data.level : _level;

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
            _spriteRenderer = GetComponent<SpriteRenderer>();

            UpdateVisual();
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
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

            // Set color based on rarity for now (sprites will be added later)
            if (_data != null)
            {
                _spriteRenderer.color = GetRarityColor(_data.rarity);

                // Set name for debugging
                string itemName = LocalizationManager.Instance != null &&
                    LocalizationManager.Instance.CurrentLanguage == LocalizationManager.Language.Arabic
                    ? _data.itemNameAR
                    : _data.itemNameEN;
                gameObject.name = $"Item_{_data.chainId}_Lv{_data.level}_{itemName}";
            }
        }

        private Color GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:    return new Color(0.75f, 0.75f, 0.75f); // Light gray
                case ItemRarity.Uncommon:  return new Color(0.3f, 0.8f, 0.3f);   // Green
                case ItemRarity.Rare:      return new Color(0.3f, 0.5f, 0.9f);   // Blue
                case ItemRarity.Epic:      return new Color(0.6f, 0.3f, 0.9f);   // Purple
                case ItemRarity.Legendary: return new Color(1f, 0.84f, 0.0f);    // Gold
                default:                   return Color.white;
            }
        }

        // ===============================
        // Drag & Drop
        // ===============================

        public void OnPointerDown(PointerEventData eventData)
        {
            // Visual feedback on touch
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_currentCell == null) return;

            _isDragging = true;
            _startPosition = transform.position;
            _startCell = _currentCell;

            // Bring to front while dragging
            if (_spriteRenderer != null)
            {
                _originalSortingOrder = _spriteRenderer.sortingOrder;
                _spriteRenderer.sortingOrder = 100;
            }

            // Scale up slightly for feedback
            transform.localScale = Vector3.one * 1.15f;

            // Make semi-transparent
            if (_spriteRenderer != null)
            {
                Color c = _spriteRenderer.color;
                _spriteRenderer.color = new Color(c.r, c.g, c.b, 0.8f);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            // Follow pointer in world space
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = transform.position.z;
            transform.position = worldPos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            _isDragging = false;

            // Reset visual feedback
            transform.localScale = Vector3.one;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = _originalSortingOrder;
                Color c = _spriteRenderer.color;
                _spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);
            }

            // Check if dropped on a valid cell via raycast
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                GridCell targetCell = hit.collider.GetComponent<GridCell>();
                if (targetCell != null && targetCell != _startCell)
                {
                    // Attempt merge or move via GridManager
                    if (GridManager.Instance != null)
                    {
                        bool handled = GridManager.Instance.HandleItemDrop(this, targetCell);
                        if (handled) return;
                    }
                }
            }

            // If drop failed, return to start position
            ReturnToStartPosition();
        }

        /// <summary>
        /// Snap back to the starting position (drop failed or invalid).
        /// </summary>
        public void ReturnToStartPosition()
        {
            transform.position = _startPosition;
        }

        /// <summary>
        /// Move smoothly to a target position.
        /// </summary>
        public void MoveToPosition(Vector3 targetPos)
        {
            // Simple snap for now — can add smooth tween later
            transform.position = targetPos;
        }

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
    }
}
