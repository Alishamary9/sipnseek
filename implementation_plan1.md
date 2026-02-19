# خطة تنفيذ مشروع Sip & Seek: Nomad Cafe

خطة شاملة لبناء جميع أنظمة اللعبة استناداً إلى مستندات GDD (الأقسام 1-14) وما تم إنجازه سابقاً.

## المكتمل مسبقاً (الطبقة الأساسية)

| الملف | الحالة |
|-------|--------|
| 7 ScriptableObject Data Classes | ✅ مكتمل |
| `Enums.cs` (5 تعدادات) | ✅ مكتمل |
| `GameDatabase.cs` (Singleton) | ✅ مكتمل |
| `LocalizationManager.cs` | ✅ مكتمل |
| `DataTableGenerator.cs` (Editor) | ✅ مكتمل |
| `localization.csv` (150 مُدخل) | ✅ مكتمل |
| `Test Scene` (Bootstrap & HUD) | ✅ مكتمل |
| **Visual Prompts (GDD Section 3)** | 📋 جاهز للتوليد |
| **HiddenImageData Structure** | 📋 جاهز للتنفيذ |

---

## Phase 1: البنية التحتية (Core Infrastructure)

> [!IMPORTANT]
> هذه المرحلة **إلزامية أولاً** — جميع الأنظمة اللاحقة تعتمد عليها.

### Core Infrastructure

#### [NEW] [GameManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/GameManager.cs)

المسؤولية: مدير اللعبة المركزي — Singleton MonoBehaviour مع `DontDestroyOnLoad`

- **Game States:** `MainMenu`, `Loading`, `Playing`, `Paused`, `LevelComplete`, `LevelFailed`, `Dialogue`
- **Enum جديد:** `GameState` يُضاف إلى `Enums.cs`
- **Events (C# Actions):**
  - `OnGameStateChanged(GameState oldState, GameState newState)`
  - `OnLevelStarted(int levelNumber)`
  - `OnLevelCompleted(int levelNumber, int stars, float completionPercent)`
  - `OnLevelFailed(int levelNumber)`
- **وظائف رئيسية:** `StartLevel()`, `CompleteLevel()`, `FailLevel()`, `PauseGame()`, `ResumeGame()`
- يدير المراجع إلى جميع المديرين الآخرين

#### [NEW] [PlayerData.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Data/PlayerData.cs)

المسؤولية: نموذج بيانات اللاعب (Serializable class — ليس ScriptableObject)

```csharp
[System.Serializable]
public class PlayerData
{
    public int coins;
    public int gems;
    public int currentLevel;
    public int totalXP;
    public int totalMerges;
    public int totalTilesRevealed;
    public List<int> completedLevels;       // بنجوم
    public Dictionary<int, int> levelStars; // level → stars (1-3)
    public List<string> unlockedAchievements;
    public Dictionary<string, int> powerupInventory; // id → count
    public List<string> unlockedSkins;
    public string activeSkin;
    public int consecutiveLoginDays;
    public string lastLoginDate;
    public List<string> completedDailyChallenges;
    public string lastDailyChallengeDate;
}
```

#### [NEW] [PlayerDataManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/PlayerDataManager.cs)

المسؤولية: إدارة بيانات اللاعب مع حفظ/تحميل

- **Singleton MonoBehaviour** مع `DontDestroyOnLoad`
- الحفظ بصيغة **JSON** في `Application.persistentDataPath`
- **وظائف:**
  - `AddCoins(int amount)`, `SpendCoins(int amount) → bool`
  - `AddGems(int amount)`, `SpendGems(int amount) → bool`
  - `SetLevelStars(int level, int stars)`
  - `GetLevelStars(int level) → int`
  - `IsLevelUnlocked(int level) → bool`
  - `AddMergeCount(int count)`, `AddTilesRevealed(int count)`
  - `SaveData()`, `LoadData()`, `ResetData()`
- **Auto-Save:** يحفظ تلقائياً عند كل تغيير مهم
- **Events:** `OnCoinsChanged`, `OnGemsChanged`, `OnLevelCompleted`

#### [NEW] [SaveSystem.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/SaveSystem.cs)

المسؤولية: نظام الحفظ/التحميل العام (JSON serialization)

- `Save<T>(string key, T data)` — يحفظ أي object كـ JSON
- `Load<T>(string key) → T` — يُحمّل ويُعيد deserialized object
- `HasSave(string key) → bool`
- `DeleteSave(string key)`
- `DeleteAllSaves()`
- يستخدم `Application.persistentDataPath` + `JsonUtility`

#### [NEW] [AudioManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/AudioManager.cs)

المسؤولية: مدير الأصوات — SFX + BGM

- **Singleton MonoBehaviour** مع `DontDestroyOnLoad`
- **مكوّنان AudioSource:** واحد لـ BGM (loop)، وآخر لـ SFX
- **وظائف:**
  - `PlaySFX(string sfxName)` — يشغّل من `Resources/Audio/SFX/`
  - `PlayBGM(string bgmName)` — مع fade in/out
  - `StopBGM()`, `PauseBGM()`, `ResumeBGM()`
  - `SetMasterVolume(float)`, `SetSFXVolume(float)`, `SetBGMVolume(float)`
  - `ToggleMute()`
- يحفظ إعدادات الصوت في `PlayerPrefs`

#### [NEW] [SceneLoader.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/SceneLoader.cs)

المسؤولية: تحميل المشاهد مع شاشة تحميل

- **Singleton MonoBehaviour**
- `LoadScene(string sceneName)` — مع transition animation
- `LoadSceneAsync(string sceneName, Action<float> onProgress)` — مع progress callback
- **Scenes المطلوبة:** `MainMenu`, `Gameplay`, `Loading`

---

#### [MODIFY] [Enums.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Enums/Enums.cs)

إضافة تعدادات جديدة:

```diff
+public enum GameState
+{
+    MainMenu,
+    Loading,
+    Playing,
+    Paused,
+    LevelComplete,
+    LevelFailed,
+    Dialogue
+}
+
+public enum MergeDirection
+{
+    None,
+    Up,
+    Down,
+    Left,
+    Right
+}
+
+public enum TileState
+{
+    Empty,
+    Occupied,
+    Revealed,
+    Locked,
+    Frozen,
+    KeyLocked,
+    Dark,
+    Golden
+}
```

---

## Phase 2: أنظمة اللعب الأساسية (Core Gameplay)

> [!IMPORTANT]
> هذه هي أنظمة **قلب اللعبة** — الشبكة، الدمج، الكشف. بدونها لا توجد لعبة قابلة للعب.

### Grid & Merge System

#### [NEW] [GridCell.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Gameplay/GridCell.cs)

المسؤولية: خلية واحدة في الشبكة

- **Properties:** `int Row`, `int Col`, `TileState State`, `MergeItem CurrentItem`, `ObstacleData Obstacle`
- يدير حالة الخلية (فارغة، مشغولة، مكشوفة، مقفلة...)
- يحتوي مرجع للصورة المخفية تحته (tile في الصورة)
- **Visuals:** يُحدّث sprite/color بناءً على الحالة

#### [NEW] [MergeItem.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Gameplay/MergeItem.cs)

المسؤولية: عنصر دمج قابل للسحب

- يرث من `MonoBehaviour` مع `IDragHandler`, `IDropHandler`
- **Properties:** `MergeChainItemData Data`, `GridCell CurrentCell`
- **Drag & Drop:** سحب وإفلات بين الخلايا
- مرجع لبيانات العنصر من `MergeChainItemData`
- Visual feedback أثناء السحب (scale up, shadow)

#### [NEW] [GridManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/GridManager.cs)

المسؤولية: إنشاء وإدارة الشبكة

- يُنشئ شبكة ديناميكية بناءً على حجم المستوى (5×5 إلى 8×8)
- `CreateGrid(int rows, int cols)`
- `GetCell(int row, int col) → GridCell`
- `GetAdjacentCells(GridCell cell) → List<GridCell>` — أفقي وعمودي فقط (بلا أقطار)
- `GetEmptyCells() → List<GridCell>`
- `PlaceItem(GridCell cell, MergeItem item)`
- `RemoveItem(GridCell cell)`
- يدير توزيع العوائق بناءً على بيانات المستوى

#### [NEW] [MergeManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/MergeManager.cs)

المسؤولية: منطق الدمج — GDD Section 2.2

- `TryMerge(GridCell source, GridCell target) → bool`
  - فحص: هل الخليتان **متجاورتان**؟
  - فحص: هل العنصران من **نفس السلسلة ونفس المستوى**؟
  - فحص: هل يوجد **مستوى أعلى** في السلسلة؟
- `ExecuteMerge()`:
  - إزالة العنصرين القديمين
  - إنشاء عنصر بمستوى أعلى في خلية الهدف
  - خلية المصدر تصبح فارغة
  - **فحص العوائق المجاورة** وفتحها إذا تحققت الشروط
  - تحديث عداد الدمج في `PlayerDataManager`
  - **لا يوجد Undo** (كل دمجة نهائية)
- **Events:** `OnMergeSuccess`, `OnMergeFailed`
- **Combo System (Lv15+):** 3 دمجات في 5 ثوانٍ = bonus tile مكشوف

#### [NEW] [ItemGenerator.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Gameplay/ItemGenerator.cs)

المسؤولية: مولّد العناصر — GDD Section 2.3

- يُنتج عناصر Level 1 بناءً على `spawnWeight` من بيانات المستوى
- `GenerateItem() → MergeChainItemData` — اختيار عشوائي مُوزّع
- يراعي السلاسل المتاحة في المستوى الحالي
- **Cooldown:** فترة انتظار بين كل إنتاج (قابلة للتخطي بـ 50 عملة)
- يوضع في حافة الشبكة (أسفل أو جانب)

---

### Reveal System

#### [NEW] [RevealManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/RevealManager.cs)

المسؤولية: نظام كشف الصور المخفية — GDD Section 2.5

- يقسّم الصورة المخفية إلى tiles (3×3 إلى 7×7)
- `RevealTile(GridCell cell)` — يكشف قطعة من الصورة
- `CalculateProgress() → float` — نسبة الإنجاز (0-100%)
- `CalculateStars() → int` — (80%=1⭐, 90%=2⭐, 100%=3⭐)
- **Reveal Logic:**
  - الدمج المجاور يكشف الخلايا المجاورة (Progressive Adjacent)
  - خلايا الحواف قابلة للكشف أولاً
- **VFX Events:** `OnTileRevealed`, `OnImageComplete`
- يعمل مع صور من `Resources/Data/HiddenImages/`

#### [NEW] [ObstacleManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/ObstacleManager.cs)

المسؤولية: إدارة العوائق — GDD Section 4

- `PlaceObstacles(LevelConfig config)` — توزيع العوائق على الشبكة
- `CheckObstacleUnlock(GridCell cell, int mergeLevel)` — فحص شروط الفتح لكل نوع:
  - **Locked:** دمج Lv3+ مجاور
  - **Frozen:** دمجتان متجاورتان (مرحلتين: جليد → مقفل → مكشوف)
  - **Key Lock:** يتطلب عنصر أدوات (مفتاح)
  - **Dark:** دمج عنصر ضوء مجاور
  - **Golden:** كشف عادي + bonus مكافأة
- **Events:** `OnObstacleUnlocked(GridCell cell, ObstacleType type)`

---

### Level System

#### [NEW] [LevelConfig.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Data/LevelConfig.cs)

المسؤولية: بيانات تكوين المستوى (ScriptableObject)

```csharp
public class LevelConfig : ScriptableObject
{
    public int levelNumber;
    public int gridRows, gridCols;
    public int imageGridRows, imageGridCols;  // حجم الصورة المخفية
    public Difficulty difficulty;
    public int moveLimit;       // -1 = unlimited
    public float targetPercent; // 80% للمرور
    public Sprite hiddenImage;
    public List<string> availableChains;      // سلاسل الدمج المتاحة
    public int lockedTiles, frozenTiles, keyLockTiles, darkTiles, goldenTiles;
    public string narrativeId;
}
```

#### [NEW] [LevelManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/LevelManager.cs)

المسؤولية: إدارة تدفق المستوى

- `LoadLevel(int levelNumber)` — يُحمّل `LevelConfig` ويُعد الشبكة
- `StartLevel()`, `EndLevel()`, `RetryLevel()`
- يراقب التقدم (نسبة الكشف، الحركات المتبقية)
- يحسب النجوم والمكافآت عند الإكمال
- **Move Tracking:** عداد حركات (من Lv20+)
- **Events:** `OnMoveUsed`, `OnProgressUpdated(float percent)`

#### [NEW] [LevelConfigGenerator.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Editor/LevelConfigGenerator.cs)

المسؤولية: أداة محرر لتوليد `LevelConfig` assets لجميع المستويات

- يستخدم جدول صعوبة GDD Section 5.2
- يُنتج 50 ملف `LevelConfig` في `Resources/Data/Levels/`

---

## Phase 3: الأنظمة الثانوية (Secondary Systems)

### Economy & Powerups

#### [NEW] [EconomyManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/EconomyManager.cs)

المسؤولية: Managing the game economy as per GDD Section 7.
- Integration: Use **Odin Inspector** to create a visual balance dashboard for the developer to tune coins/gems.

#### [NEW] [PowerupManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/PowerupManager.cs)

المسؤولية: Powerup system as per GDD Section 14.4.
- Integration: Use **Fantasy Sounds Bundle** for specific feedback for each powerup (Fog Clearer, Hammer, Bomb).

---

### Achievement & Challenge Systems

#### [NEW] [AchievementManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/AchievementManager.cs)

المسؤولية: نظام الإنجازات — GDD Section 14.6

- `CheckAchievements()` — يفحص جميع الشروط بعد كل حدث
- يستمع إلى events من `PlayerDataManager`, `MergeManager`, `RevealManager`
- `ClaimReward(string achId)` — يمنح المكافأة
- **Events:** `OnAchievementUnlocked(AchievementData)`

#### [NEW] [DailyChallengeManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/DailyChallengeManager.cs)

المسؤولية: التحديات اليومية — GDD Section 14.7

- `GenerateDailyChallenges()` — يختار 3 تحديات عشوائية يومياً
- `UpdateProgress(ChallengeType type, int amount)`
- `ClaimReward(string challengeId)`
- يتحقق من التاريخ لإعادة التوليد يومياً
- **Events:** `OnChallengeCompleted`, `OnChallengeProgress`

---

### Narrative System

#### [NEW] [DialogueData.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Data/DialogueData.cs)

المسؤولية: نموذج بيانات الحوار (ScriptableObject)

```csharp
public class DialogueData : ScriptableObject
{
    public int level;
    public string dialogueType; // "Pre" or "Post"
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    public string character;   // "Laith" or "Grandma"
    public string emotion;     // "Curious", "Warm", "Surprised"...
    public string textKey;     // Key from localization CSV
}
```

#### [NEW] [DialogueManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/DialogueManager.cs)

المسؤولية: عرض الحوارات — GDD Section 9

- `ShowDialogue(DialogueData data)` — يعرض الحوار خطوة بخطوة
- يستخدم `LocalizationManager` للنصوص المترجمة
- أيقونات شخصيات مع تعبيرات عاطفية
- زر **[التالي]** و **[تخطي]**
- **Events:** `OnDialogueStarted`, `OnDialogueEnded`

#### [NEW] [NarrativeManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/NarrativeManager.cs)

المسؤولية: السرد القصصي بين المستويات

- `ShowPreLevelNarrative(int level)` — نص سردي قبل المستوى
- `ShowPostLevelNarrative(int level)` — نص سردي بعد المستوى
- يستخدم مفاتيح `nar_lv{X}_before/after` من CSV

#### [NEW] [TutorialManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/Managers/TutorialManager.cs)

المسؤولية: نظام التعليم التفاعلي — GDD Section 4.4

- يوجّه اللاعب بخطوات مرئية (hand pointer)
- تعليمات محددة لكل مستوى (Lv2: القفل، Lv3: الجليد، Lv5: الأدوات)
- `StartTutorial(int level)`, `NextStep()`, `CompleteTutorial()`
- يستخدم نصوص التعليم من `localization.csv`

---

## Phase 4: واجهة المستخدم (UI/UX)

> [!NOTE]
> جميع شاشات UI تدعم **العربية RTL** و **الإنجليزية LTR** عبر `LocalizationManager`.
> الألوان من GDD Section 11: Coffee Brown `#8D6E63`, Cream Beige `#D7CCC8`, Gold `#FFD54F`, Turquoise `#4DB6AC`, Sunset Orange `#FF7043`.

### UI System

#### [NEW] [UIManager.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/UIManager.cs)

المسؤولية: مدير الشاشات المركزي

- `ShowScreen(string screenName)`, `HideScreen(string screenName)`
- `ShowPopup(string popupName)`, `HidePopup()`
- يدير stack الشاشات (push/pop)
- يستمع لـ `GameManager.OnGameStateChanged`

#### [NEW] [MainMenuUI.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/MainMenuUI.cs)

- أزرار: Play, Settings, Shop, Passport, Album, Achievements, Daily Challenges
- عرض العملات (Coins + Gems) في الأعلى
- اسم اللعبة مع animation

#### [NEW] [GameplayHUD.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/GameplayHUD.cs)

- عرض: مستوى، نجوم، تقدم (progress bar)، عملات
- حركات متبقية (Lv20+)
- أزرار الأدوات المساعدة (أسفل الشاشة)
- زر إيقاف مؤقت

#### [NEW] [LevelCompleteUI.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/LevelCompleteUI.cs)

- نجوم متحركة (1-3)
- ملخص المكافآت (عملات، جواهر، عناصر)
- أزرار: التالي، إعادة، القائمة
- رسالة إكمال من `localization.csv`

#### [NEW] [ShopUI.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/ShopUI.cs)

- عرض الأدوات المساعدة والأسعار
- حزم العملات (Starter, Small, Medium, Large, Mega)
- العروض الخاصة (Welcome Bundle, Weekend Bundle)
- Premium Pass

#### [NEW] [SettingsUI.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/SettingsUI.cs)

- تبديل اللغة (عربي/إنجليزي)
- مستوى الصوت (BGM + SFX)
- إيقاف/تشغيل الاهتزاز
- رابط سياسة الخصوصية

#### [NEW] [LocalizedText.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/LocalizedText.cs)

المسؤولية: مكوّن UI يربط `Text/TMP_Text` بمفتاح توطين

- يوضع على أي عنصر نصي
- يُحدّث النص تلقائياً عند تغيير اللغة
- يدعم RTL للعربية (عكس اتجاه النص)

#### [NEW] [DialogueUI.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/DialogueUI.cs)

- Integration: Use **NodeCanvas** state machines to trigger dialogue sequences at specific game events.
- Visuals: Use **GUI Pro - Fantasy RPG** dialogue bubble templates.

#### [NEW] [PassportUI.cs](file:///e:/game_test/SipAndSeek/Assets/Scripts/UI/PassportUI.cs)

- Integration: Use **Dreamteck Splines** to animate the "Travel Path" across the world map as the user unlocks countries.

---

## ملخص الملفات الجديدة

| المرحلة | عدد الملفات | المجلد |
|---------|-------------|--------|
| Phase 1 | 5 ملفات + تعديل 1 | `Managers/`, `Data/` |
| Phase 2 | 8 ملفات | `Gameplay/`, `Managers/`, `Data/`, `Editor/` |
| Phase 3 | 7 ملفات | `Managers/`, `Data/` |
| Phase 4 | 7 ملفات | `UI/` |
| **المجموع** | **27 ملف جديد + 1 تعديل** | |

---

## هيكل المجلدات النهائي

```
SipAndSeek/Assets/Scripts/
├── Data/
│   ├── AchievementData.cs          ✅ موجود
│   ├── DailyChallengeData.cs       ✅ موجود
│   ├── DialogueData.cs             🆕
│   ├── HiddenImageData.cs          ✅ موجود
│   ├── LevelConfig.cs              🆕
│   ├── LevelRewardData.cs          ✅ موجود
│   ├── MergeChainItemData.cs       ✅ موجود
│   ├── ObstacleData.cs             ✅ موجود
│   ├── PlayerData.cs               🆕
│   └── PowerupData.cs              ✅ موجود
├── Editor/
│   ├── DataTableGenerator.cs       ✅ موجود
│   └── LevelConfigGenerator.cs     🆕
├── Enums/
│   └── Enums.cs                    ✏️ تعديل
├── Gameplay/
│   ├── GridCell.cs                  🆕
│   ├── ItemGenerator.cs            🆕
│   └── MergeItem.cs                🆕
├── Managers/
│   ├── AchievementManager.cs       🆕
│   ├── AudioManager.cs             🆕
│   ├── DailyChallengeManager.cs    🆕
│   ├── DialogueManager.cs          🆕
│   ├── EconomyManager.cs           🆕
│   ├── GameDatabase.cs             ✅ موجود
│   ├── GameManager.cs              🆕
│   ├── GridManager.cs              🆕
│   ├── LevelManager.cs             🆕
│   ├── LocalizationManager.cs      ✅ موجود
│   ├── MergeManager.cs             🆕
│   ├── NarrativeManager.cs         🆕
│   ├── ObstacleManager.cs          🆕
│   ├── PlayerDataManager.cs        🆕
│   ├── PowerupManager.cs           🆕
│   ├── RevealManager.cs            🆕
│   ├── SceneLoader.cs              🆕
│   └── TutorialManager.cs          🆕
└── UI/
    ├── DialogueUI.cs               🆕
    ├── GameplayHUD.cs              🆕
    ├── LevelCompleteUI.cs          🆕
    ├── LocalizedText.cs            🆕
    ├── MainMenuUI.cs               🆕
    ├── SettingsUI.cs               🆕
    ├── ShopUI.cs                   🆕
    └── UIManager.cs                🆕
```

---

## Phase 5: التطوير البصري والتحسينات (Visual Arts & Juice)

> [!TIP]
> هذه المرحلة تهدف لنقل اللعبة من "قابلة للعب" إلى "تجربة ممتعة وجذابة".

### 1. توليد الأصول البصرية (Asset Generation)
استخدام الذكاء الاصطناعي لتوليد الصور بناءً على الأوصاف في GDD:
- **Hidden Images:** صور 1024x1024 لكل مستوى (المقهى، الرف، النافذة).
- **Item Icons:** أيقونات سلاسل الدمج (حبوب القهوة، أوراق الشاي، أدوات السفر).
- **Characters:** صور تعبيرية لـ "ليث" والجدة بوضعيات مختلفة (سعيد، مفاجئ، حزين).

### 2. تحسينات الـ Visual Juice
- **Merge Animation:** إضافة Squash & Stretch عند اصطدام العناصر.
- **Particle Systems:** شرارات ذهبية عند نجوم الإكمال و "غبار" عند تنظيف الأرفف.
- **Dynamic Elements:** تحريك بسيط (Loop) داخل الصور المكشوفة (بخار الشاي، حركة الستائر).

### 3. ميزات الميتا جيم (Meta-Game Layer)
- **Cafe Decoration:** نظام لوضع الأثاث المفتوح في المقهى الرئيسي.
- **Passport Album:** واجهة لعرض الصور المكتملة كطوابع بريد.

---

## Verification Plan

### Unity Editor Tests
1. **Compilation Check:** فتح المشروع في Unity — يجب أن يُترجم بدون أخطاء.
2. **Editor Tool Test:** تشغيل `Tools > Sip & Seek > Generate All Data`.
3. **Asset Validation:** التأكد من أن الصور المولدة بدقة 1024x1024 وبصيغة PNG.

### Manual Playtesting
- **Juice Perception:** هل يشعر اللاعب بالمتعة عند الدمج؟
- **Localization Sync:** هل تظهر النصوص العربية باتجاه RTL وبخط متوافق؟
- **Progression:** هل يتم حفظ التقدم في الصور المكتملة في الـ Passport؟

---

## User Review Required

> [!IMPORTANT]
> **توليد الصور:** سيتم توليد الصور فور توفر سعة السيرفر.
> **أولوية التنفيذ:** هل نبدأ ببرمجة الـ UI (Phase 4) أم نركز على تحسينات الـ Juice أولاً؟
