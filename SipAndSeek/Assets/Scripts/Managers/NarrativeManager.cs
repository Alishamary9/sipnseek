using UnityEngine;
using System;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Orchestrates pre-level and post-level narrative sequences.
    /// Uses DialogueManager to display dialogue lines from localization.csv.
    /// Narrative keys follow the pattern: nar_lv{X}_before / nar_lv{X}_after
    /// </summary>
    public class NarrativeManager : MonoBehaviour
    {
        public static NarrativeManager Instance { get; private set; }

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when a narrative sequence starts.</summary>
        public static event Action<int, bool> OnNarrativeStarted; // level, isPre

        /// <summary>Fired when a narrative sequence ends.</summary>
        public static event Action<int, bool> OnNarrativeEnded; // level, isPre

        // State
        private int _currentNarrativeLevel;
        private bool _isPreLevel;
        private bool _isActive;
        public bool IsActive => _isActive;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[NarrativeManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ===============================
        // Pre-Level Narrative
        // ===============================

        /// <summary>
        /// Show the narrative text before a level starts.
        /// If no narrative exists for this level, calls onComplete immediately.
        /// </summary>
        public void ShowPreLevelNarrative(int level, Action onComplete = null)
        {
            string key = $"nar_lv{level}_before";
            ShowNarrative(level, true, key, onComplete);
        }

        // ===============================
        // Post-Level Narrative
        // ===============================

        /// <summary>
        /// Show the narrative text after a level is completed.
        /// If no narrative exists for this level, calls onComplete immediately.
        /// </summary>
        public void ShowPostLevelNarrative(int level, Action onComplete = null)
        {
            string key = $"nar_lv{level}_after";
            ShowNarrative(level, false, key, onComplete);
        }

        // ===============================
        // Core Logic
        // ===============================

        /// <summary>
        /// Show a narrative sequence using the specified localization key.
        /// </summary>
        private void ShowNarrative(int level, bool isPre, string locKey, Action onComplete)
        {
            // Check if localization text exists
            if (LocalizationManager.Instance == null)
            {
                Debug.LogWarning("[NarrativeManager] LocalizationManager not found.");
                onComplete?.Invoke();
                return;
            }

            string text = LocalizationManager.Instance.GetText(locKey);

            // If the returned text is the key itself, no localization entry exists
            if (string.IsNullOrEmpty(text) || text == locKey)
            {
                Debug.Log($"[NarrativeManager] No narrative for level {level} ({(isPre ? "pre" : "post")})");
                onComplete?.Invoke();
                return;
            }

            _currentNarrativeLevel = level;
            _isPreLevel = isPre;
            _isActive = true;

            Debug.Log($"[NarrativeManager] 📖 Showing {(isPre ? "pre" : "post")}-level narrative for Level {level}");
            OnNarrativeStarted?.Invoke(level, isPre);

            // Use DialogueManager to show the narrative
            if (DialogueManager.Instance != null)
            {
                // Determine the speaking character based on context
                string character = isPre ? "Laith" : "Grandma";
                string emotion = isPre ? "Curious" : "Warm";

                DialogueManager.Instance.StartDialogue();
                DialogueManager.Instance.ShowLine(character, emotion, locKey, () =>
                {
                    EndNarrative(onComplete);
                });
            }
            else
            {
                // No DialogueManager — just log and continue
                Debug.Log($"[NarrativeManager] 📝 \"{text}\"");
                EndNarrative(onComplete);
            }
        }

        /// <summary>
        /// End the current narrative and invoke the completion callback.
        /// </summary>
        private void EndNarrative(Action onComplete)
        {
            _isActive = false;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.EndDialogue();
            }

            Debug.Log($"[NarrativeManager] ✅ Narrative ended for Level {_currentNarrativeLevel}");
            OnNarrativeEnded?.Invoke(_currentNarrativeLevel, _isPreLevel);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Skip the current narrative immediately.
        /// </summary>
        public void SkipNarrative()
        {
            if (!_isActive) return;

            Debug.Log("[NarrativeManager] ⏭️ Narrative skipped.");
            EndNarrative(null);
        }

        /// <summary>
        /// Check if a narrative exists for a specific level and timing.
        /// </summary>
        public bool HasNarrative(int level, bool isPre)
        {
            if (LocalizationManager.Instance == null) return false;

            string key = isPre ? $"nar_lv{level}_before" : $"nar_lv{level}_after";
            string text = LocalizationManager.Instance.GetText(key);
            return !string.IsNullOrEmpty(text) && text != key;
        }
    }
}
