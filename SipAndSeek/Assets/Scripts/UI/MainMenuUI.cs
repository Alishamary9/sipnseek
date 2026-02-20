using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SipAndSeek.Managers;

namespace SipAndSeek.UI
{
    /// <summary>
    /// Main Menu with Play, Level Select, and Settings buttons.
    /// Shows player info (coins, gems, level).
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _levelSelectPanel;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _levelSelectButton;
        [SerializeField] private Button _settingsButton;

        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _gemsText;
        [SerializeField] private TextMeshProUGUI _playerLevelText;
        [SerializeField] private TextMeshProUGUI _titleText;

        [Header("Level Select")]
        [SerializeField] private Transform _levelButtonContainer;
        [SerializeField] private Button _levelSelectBackButton;
        [SerializeField] private int _maxLevelsToShow = 10;

        private void Start()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(OnPlayClicked);
            if (_levelSelectButton != null)
                _levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
            if (_levelSelectBackButton != null)
                _levelSelectBackButton.onClick.AddListener(OnBackToMenu);

            // Set title
            if (_titleText != null)
            {
                string title = LocalizationManager.Instance != null
                    ? LocalizationManager.Instance.GetText("game_title")
                    : "Sip & Seek";
                _titleText.text = title;
            }

            RefreshPlayerInfo();
            ShowMainPanel();
        }

        private void OnEnable()
        {
            GameManager.OnGameStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState oldState, GameState newState)
        {
            if (newState == GameState.MainMenu)
            {
                RefreshPlayerInfo();
                ShowMainPanel();
                gameObject.SetActive(true);
            }
            else if (newState == GameState.Loading || newState == GameState.Playing)
            {
                gameObject.SetActive(false);
            }
        }

        // ===============================
        // UI Updates
        // ===============================

        private void RefreshPlayerInfo()
        {
            if (PlayerDataManager.Instance == null) return;

            if (_coinsText != null)
                _coinsText.text = PlayerDataManager.Instance.Coins.ToString();
            if (_gemsText != null)
                _gemsText.text = PlayerDataManager.Instance.Gems.ToString();
            if (_playerLevelText != null)
                _playerLevelText.text = $"Lv. {PlayerDataManager.Instance.CurrentLevel}";
        }

        // ===============================
        // Panel Navigation
        // ===============================

        private void ShowMainPanel()
        {
            if (_mainPanel != null) _mainPanel.SetActive(true);
            if (_levelSelectPanel != null) _levelSelectPanel.SetActive(false);
        }

        private void ShowLevelSelect()
        {
            if (_mainPanel != null) _mainPanel.SetActive(false);
            if (_levelSelectPanel != null) _levelSelectPanel.SetActive(true);

            GenerateLevelButtons();
        }

        // ===============================
        // Level Buttons
        // ===============================

        private void GenerateLevelButtons()
        {
            if (_levelButtonContainer == null) return;

            // Clear existing buttons
            foreach (Transform child in _levelButtonContainer)
            {
                Destroy(child.gameObject);
            }

            int maxUnlocked = 1;
            if (PlayerDataManager.Instance != null)
            {
                maxUnlocked = PlayerDataManager.Instance.HighestUnlockedLevel;
            }

            for (int i = 1; i <= _maxLevelsToShow; i++)
            {
                int levelNumber = i; // Capture for lambda
                bool unlocked = levelNumber <= maxUnlocked;

                GameObject btnObj = new GameObject($"LevelBtn_{levelNumber}");
                btnObj.transform.SetParent(_levelButtonContainer, false);

                // Add button components
                Image btnImage = btnObj.AddComponent<Image>();
                btnImage.color = unlocked
                    ? new Color(0.85f, 0.75f, 0.55f, 1f)   // Warm gold
                    : new Color(0.6f, 0.6f, 0.6f, 0.5f);    // Gray locked

                Button btn = btnObj.AddComponent<Button>();
                btn.interactable = unlocked;

                // Add text
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(btnObj.transform, false);
                TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
                text.text = unlocked ? levelNumber.ToString() : "🔒";
                text.fontSize = 24;
                text.alignment = TextAlignmentOptions.Center;
                text.color = Color.white;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;

                // Set button size
                RectTransform btnRect = btnObj.GetComponent<RectTransform>();
                btnRect.sizeDelta = new Vector2(80, 80);

                // Star display for completed levels
                if (unlocked && PlayerDataManager.Instance != null)
                {
                    int stars = PlayerDataManager.Instance.GetLevelStars(levelNumber);
                    if (stars > 0)
                    {
                        text.text = $"{levelNumber}\n{"★".PadLeft(stars, '★')}";
                        text.fontSize = 18;
                    }
                }

                // Click handler
                if (unlocked)
                {
                    btn.onClick.AddListener(() => StartLevel(levelNumber));
                }
            }
        }

        // ===============================
        // Button Handlers
        // ===============================

        private void OnPlayClicked()
        {
            // Start the next available level
            int nextLevel = 1;
            if (PlayerDataManager.Instance != null)
                nextLevel = PlayerDataManager.Instance.HighestUnlockedLevel;

            StartLevel(nextLevel);
        }

        private void OnLevelSelectClicked()
        {
            ShowLevelSelect();
        }

        private void OnBackToMenu()
        {
            ShowMainPanel();
        }

        private void StartLevel(int levelNumber)
        {
            Debug.Log($"[MainMenuUI] Starting Level {levelNumber}");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartLevel(levelNumber);
            }
        }
    }
}
