using UnityEngine;
using SipAndSeek;
using SipAndSeek.Managers;
using SipAndSeek.Gameplay;
using SipAndSeek.Data;
using SipAndSeek.VFX;
using UnityEngine.EventSystems;

namespace SipAndSeek.Testing
{
    /// <summary>
    /// Test Scene bootstrap — auto-creates all managers and starts Level 1.
    /// Attach this to a GameObject in the scene and press Play.
    /// </summary>
    public class TestSceneSetup : MonoBehaviour
    {
        [Header("Test Config")]
        [SerializeField] private int _testLevel = 1;
        [SerializeField] private bool _autoStart = true;

        [Header("Camera Setup")]
        [SerializeField] private float _cameraSize = 5f;

        private void Start()
        {
            Debug.Log("========================================");
            Debug.Log("  🧪 TEST SCENE — Sip & Seek");
            Debug.Log("========================================");

            // Step 0: Ensure Input System (EventSystem + Raycaster)
            SetupInput();

            // Step 1: Setup Camera
            SetupCamera();

            // Step 2: Create all required managers
            CreateManagers();

            // Step 3: Generate data tables if not already done
            EnsureGameDatabase();

            // Step 4: Auto-start level if enabled
            if (_autoStart)
            {
                Invoke(nameof(StartTestLevel), 0.5f);
            }
        }

        private void SetupInput()
        {
            // EventSystem is still needed for UI (Canvas) elements
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                Debug.Log("[TestScene] 🖱️ Created EventSystem");
            }

            // Physics2DRaycaster on camera is critical for OnMouseDown to work
            // on BoxCollider2D sprites
            Camera cam = Camera.main;
            if (cam != null && cam.GetComponent<Physics2DRaycaster>() == null)
            {
                cam.gameObject.AddComponent<Physics2DRaycaster>();
                Debug.Log("[TestScene] 🖱️ Added Physics2DRaycaster to Main Camera");
            }
        }

        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = _cameraSize;
                cam.transform.position = new Vector3(0, 0, -10);
                cam.backgroundColor = new Color(0.95f, 0.92f, 0.88f); // Warm sand bg

                Debug.Log("[TestScene] 📷 Camera configured (ortho, size=" + _cameraSize + ")");
            }
        }

        private void CreateManagers()
        {
            GameObject managersObj = new GameObject("=== MANAGERS ===");

            // Core managers
            EnsureManager<GameManager>(managersObj);
            EnsureManager<PlayerDataManager>(managersObj);
            EnsureManager<AudioManager>(managersObj);
            EnsureManager<SceneLoader>(managersObj);

            // Gameplay managers
            EnsureManager<GridManager>(managersObj);
            EnsureManager<MergeManager>(managersObj);
            EnsureManager<RevealManager>(managersObj);
            EnsureManager<ObstacleManager>(managersObj);
            EnsureManager<LevelManager>(managersObj);

            // Item generator
            EnsureManager<ItemGenerator>(managersObj);

            // Secondary systems
            EnsureManager<EconomyManager>(managersObj);
            EnsureManager<PowerupManager>(managersObj);
            EnsureManager<AchievementManager>(managersObj);
            EnsureManager<DailyChallengeManager>(managersObj);
            EnsureManager<NarrativeManager>(managersObj);
            EnsureManager<TutorialManager>(managersObj);

            // VFX
            EnsureManager<MergeVFXManager>(managersObj);

            Debug.Log("[TestScene] ✅ All managers created");
        }

        private T EnsureManager<T>(GameObject parent) where T : Component
        {
            T existing = FindFirstObjectByType<T>();
            if (existing != null) return existing;

            T manager = parent.AddComponent<T>();
            Debug.Log($"[TestScene]   + {typeof(T).Name}");
            return manager;
        }

        private void EnsureGameDatabase()
        {
            if (GameDatabase.Instance == null)
            {
                Debug.LogWarning("[TestScene] ⚠️ GameDatabase not found in Resources! " +
                    "Run 'Tools → Sip & Seek → Generate Data Tables' first.");
            }
            else
            {
                GameDatabase db = GameDatabase.Instance;

                // We trust the GameDatabase has been set up correctly in the Editor via AssetAutoImporter.
                // We no longer attempt to force-load from Resources at runtime to avoid wiping new assets
                // that aren't located in the exact 'Resources/Data' folders yet.

                Debug.Log($"[TestScene] 📊 GameDatabase loaded/verified: " +
                    $"{db.mergeChainItems.Count} merge items");
            }
        }

        private void StartTestLevel()
        {
            LevelManager levelMgr = FindFirstObjectByType<LevelManager>();
            if (levelMgr == null)
            {
                Debug.LogError("[TestScene] LevelManager not found!");
                return;
            }

            string configPath = $"Data/LevelConfigs/Level_{_testLevel}";
            LevelConfig config = Resources.Load<LevelConfig>(configPath);

            if (config != null)
            {
                Debug.Log($"[TestScene] 🎮 Starting Level {_testLevel} from config");
                levelMgr.SetupLevel(config);
            }
            else
            {
                Debug.LogWarning($"[TestScene] ⚠️ No LevelConfig at Resources/{configPath}");
                Debug.Log("[TestScene] 🔧 Creating inline test level...");
                StartInlineTestLevel(levelMgr);
            }
        }

        /// <summary>
        /// Creates a simple test level without needing LevelConfig assets.
        /// </summary>
        private void StartInlineTestLevel(LevelManager levelMgr)
        {
            LevelConfig testConfig = ScriptableObject.CreateInstance<LevelConfig>();
            testConfig.levelNumber = _testLevel;
            testConfig.difficulty = Difficulty.VeryEasy;
            testConfig.gridRows = 5;
            testConfig.gridCols = 5;
            testConfig.imageGridRows = 3;
            testConfig.imageGridCols = 3;
            testConfig.targetPercent = 0.8f;
            testConfig.moveLimit = -1;
            testConfig.availableChains = new System.Collections.Generic.List<string> { "coffee", "travel" };
            testConfig.lockedTiles = 0;
            testConfig.frozenTiles = 0;
            testConfig.keyLockTiles = 0;
            testConfig.darkTiles = 0;
            testConfig.goldenTiles = 0;

            levelMgr.SetupLevel(testConfig);
        }

        // ===============================
        // Manual Controls
        // ===============================

        [ContextMenu("Start Level")]
        public void ManualStartLevel()
        {
            StartTestLevel();
        }

        [ContextMenu("Spawn Random Item")]
        public void SpawnRandomItem()
        {
            ItemGenerator gen = FindFirstObjectByType<ItemGenerator>();
            if (gen != null)
            {
                MergeItem item = gen.GenerateAndSpawn();
                if (item != null)
                    Debug.Log($"[TestScene] Spawned: {item.ChainId} Lv{item.Level}");
                else
                    Debug.Log("[TestScene] No item spawned (cooldown or no empty cell)");
            }
        }

        [ContextMenu("Reveal Next Tile")]
        public void RevealNextTile()
        {
            if (RevealManager.Instance != null)
            {
                RevealManager.Instance.RevealNextTile();
            }
        }

        [ContextMenu("Add 100 Coins")]
        public void AddTestCoins()
        {
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.AddCoins(100);
                Debug.Log($"[TestScene] 🪙 Coins: {PlayerDataManager.Instance.Coins}");
            }
        }

        // ===============================
        // Debug HUD (OnGUI)
        // ===============================
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.normal.textColor = Color.white;

            GUIStyle shadowStyle = new GUIStyle(style);
            shadowStyle.normal.textColor = new Color(0, 0, 0, 0.6f);

            float y = 10;

            // Shadow + text for readability
            DrawLabel(10, y, "=== Sip & Seek Test Scene ===", style, shadowStyle);
            y += 25;

            if (GameManager.Instance != null)
            {
                DrawLabel(10, y, $"State: {GameManager.Instance.CurrentState}", style, shadowStyle);
                y += 22;
            }

            if (PlayerDataManager.Instance != null)
            {
                DrawLabel(10, y,
                    $"🪙 {PlayerDataManager.Instance.Coins}  |  💎 {PlayerDataManager.Instance.Gems}",
                    style, shadowStyle);
                y += 22;
            }

            if (MergeManager.Instance != null)
            {
                DrawLabel(10, y,
                    $"Merges: {MergeManager.Instance.TotalMergesThisLevel}  |  Next Reveal: {MergeManager.Instance.MergesUntilReveal}",
                    style, shadowStyle);
                y += 22;
            }

            if (RevealManager.Instance != null)
            {
                DrawLabel(10, y,
                    $"Revealed: {RevealManager.Instance.RevealedCount}/{RevealManager.Instance.TotalImageTiles} ({RevealManager.Instance.RevealProgress * 100:F0}%)",
                    style, shadowStyle);
                y += 22;
            }

            if (ObstacleManager.Instance != null)
            {
                DrawLabel(10, y,
                    $"Obstacles: {ObstacleManager.Instance.ClearedObstacles}/{ObstacleManager.Instance.TotalObstacles}",
                    style, shadowStyle);
                y += 30;
            }
            else
            {
                y += 30;
            }

            // Buttons
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 14;

            if (GUI.Button(new Rect(10, y, 150, 35), "🎲 Spawn Item", btnStyle))
                SpawnRandomItem();
            if (GUI.Button(new Rect(170, y, 150, 35), "🔍 Reveal Tile", btnStyle))
                RevealNextTile();
            if (GUI.Button(new Rect(330, y, 150, 35), "🪙 +100 Coins", btnStyle))
                AddTestCoins();
        }

        private void DrawLabel(float x, float y, string text, GUIStyle style, GUIStyle shadowStyle)
        {
            GUI.Label(new Rect(x + 1, y + 1, 500, 25), text, shadowStyle);
            GUI.Label(new Rect(x, y, 500, 25), text, style);
        }
    }
}
