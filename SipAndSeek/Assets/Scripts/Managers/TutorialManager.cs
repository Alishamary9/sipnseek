using UnityEngine;
using System;
using System.Collections.Generic;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Interactive tutorial system for new players.
    /// Guides players through core mechanics with step-by-step instructions.
    /// Tutorial levels: Lv1 (basic merge), Lv2 (locks), Lv3 (ice), Lv5 (powerups).
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        private const string PREFS_PREFIX = "tutorial_lv";
        private const string PREFS_SUFFIX = "_done";

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when a tutorial starts (level).</summary>
        public static event Action<int> OnTutorialStarted;

        /// <summary>Fired when a tutorial step changes (level, stepIndex, locKey).</summary>
        public static event Action<int, int, string> OnTutorialStepChanged;

        /// <summary>Fired when a tutorial is completed (level).</summary>
        public static event Action<int> OnTutorialCompleted;

        // ===============================
        // Tutorial Definitions
        // ===============================

        [System.Serializable]
        private class TutorialSequence
        {
            public int level;
            public string[] stepKeys; // Localization keys for each step
        }

        // Hardcoded tutorial sequences — keys reference localization.csv
        private static readonly Dictionary<int, string[]> TutorialSteps = new Dictionary<int, string[]>
        {
            {
                1, new[]
                {
                    "tut_lv1_step1", // "Welcome! Drag items to merge them"
                    "tut_lv1_step2", // "Match two identical items"
                    "tut_lv1_step3", // "Merging reveals hidden image tiles"
                    "tut_lv1_step4", // "Reveal 80% of the image to pass!"
                }
            },
            {
                2, new[]
                {
                    "tut_lv2_step1", // "Locked tiles block the image"
                    "tut_lv2_step2", // "Merge a Level 3+ item nearby to unlock"
                }
            },
            {
                3, new[]
                {
                    "tut_lv3_step1", // "Frozen tiles need two steps to clear"
                    "tut_lv3_step2", // "First merge: Ice → Lock"
                    "tut_lv3_step3", // "Second merge: Lock → Free!"
                }
            },
            {
                5, new[]
                {
                    "tut_lv5_step1", // "Use powerups to help you!"
                    "tut_lv5_step2", // "Hint reveals a tile"
                    "tut_lv5_step3", // "Shuffle rearranges all items"
                }
            }
        };

        // State
        private int _currentLevel;
        private int _currentStep;
        private string[] _currentSteps;
        private bool _isActive;

        public bool IsActive => _isActive;
        public int CurrentLevel => _currentLevel;
        public int CurrentStep => _currentStep;
        public int TotalSteps => _currentSteps != null ? _currentSteps.Length : 0;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[TutorialManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ===============================
        // Tutorial Flow
        // ===============================

        /// <summary>
        /// Start the tutorial for a specific level.
        /// Does nothing if the tutorial for this level was already completed.
        /// </summary>
        public void StartTutorial(int level)
        {
            // Check if already completed
            if (IsTutorialCompleted(level))
            {
                Debug.Log($"[TutorialManager] Tutorial for level {level} already completed.");
                return;
            }

            // Check if tutorial exists for this level
            if (!TutorialSteps.ContainsKey(level))
            {
                Debug.Log($"[TutorialManager] No tutorial defined for level {level}.");
                return;
            }

            _currentLevel = level;
            _currentSteps = TutorialSteps[level];
            _currentStep = 0;
            _isActive = true;

            Debug.Log($"[TutorialManager] 📚 Tutorial started for Level {level} ({_currentSteps.Length} steps)");
            OnTutorialStarted?.Invoke(level);

            ShowCurrentStep();
        }

        /// <summary>
        /// Advance to the next tutorial step.
        /// Completes the tutorial if all steps are done.
        /// </summary>
        public void NextStep()
        {
            if (!_isActive) return;

            _currentStep++;

            if (_currentStep >= _currentSteps.Length)
            {
                CompleteTutorial();
            }
            else
            {
                ShowCurrentStep();
            }
        }

        /// <summary>
        /// Show the current step's instruction text.
        /// </summary>
        private void ShowCurrentStep()
        {
            if (_currentSteps == null || _currentStep >= _currentSteps.Length) return;

            string locKey = _currentSteps[_currentStep];

            // Get localized text
            string text = locKey;
            if (LocalizationManager.Instance != null)
            {
                string localized = LocalizationManager.Instance.GetText(locKey);
                if (!string.IsNullOrEmpty(localized) && localized != locKey)
                    text = localized;
            }

            Debug.Log($"[TutorialManager] Step {_currentStep + 1}/{_currentSteps.Length}: {text}");
            OnTutorialStepChanged?.Invoke(_currentLevel, _currentStep, locKey);
        }

        /// <summary>
        /// Complete the current tutorial and save progress.
        /// </summary>
        public void CompleteTutorial()
        {
            if (!_isActive) return;

            int level = _currentLevel;
            _isActive = false;
            _currentSteps = null;

            // Save completion
            PlayerPrefs.SetInt(PREFS_PREFIX + level + PREFS_SUFFIX, 1);
            PlayerPrefs.Save();

            Debug.Log($"[TutorialManager] ✅ Tutorial completed for Level {level}");
            OnTutorialCompleted?.Invoke(level);
        }

        /// <summary>
        /// Skip the current tutorial entirely.
        /// </summary>
        public void SkipTutorial()
        {
            if (!_isActive) return;

            Debug.Log($"[TutorialManager] ⏭️ Tutorial skipped for Level {_currentLevel}");
            CompleteTutorial();
        }

        // ===============================
        // Queries
        // ===============================

        /// <summary>
        /// Check if the tutorial for a specific level has been completed.
        /// </summary>
        public bool IsTutorialCompleted(int level)
        {
            return PlayerPrefs.GetInt(PREFS_PREFIX + level + PREFS_SUFFIX, 0) == 1;
        }

        /// <summary>
        /// Check if a tutorial exists for a specific level.
        /// </summary>
        public bool HasTutorial(int level)
        {
            return TutorialSteps.ContainsKey(level);
        }

        /// <summary>
        /// Reset all tutorial progress (for testing).
        /// </summary>
        public void ResetAllTutorials()
        {
            foreach (var kvp in TutorialSteps)
            {
                PlayerPrefs.DeleteKey(PREFS_PREFIX + kvp.Key + PREFS_SUFFIX);
            }
            PlayerPrefs.Save();
            Debug.Log("[TutorialManager] 🔄 All tutorials reset.");
        }
    }
}
