#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

namespace SipAndSeek.Editor
{
    /// <summary>
    /// Editor tool to auto-generate the full game UI Canvas.
    /// Creates HUD, Level Complete, Pause Menu, and Main Menu panels.
    /// Run via Tools → Sip & Seek → Generate Game UI.
    /// </summary>
    public class UICanvasGenerator
    {
        // ========== Color Palette (from GDD Visual Guide) ==========
        private static readonly Color CreamBG = new Color(0.96f, 0.96f, 0.86f, 1f);       // #F5F5DC
        private static readonly Color WarmBrown = new Color(0.43f, 0.30f, 0.25f, 1f);      // #6D4C41
        private static readonly Color BrassGold = new Color(1f, 0.84f, 0.31f, 1f);          // #FFD54F
        private static readonly Color VintageRose = new Color(1f, 0.67f, 0.57f, 1f);        // #FFAB91
        private static readonly Color SageGreen = new Color(0.65f, 0.84f, 0.65f, 1f);       // #A5D6A7
        private static readonly Color PanelBG = new Color(0.25f, 0.22f, 0.18f, 0.85f);     // Dark semi-transparent
        private static readonly Color ButtonColor = new Color(0.85f, 0.75f, 0.55f, 1f);     // Warm sand

        [MenuItem("Tools/Sip & Seek/Generate Game UI")]
        public static void GenerateUI()
        {
            // Ensure EventSystem exists
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            // Create main Canvas
            GameObject canvasObj = new GameObject("GameCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Create panels
            CreateHUD(canvasObj.transform);
            CreateLevelCompletePanel(canvasObj.transform);
            CreatePauseMenuPanel(canvasObj.transform);

            // Mark as dirty
            EditorUtility.SetDirty(canvasObj);
            Selection.activeGameObject = canvasObj;

            Debug.Log("[UICanvasGenerator] ✅ Game UI Canvas generated!");
            Debug.Log("  📌 Don't forget to add GameHUD, LevelCompleteUI, and PauseMenuUI scripts to the respective panels.");
        }

        // ========================================
        // HUD
        // ========================================
        private static void CreateHUD(Transform parent)
        {
            GameObject hud = CreatePanel(parent, "HUD_Panel", new Color(0, 0, 0, 0));
            SetAnchors(hud, Vector2.zero, Vector2.one);

            // --- Top Bar ---
            GameObject topBar = CreatePanel(hud.transform, "TopBar", new Color(0.25f, 0.2f, 0.15f, 0.7f));
            SetAnchors(topBar, new Vector2(0, 0.94f), Vector2.one);

            // Coins
            GameObject coinsGroup = CreatePanel(topBar.transform, "CoinsGroup", Color.clear);
            SetAnchors(coinsGroup, new Vector2(0.02f, 0.1f), new Vector2(0.3f, 0.9f));
            CreateIcon(coinsGroup.transform, "CoinIcon", "🪙", BrassGold);
            TextMeshProUGUI coinsText = CreateTMP(coinsGroup.transform, "CoinsText", "0",
                24, Color.white, TextAlignmentOptions.Left);
            SetAnchors(coinsText.gameObject, new Vector2(0.35f, 0), new Vector2(1, 1));

            // Gems
            GameObject gemsGroup = CreatePanel(topBar.transform, "GemsGroup", Color.clear);
            SetAnchors(gemsGroup, new Vector2(0.32f, 0.1f), new Vector2(0.55f, 0.9f));
            CreateIcon(gemsGroup.transform, "GemIcon", "💎", VintageRose);
            TextMeshProUGUI gemsText = CreateTMP(gemsGroup.transform, "GemsText", "0",
                24, Color.white, TextAlignmentOptions.Left);
            SetAnchors(gemsText.gameObject, new Vector2(0.35f, 0), new Vector2(1, 1));

            // Pause Button
            GameObject pauseBtn = CreateButton(topBar.transform, "PauseButton", "⏸", ButtonColor);
            SetAnchors(pauseBtn, new Vector2(0.88f, 0.1f), new Vector2(0.98f, 0.9f));

            // --- Progress Bar ---
            GameObject progressArea = CreatePanel(hud.transform, "ProgressArea", Color.clear);
            SetAnchors(progressArea, new Vector2(0.05f, 0.88f), new Vector2(0.95f, 0.93f));

            // Slider
            GameObject sliderObj = new GameObject("ProgressBar");
            sliderObj.transform.SetParent(progressArea.transform, false);
            SetAnchors(sliderObj, Vector2.zero, Vector2.one);

            Image sliderBg = sliderObj.AddComponent<Image>();
            sliderBg.color = new Color(0.3f, 0.25f, 0.2f, 0.5f);

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 0;

            // Fill
            GameObject fillArea = CreatePanel(sliderObj.transform, "Fill Area", Color.clear);
            SetAnchors(fillArea, new Vector2(0.01f, 0.1f), new Vector2(0.99f, 0.9f));

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = BrassGold;
            SetAnchors(fill, Vector2.zero, Vector2.one);

            slider.fillRect = fill.GetComponent<RectTransform>();

            // Progress Text
            TextMeshProUGUI progressText = CreateTMP(progressArea.transform, "ProgressText", "0%",
                20, Color.white, TextAlignmentOptions.Center);
            SetAnchors(progressText.gameObject, Vector2.zero, Vector2.one);

            // --- Stars (below progress) ---
            GameObject starsArea = CreatePanel(hud.transform, "StarsArea", Color.clear);
            SetAnchors(starsArea, new Vector2(0.3f, 0.85f), new Vector2(0.7f, 0.88f));

            for (int i = 0; i < 3; i++)
            {
                float x = 0.1f + i * 0.3f;
                TextMeshProUGUI star = CreateTMP(starsArea.transform, $"Star_{i}", "★",
                    28, new Color(0.5f, 0.5f, 0.5f, 0.4f), TextAlignmentOptions.Center);
                SetAnchors(star.gameObject, new Vector2(x, 0), new Vector2(x + 0.2f, 1));
            }

            // --- Bottom Bar (Merge Info + Spawn Button) ---
            GameObject bottomBar = CreatePanel(hud.transform, "BottomBar",
                new Color(0.25f, 0.2f, 0.15f, 0.7f));
            SetAnchors(bottomBar, new Vector2(0, 0), new Vector2(1, 0.06f));

            TextMeshProUGUI mergeText = CreateTMP(bottomBar.transform, "MergeCountText", "Merges: 0",
                18, Color.white, TextAlignmentOptions.Left);
            SetAnchors(mergeText.gameObject, new Vector2(0.05f, 0), new Vector2(0.4f, 1));

            TextMeshProUGUI moveText = CreateTMP(bottomBar.transform, "MoveCountText", "Moves: ∞",
                18, Color.white, TextAlignmentOptions.Center);
            SetAnchors(moveText.gameObject, new Vector2(0.4f, 0), new Vector2(0.65f, 1));

            GameObject spawnBtn = CreateButton(bottomBar.transform, "SpawnButton", "＋", SageGreen);
            SetAnchors(spawnBtn, new Vector2(0.75f, 0.1f), new Vector2(0.95f, 0.9f));

            Debug.Log("[UICanvasGenerator]   ✅ HUD created");
        }

        // ========================================
        // Level Complete Panel
        // ========================================
        private static void CreateLevelCompletePanel(Transform parent)
        {
            // Dimming background
            GameObject dimBg = CreatePanel(parent, "LevelComplete_Dim", new Color(0, 0, 0, 0.5f));
            SetAnchors(dimBg, Vector2.zero, Vector2.one);

            GameObject panel = CreatePanel(dimBg.transform, "LevelCompletePanel", PanelBG);
            SetAnchors(panel, new Vector2(0.1f, 0.25f), new Vector2(0.9f, 0.75f));

            // Round corners visual (just color adjustment)
            Image panelImg = panel.GetComponent<Image>();
            if (panelImg != null) panelImg.color = new Color(0.2f, 0.18f, 0.15f, 0.95f);

            // Title
            CreateTMP(panel.transform, "TitleText", "Level Complete!",
                36, BrassGold, TextAlignmentOptions.Center,
                new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.95f));

            // Subtitle
            CreateTMP(panel.transform, "SubtitleText", "⭐⭐⭐ 100%",
                24, Color.white, TextAlignmentOptions.Center,
                new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.75f));

            // Stars
            GameObject starsRow = CreatePanel(panel.transform, "StarsRow", Color.clear);
            SetAnchors(starsRow, new Vector2(0.2f, 0.45f), new Vector2(0.8f, 0.62f));
            for (int i = 0; i < 3; i++)
            {
                float x = 0.05f + i * 0.32f;
                TextMeshProUGUI star = CreateTMP(starsRow.transform, $"Star_{i}", "★",
                    48, BrassGold, TextAlignmentOptions.Center);
                SetAnchors(star.gameObject, new Vector2(x, 0), new Vector2(x + 0.28f, 1));
            }

            // Rewards
            CreateTMP(panel.transform, "CoinsReward", "+100 🪙",
                22, BrassGold, TextAlignmentOptions.Center,
                new Vector2(0.1f, 0.35f), new Vector2(0.5f, 0.45f));
            CreateTMP(panel.transform, "GemsReward", "+5 💎",
                22, VintageRose, TextAlignmentOptions.Center,
                new Vector2(0.5f, 0.35f), new Vector2(0.9f, 0.45f));

            // Buttons
            CreateButton(panel.transform, "NextLevelButton", "Next Level →", SageGreen,
                new Vector2(0.15f, 0.12f), new Vector2(0.85f, 0.25f));
            CreateButton(panel.transform, "RetryButton", "Retry", ButtonColor,
                new Vector2(0.15f, 0.02f), new Vector2(0.48f, 0.11f));
            CreateButton(panel.transform, "MenuButton", "Menu", new Color(0.6f, 0.5f, 0.4f),
                new Vector2(0.52f, 0.02f), new Vector2(0.85f, 0.11f));

            // Add CanvasGroup for fade
            CanvasGroup cg = dimBg.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            dimBg.SetActive(false);

            Debug.Log("[UICanvasGenerator]   ✅ Level Complete panel created");
        }

        // ========================================
        // Pause Menu Panel
        // ========================================
        private static void CreatePauseMenuPanel(Transform parent)
        {
            GameObject dimBg = CreatePanel(parent, "PauseMenu_Dim", new Color(0, 0, 0, 0.6f));
            SetAnchors(dimBg, Vector2.zero, Vector2.one);

            GameObject panel = CreatePanel(dimBg.transform, "PauseMenuPanel",
                new Color(0.2f, 0.18f, 0.15f, 0.95f));
            SetAnchors(panel, new Vector2(0.15f, 0.3f), new Vector2(0.85f, 0.7f));

            // Title
            CreateTMP(panel.transform, "PauseTitle", "⏸ Paused",
                32, CreamBG, TextAlignmentOptions.Center,
                new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.95f));

            // Level info
            CreateTMP(panel.transform, "LevelText", "Level 1",
                20, new Color(0.8f, 0.7f, 0.6f), TextAlignmentOptions.Center,
                new Vector2(0.2f, 0.62f), new Vector2(0.8f, 0.75f));

            // Buttons
            CreateButton(panel.transform, "ResumeButton", "▶ Resume", SageGreen,
                new Vector2(0.15f, 0.4f), new Vector2(0.85f, 0.58f));
            CreateButton(panel.transform, "RetryButton", "🔄 Retry", ButtonColor,
                new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.38f));
            CreateButton(panel.transform, "MainMenuButton", "🏠 Main Menu",
                new Color(0.6f, 0.5f, 0.4f),
                new Vector2(0.15f, 0.04f), new Vector2(0.85f, 0.18f));

            CanvasGroup cg = dimBg.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            dimBg.SetActive(false);

            Debug.Log("[UICanvasGenerator]   ✅ Pause Menu panel created");
        }

        // ========================================
        // Utility Methods
        // ========================================

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            Image img = obj.AddComponent<Image>();
            img.color = color;
            if (color.a <= 0.01f) img.raycastTarget = false;

            return obj;
        }

        private static void SetAnchors(GameObject obj, Vector2 min, Vector2 max)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect == null) rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }

        private static TextMeshProUGUI CreateTMP(Transform parent, string name, string text,
            int fontSize, Color color, TextAlignmentOptions alignment,
            Vector2? anchorMin = null, Vector2? anchorMax = null)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.enableAutoSizing = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            tmp.raycastTarget = false;

            if (anchorMin.HasValue && anchorMax.HasValue)
            {
                SetAnchors(obj, anchorMin.Value, anchorMax.Value);
            }

            return tmp;
        }

        private static void CreateIcon(Transform parent, string name, string emoji, Color color)
        {
            TextMeshProUGUI icon = CreateTMP(parent, name, emoji,
                24, color, TextAlignmentOptions.Center);
            SetAnchors(icon.gameObject, new Vector2(0, 0), new Vector2(0.3f, 1));
        }

        private static GameObject CreateButton(Transform parent, string name, string label,
            Color color, Vector2? anchorMin = null, Vector2? anchorMax = null)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            Image img = obj.AddComponent<Image>();
            img.color = color;

            Button btn = obj.AddComponent<Button>();
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(color.r * 1.1f, color.g * 1.1f, color.b * 1.1f);
            colors.pressedColor = new Color(color.r * 0.8f, color.g * 0.8f, color.b * 0.8f);
            btn.colors = colors;

            // Button text
            TextMeshProUGUI text = CreateTMP(obj.transform, "Label", label,
                20, Color.white, TextAlignmentOptions.Center);
            SetAnchors(text.gameObject, Vector2.zero, Vector2.one);

            if (anchorMin.HasValue && anchorMax.HasValue)
            {
                SetAnchors(obj, anchorMin.Value, anchorMax.Value);
            }

            return obj;
        }
    }
}
#endif
