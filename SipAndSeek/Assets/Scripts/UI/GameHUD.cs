using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SipAndSeek.Managers;
using SipAndSeek.Gameplay;
using SipAndSeek.Data;

namespace SipAndSeek.UI
{
    /// <summary>
    /// In-game HUD showing coins, gems, progress, merges, and level info.
    /// Auto-updates by listening to manager events.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Currency")]
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _gemsText;

        [Header("Progress")]
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Image _progressFill;

        [Header("Merge Info")]
        [SerializeField] private TextMeshProUGUI _mergeCountText;
        [SerializeField] private TextMeshProUGUI _moveCountText;

        [Header("Stars")]
        [SerializeField] private Image[] _starImages; // 3 stars

        [Header("Level Info")]
        [SerializeField] private TextMeshProUGUI _levelNameText;

        [Header("Buttons")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _spawnButton;

        [Header("Colors")]
        [SerializeField] private Color _progressColorLow = new Color(0.9f, 0.5f, 0.3f);
        [SerializeField] private Color _progressColorMid = new Color(0.9f, 0.8f, 0.3f);
        [SerializeField] private Color _progressColorFull = new Color(0.4f, 0.9f, 0.4f);

        // Star thresholds
        private float _star1Threshold = 0.8f;
        private float _star2Threshold = 0.9f;
        private float _star3Threshold = 1.0f;

        private void OnEnable()
        {
            LevelManager.OnLevelReady += HandleLevelReady;
            LevelManager.OnProgressUpdated += HandleProgressUpdated;
            MergeManager.OnMergePerformed += HandleMerge;
            MergeManager.OnMovesChanged += HandleMovesChanged;
            MergeManager.OnSellItem += HandleSellItem;
            EconomyManager.OnRewardGranted += HandleReward;
        }

        private void OnDisable()
        {
            LevelManager.OnLevelReady -= HandleLevelReady;
            LevelManager.OnProgressUpdated -= HandleProgressUpdated;
            MergeManager.OnMergePerformed -= HandleMerge;
            MergeManager.OnMovesChanged -= HandleMovesChanged;
            MergeManager.OnSellItem -= HandleSellItem;
            EconomyManager.OnRewardGranted -= HandleReward;
        }

        private void Start()
        {
            // Wire up buttons
            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(OnPauseClicked);
            if (_spawnButton != null)
                _spawnButton.onClick.AddListener(OnSpawnClicked);

            // Initial refresh
            RefreshCurrency();
        }

        // ===============================
        // Event Handlers
        // ===============================

        private void HandleLevelReady(LevelConfig config)
        {
            if (_levelNameText != null)
            {
                string levelTitle = LocalizationManager.Instance != null
                    ? LocalizationManager.Instance.GetText($"level_{config.levelNumber}_name")
                    : $"Level {config.levelNumber}";
                _levelNameText.text = levelTitle;
            }

            _star1Threshold = config.targetPercent;
            _star2Threshold = 0.9f;
            _star3Threshold = 1.0f;

            UpdateProgress(0f);
            UpdateMergeCount(0);
            UpdateMoves(0, config.moveLimit);
            RefreshCurrency();
        }

        private void HandleProgressUpdated(float progress)
        {
            UpdateProgress(progress);
        }

        private void HandleMerge(MergeItem oldItem, MergeItem newItem)
        {
            if (MergeManager.Instance != null)
                UpdateMergeCount(MergeManager.Instance.TotalMergesThisLevel);
            RefreshCurrency();
        }

        private void HandleMovesChanged(int used, int limit)
        {
            UpdateMoves(used, limit);
        }

        private void HandleSellItem(int coins)
        {
            RefreshCurrency();
            // TODO: Show floating "+coins" animation
        }

        private void HandleReward(RewardType type, int amount)
        {
            RefreshCurrency();
        }

        // ===============================
        // UI Updates
        // ===============================

        private void RefreshCurrency()
        {
            if (PlayerDataManager.Instance == null) return;

            if (_coinsText != null)
                _coinsText.text = FormatNumber(PlayerDataManager.Instance.Coins);
            if (_gemsText != null)
                _gemsText.text = FormatNumber(PlayerDataManager.Instance.Gems);
        }

        private void UpdateProgress(float progress)
        {
            if (_progressBar != null)
            {
                _progressBar.value = progress;

                // Color gradient based on progress
                if (_progressFill != null)
                {
                    if (progress >= _star3Threshold)
                        _progressFill.color = _progressColorFull;
                    else if (progress >= _star1Threshold)
                        _progressFill.color = _progressColorMid;
                    else
                        _progressFill.color = _progressColorLow;
                }
            }

            if (_progressText != null)
                _progressText.text = $"{progress * 100:F0}%";

            // Update stars
            UpdateStars(progress);
        }

        private void UpdateStars(float progress)
        {
            if (_starImages == null || _starImages.Length < 3) return;

            float[] thresholds = { _star1Threshold, _star2Threshold, _star3Threshold };
            for (int i = 0; i < 3 && i < _starImages.Length; i++)
            {
                if (_starImages[i] != null)
                {
                    bool earned = progress >= thresholds[i];
                    _starImages[i].color = earned
                        ? new Color(1f, 0.84f, 0f, 1f) // Gold
                        : new Color(0.5f, 0.5f, 0.5f, 0.4f); // Gray
                }
            }
        }

        private void UpdateMergeCount(int count)
        {
            if (_mergeCountText != null)
                _mergeCountText.text = $"{count}";
        }

        private void UpdateMoves(int used, int limit)
        {
            if (_moveCountText != null)
            {
                if (limit > 0)
                    _moveCountText.text = $"{used}/{limit}";
                else
                    _moveCountText.text = $"{used}";
            }
        }

        // ===============================
        // Button Handlers
        // ===============================

        private void OnPauseClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TogglePause();
        }

        private void OnSpawnClicked()
        {
            var gen = FindFirstObjectByType<Gameplay.ItemGenerator>();
            if (gen != null && gen.IsReady)
            {
                gen.GenerateAndSpawn();
            }
        }

        // ===============================
        // Helpers
        // ===============================

        private string FormatNumber(int number)
        {
            if (number >= 1000000) return $"{number / 1000000f:F1}M";
            if (number >= 1000) return $"{number / 1000f:F1}K";
            return number.ToString();
        }
    }
}
