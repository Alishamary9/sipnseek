
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using tmpro = TMPro; // Alias to avoid conflicts
using SipAndSeek.UI;

namespace SipAndSeek.Editor
{
    public class DialogueSystemGenerator
    {
        [MenuItem("Tools/Sip & Seek/Create Dialogue UI")]
        public static void CreateDialogueUI()
        {
            // 1. Create Canvas
            GameObject canvasObj = new GameObject("DialogueCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // 2. Create Panel (Content Root)
            GameObject panelObj = new GameObject("DialoguePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.1f);
            panelRect.anchorMax = new Vector2(0.9f, 0.4f); // Bottom area
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // 3. Create Portrait
            GameObject portraitObj = new GameObject("Portrait");
            portraitObj.transform.SetParent(panelObj.transform, false);
            Image portraitImage = portraitObj.AddComponent<Image>();
            RectTransform portraitRect = portraitObj.GetComponent<RectTransform>();
            portraitRect.anchorMin = new Vector2(0, 0);
            portraitRect.anchorMax = new Vector2(0.2f, 1);
            portraitRect.offsetMin = new Vector2(10, 10);
            portraitRect.offsetMax = new Vector2(-10, -10);

            // 4. Create Name Text
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(panelObj.transform, false);
            tmpro.TextMeshProUGUI nameText = nameObj.AddComponent<tmpro.TextMeshProUGUI>();
            nameText.text = "Character Name";
            nameText.fontSize = 24;
            nameText.fontStyle = tmpro.FontStyles.Bold;
            nameText.alignment = tmpro.TextAlignmentOptions.TopLeft;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.22f, 0.8f);
            nameRect.anchorMax = new Vector2(0.9f, 0.95f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            // 5. Create Dialogue Text
            GameObject dialogueObj = new GameObject("DialogueText");
            dialogueObj.transform.SetParent(panelObj.transform, false);
            tmpro.TextMeshProUGUI dialogueText = dialogueObj.AddComponent<tmpro.TextMeshProUGUI>();
            dialogueText.text = "Dialogue text goes here...";
            dialogueText.fontSize = 20;
            dialogueText.alignment = tmpro.TextAlignmentOptions.TopLeft;
            RectTransform dialogueRect = dialogueObj.GetComponent<RectTransform>();
            dialogueRect.anchorMin = new Vector2(0.22f, 0.2f);
            dialogueRect.anchorMax = new Vector2(0.9f, 0.75f);
            dialogueRect.offsetMin = Vector2.zero;
            dialogueRect.offsetMax = Vector2.zero;

            // 6. Create Next Button
            GameObject nextBtnObj = new GameObject("NextButton");
            nextBtnObj.transform.SetParent(panelObj.transform, false);
            Image btnImage = nextBtnObj.AddComponent<Image>();
            btnImage.color = Color.gray;
            Button nextBtn = nextBtnObj.AddComponent<Button>();
            RectTransform btnRect = nextBtnObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.85f, 0.05f);
            btnRect.anchorMax = new Vector2(0.98f, 0.25f);
            btnRect.offsetMin = Vector2.zero;
            btnRect.offsetMax = Vector2.zero;

            GameObject btnTextObj = new GameObject("Text");
            btnTextObj.transform.SetParent(nextBtnObj.transform, false);
            tmpro.TextMeshProUGUI btnText = btnTextObj.AddComponent<tmpro.TextMeshProUGUI>();
            btnText.text = "Next";
            btnText.alignment = tmpro.TextAlignmentOptions.Center;
            RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.offsetMin = Vector2.zero;
            btnTextRect.offsetMax = Vector2.zero;

            // 7. Add DialogueUI Component
            DialogueUI ui = canvasObj.AddComponent<DialogueUI>();
            
            // Assign References via SerializedObject to access private fields
            SerializedObject so = new SerializedObject(ui);
            so.FindProperty("contentRoot").objectReferenceValue = panelObj;
            so.FindProperty("characterPortrait").objectReferenceValue = portraitImage;
            so.FindProperty("nameText").objectReferenceValue = nameText;
            so.FindProperty("dialogueText").objectReferenceValue = dialogueText;
            so.FindProperty("nextButton").objectReferenceValue = nextBtn;
            so.ApplyModifiedProperties();

            // 8. Create Managers
            GameObject managersObj = new GameObject("Managers");
            managersObj.AddComponent<SipAndSeek.Managers.DialogueManager>();
            managersObj.AddComponent<SipAndSeek.Managers.LocalizationManager>();
            // Also need GameManager as it's referenced
            managersObj.AddComponent<SipAndSeek.Managers.GameManager>();

            Selection.activeGameObject = canvasObj;
            Debug.Log("Dialogue UI & Managers created successfully!");
        }
    }
}
