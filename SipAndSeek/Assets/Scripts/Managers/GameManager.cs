using UnityEngine;
using System;
using SipAndSeek;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Central game manager — controls game state, orchestrates all systems.
    /// Singleton MonoBehaviour with DontDestroyOnLoad.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // ===============================
        // State
        // ===============================
        private GameState _currentState = GameState.MainMenu;
        public GameState CurrentState => _currentState;

        private int _currentLevelNumber;
        public int CurrentLevelNumber => _currentLevelNumber;

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired whenever the game state changes.</summary>
        public static event Action<GameState, GameState> OnGameStateChanged;

        /// <summary>Fired when a level starts.</summary>
        public static event Action<int> OnLevelStarted;

        /// <summary>Fired when a level is completed (level, stars, completion%).</summary>
        public static event Action<int, int, float> OnLevelCompleted;

        /// <summary>Fired when a level is failed.</summary>
        public static event Action<int> OnLevelFailed;

        /// <summary>Fired when the game is paused or resumed.</summary>
        public static event Action<bool> OnPauseStateChanged;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[GameManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Process daily login if PlayerDataManager is ready
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.ProcessDailyLogin();
            }
        }

        // ===============================
        // State Machine
        // ===============================

        /// <summary>
        /// Transition to a new game state. Fires OnGameStateChanged event.
        /// </summary>
        public void SetState(GameState newState)
        {
            if (_currentState == newState) return;

            GameState oldState = _currentState;
            _currentState = newState;

            Debug.Log($"[GameManager] State: {oldState} → {newState}");
            OnGameStateChanged?.Invoke(oldState, newState);

            // Handle time scale for pause
            if (newState == GameState.Paused)
            {
                Time.timeScale = 0f;
            }
            else if (oldState == GameState.Paused)
            {
                Time.timeScale = 1f;
            }
        }

        // ===============================
        // Level Flow
        // ===============================

        /// <summary>
        /// Start a level by number. Call this from level select or auto-continue.
        /// </summary>
        public void StartLevel(int levelNumber)
        {
            _currentLevelNumber = levelNumber;
            SetState(GameState.Loading);

            Debug.Log($"[GameManager] 🎮 Starting Level {levelNumber}");
            OnLevelStarted?.Invoke(levelNumber);

            // After loading is complete, transition to Playing
            // (In practice, SceneLoader would call ReadyToPlay())
            SetState(GameState.Playing);
        }

        /// <summary>
        /// Called by LevelManager when loading is done and gameplay can begin.
        /// </summary>
        public void ReadyToPlay()
        {
            SetState(GameState.Playing);
        }

        /// <summary>
        /// Complete the current level with a star rating and completion percentage.
        /// </summary>
        public void CompleteLevel(int stars, float completionPercent)
        {
            if (_currentState != GameState.Playing) return;

            Debug.Log($"[GameManager] ✅ Level {_currentLevelNumber} Complete! Stars: {stars}, Progress: {completionPercent:F1}%");

            // Save progress
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.SetLevelStars(_currentLevelNumber, stars);
                PlayerDataManager.Instance.FlushStats();
            }

            SetState(GameState.LevelComplete);
            OnLevelCompleted?.Invoke(_currentLevelNumber, stars, completionPercent);
        }

        /// <summary>
        /// Fail the current level (e.g., ran out of moves).
        /// </summary>
        public void FailLevel()
        {
            if (_currentState != GameState.Playing) return;

            Debug.Log($"[GameManager] ❌ Level {_currentLevelNumber} Failed.");

            SetState(GameState.LevelFailed);
            OnLevelFailed?.Invoke(_currentLevelNumber);
        }

        /// <summary>
        /// Retry the current level.
        /// </summary>
        public void RetryLevel()
        {
            Debug.Log($"[GameManager] 🔄 Retrying Level {_currentLevelNumber}");
            StartLevel(_currentLevelNumber);
        }

        /// <summary>
        /// Proceed to the next level.
        /// </summary>
        public void NextLevel()
        {
            StartLevel(_currentLevelNumber + 1);
        }

        // ===============================
        // Pause / Resume
        // ===============================

        /// <summary>
        /// Pause the game. Only works during Playing state.
        /// </summary>
        public void PauseGame()
        {
            if (_currentState != GameState.Playing) return;
            SetState(GameState.Paused);
            OnPauseStateChanged?.Invoke(true);
        }

        /// <summary>
        /// Resume the game from pause.
        /// </summary>
        public void ResumeGame()
        {
            if (_currentState != GameState.Paused) return;
            SetState(GameState.Playing);
            OnPauseStateChanged?.Invoke(false);
        }

        /// <summary>
        /// Toggle pause state.
        /// </summary>
        public void TogglePause()
        {
            if (_currentState == GameState.Playing)
                PauseGame();
            else if (_currentState == GameState.Paused)
                ResumeGame();
        }

        // ===============================
        // Navigation
        // ===============================

        /// <summary>
        /// Return to the main menu.
        /// </summary>
        public void GoToMainMenu()
        {
            Time.timeScale = 1f; // Ensure time is normal
            SetState(GameState.MainMenu);
        }

        /// <summary>
        /// Enter dialogue state (before/after level narrative).
        /// </summary>
        public void EnterDialogue()
        {
            SetState(GameState.Dialogue);
        }

        /// <summary>
        /// Exit dialogue and return to the appropriate state.
        /// </summary>
        public void ExitDialogue()
        {
            // After dialogue, resume playing or stay in level complete
            if (_currentLevelNumber > 0)
            {
                SetState(GameState.Playing);
            }
            else
            {
                SetState(GameState.MainMenu);
            }
        }
    }
}
