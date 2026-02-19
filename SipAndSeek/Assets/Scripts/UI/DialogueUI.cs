using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

namespace SipAndSeek.UI
{
    /// <summary>
    /// Handles the visual presentation of dialogue.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject contentRoot;
        [SerializeField] private Image characterPortrait;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;

        [Header("Settings")]
        [SerializeField] private float typeSpeed = 0.05f;

        private Coroutine typingCoroutine;
        private string fullText;
        private bool isTyping;
        private Action onLineComplete;

        private void Awake()
        {
            if (nextButton != null)
                nextButton.onClick.AddListener(OnNextClicked);
                
            if (skipButton != null)
                skipButton.onClick.AddListener(OnSkipClicked);

            Hide(); // Start hidden
        }

        public void Show()
        {
            if (contentRoot != null) contentRoot.SetActive(true);
        }

        public void Hide()
        {
            if (contentRoot != null) contentRoot.SetActive(false);
        }

        public void SetContent(string name, string emotion, string text, Action onComplete)
        {
            this.fullText = text;
            this.onLineComplete = onComplete;
            
            if (nameText != null) nameText.text = name;
            
            // TODO: Load portrait based on emotion/character
            // characterPortrait.sprite = Resources.Load<Sprite>($"Images/Portraits/{name}_{emotion}");

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText());
        }

        private IEnumerator TypeText()
        {
            isTyping = true;
            if (dialogueText != null) dialogueText.text = "";

            foreach (char c in fullText)
            {
                if (dialogueText != null) dialogueText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            isTyping = false;
        }

        private void OnNextClicked()
        {
            if (isTyping)
            {
                // Instant finish
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                if (dialogueText != null) dialogueText.text = fullText;
                isTyping = false;
            }
            else
            {
                // Go to next line (Callback to NodeCanvas)
                onLineComplete?.Invoke();
            }
        }

        private void OnSkipClicked()
        {
            // Skip entire conversation? Or just this line?
            // For now, act as "Next"
            OnNextClicked();
        }
    }
}
