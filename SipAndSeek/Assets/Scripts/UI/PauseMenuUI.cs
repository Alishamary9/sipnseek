using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SipAndSeek.Managers;

namespace SipAndSeek.UI
{
    /// <summary>
    /// Pause menu with resume, retry, settings, and main menu options.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private CanvasGroup _panelGroup;

        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _mainMenuButton;

        [Header("Info")]
        [SerializeField] private TextMeshProUGUI _levelText;

        private void Start()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(OnResume);
            if (_retryButton != null)
                _retryButton.onClick.AddListener(OnRetry);
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(OnMainMenu);

            Hide();
        }

        private void OnEnable()
        {
            GameManager.OnPauseStateChanged += HandlePauseChanged;
        }

        private void OnDisable()
        {
            GameManager.OnPauseStateChanged -= HandlePauseChanged;
        }

        private void HandlePauseChanged(bool isPaused)
        {
            if (isPaused)
            {
                if (_levelText != null && GameManager.Instance != null)
                    _levelText.text = $"Level {GameManager.Instance.CurrentLevelNumber}";
                Show();
            }
            else
            {
                Hide();
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

        private void OnResume()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ResumeGame();
        }

        private void OnRetry()
        {
            Hide();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame(); // Unpause first
                GameManager.Instance.RetryLevel();
            }
        }

        private void OnMainMenu()
        {
            Hide();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
                GameManager.Instance.GoToMainMenu();
            }
        }
    }
}
