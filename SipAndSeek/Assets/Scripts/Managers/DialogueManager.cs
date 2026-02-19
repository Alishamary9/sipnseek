using UnityEngine;
using System;
using SipAndSeek.UI;
// using SipAndSeek.Enums; // Removed: Enums are in SipAndSeek namespace

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Manages the flow of dialogue sessions.
    /// Integrated with NodeCanvas for graph-based logic.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI Reference")]
        [SerializeField] private DialogueUI dialogueUI;

        // Events
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Auto-find UI if not assigned
            if (dialogueUI == null)
                dialogueUI = FindFirstObjectByType<DialogueUI>();
        }

        /// <summary>
        /// Starts a dialogue session. Pauses gameplay.
        /// </summary>
        public void StartDialogue()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetState(GameState.Dialogue);
            
            if (dialogueUI != null)
                dialogueUI.Show();

            OnDialogueStarted?.Invoke();
        }

        /// <summary>
        /// Ends the dialogue session. Resumes gameplay.
        /// </summary>
        public void EndDialogue()
        {
            if (dialogueUI != null)
                dialogueUI.Hide();

            if (GameManager.Instance != null)
                GameManager.Instance.SetState(GameState.Playing);

            OnDialogueEnded?.Invoke();
        }

        /// <summary>
        /// Displays a line of dialogue.
        /// </summary>
        /// <param name="characterKey">Character ID (e.g., "Laith")</param>
        /// <param name="emotion">Emotion ID (e.g., "Happy")</param>
        /// <param name="localizationKey">The key for the text in CSV</param>
        /// <param name="onComplete">Callback when typing finishes or user clicks next</param>
        public void ShowLine(string characterKey, string emotion, string localizationKey, Action onComplete)
        {
            if (dialogueUI == null)
            {
                Debug.LogError("DialogueUI not found!");
                onComplete?.Invoke();
                return;
            }

            // Get localized text
            string localizedText = LocalizationManager.Instance != null 
                ? LocalizationManager.Instance.GetText(localizationKey) 
                : localizationKey; // Fallback to key if manager missing

            // Get character name (could be localized too)
            string characterName = LocalizationManager.Instance != null
                ? LocalizationManager.Instance.GetText($"char_{characterKey}")
                : characterKey;

            dialogueUI.SetContent(characterName, emotion, localizedText, onComplete);
        }
    }
}
