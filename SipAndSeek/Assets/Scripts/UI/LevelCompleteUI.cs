using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SipAndSeek.Managers;

namespace SipAndSeek.UI
{
    /// <summary>
    /// Level Complete popup — shows stars, rewards, and next/retry buttons.
    /// </summary>
    public class LevelCompleteUI : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _subtitleText;
        [SerializeField] private Image[] _starImages;
        [SerializeField] private TextMeshProUGUI _coinsRewardText;
        [SerializeField] private TextMeshProUGUI _gemsRewardText;

        [Header("Buttons")]
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _mainMenuButton;

        [Header("Panel")]
        [SerializeField] private CanvasGroup _panelGroup;
        [SerializeField] private GameObject _panel;

        private int _earnedStars;
        private float _completionPercent;

        private void Start()
        {
            if (_nextLevelButton != null)
                _nextLevelButton.onClick.AddListener(OnNextLevel);
            if (_retryButton != null)
                _retryButton.onClick.AddListener(OnRetry);
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenu);

            Hide();
        }

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += HandleLevelCompleted;
            GameManager.OnLevelFailed += HandleLevelFailed;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= HandleLevelCompleted;
            GameManager.OnLevelFailed -= HandleLevelFailed;
        }

        // ===============================
        // Event Handlers
        // ===============================

        private void HandleLevelCompleted(int level, int stars, float completion)
        {
            _earnedStars = stars;
            _completionPercent = completion;

            if (_titleText != null)
            {
                string title = LocalizationManager.Instance != null
                    ? LocalizationManager.Instance.GetText("ui_level_complete")
                    : "Level Complete!";
                _titleText.text = title;
            }

            if (_subtitleText != null)
                _subtitleText.text = $"⭐ {stars}/3  •  {completion:F0}%";

            // Animate stars
            UpdateStars(stars);

            // Show rewards
            if (PlayerDataManager.Instance != null)
            {
                if (_coinsRewardText != null)
                    _coinsRewardText.text = $"+{GetLevelCoins(level, stars)}";
                if (_gemsRewardText != null)
                    _gemsRewardText.text = stars >= 3 ? $"+{5 + level / 2}" : "";
            }

            Show();
        }

        private void HandleLevelFailed(int level)
        {
            if (_titleText != null)
            {
                string title = LocalizationManager.Instance != null
                    ? LocalizationManager.Instance.GetText("ui_level_failed")
                    : "Try Again!";
                _titleText.text = title;
            }

            if (_subtitleText != null)
                _subtitleText.text = "Out of moves";

            UpdateStars(0);

            if (_nextLevelButton != null)
                _nextLevelButton.gameObject.SetActive(false);

            Show();
        }

        // ===============================
        // UI
        // ===============================

        private void UpdateStars(int stars)
        {
            if (_starImages == null) return;

            for (int i = 0; i < _starImages.Length && i < 3; i++)
            {
                if (_starImages[i] != null)
                {
                    bool earned = i < stars;
                    _starImages[i].color = earned
                        ? new Color(1f, 0.84f, 0f, 1f)
                        : new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    _starImages[i].transform.localScale = earned
                        ? Vector3.one * 1.2f
                        : Vector3.one;
                }
            }
        }

        public void Show()
        {
            if (_panel != null) _panel.SetActive(true);
            if (_panelGroup != null)
            {
                _panelGroup.alpha = 1f;
                _panelGroup.interactable = true;
                _panelGroup.blocksRaycasts = true;
            }
        }

        public void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
            if (_panelGroup != null)
            {
                _panelGroup.alpha = 0f;
                _panelGroup.interactable = false;
                _panelGroup.blocksRaycasts = false;
            }
        }

        // ===============================
        // Button Handlers
        // ===============================

        private void OnNextLevel()
        {
            Hide();
            if (GameManager.Instance != null)
                GameManager.Instance.NextLevel();
        }

        private void OnRetry()
        {
            Hide();
            if (GameManager.Instance != null)
                GameManager.Instance.RetryLevel();
        }

        private void OnMainMenu()
        {
            Hide();
            if (GameManager.Instance != null)
                GameManager.Instance.GoToMainMenu();
        }

        // ===============================
        // Helpers
        // ===============================

        private int GetLevelCoins(int level, int stars)
        {
            int baseCoins = 50 + (level * 25);
            float multiplier = stars switch
            {
                3 => 1.0f,
                2 => 0.75f,
                1 => 0.5f,
                _ => 0.25f
            };
            return Mathf.RoundToInt(baseCoins * multiplier);
        }
    }
}
