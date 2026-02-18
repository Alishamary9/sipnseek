using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using SipAndSeek;
using SipAndSeek.Data;

namespace SipAndSeek.Editor
{
    public class DataTableGenerator : EditorWindow
    {
        [MenuItem("Tools/Sip & Seek/Generate All Data")]
        public static void GenerateAllData()
        {
            GenerateMergeChains();
            GenerateLevelRewards();
            GenerateObstacles();
            GeneratePowerups();
            GenerateHiddenImages();
            GenerateAchievements();
            GenerateDailyChallenges();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("🎉 All data generated successfully!");
        }

        private static void CreateAsset<T>(T asset, string path) where T : ScriptableObject
        {
            string fullPath = "Assets/Resources/Data/" + path;
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(asset, fullPath);
        }

        private static void GenerateMergeChains()
        {
            // 14.1 Merge Chains Table
            // Coffee Chain
            CreateMergeItem("coffee", 1, "Coffee Bean", "حبة بن", "Single brown coffee bean, shiny and round", ItemRarity.Common, 1, 0.5f);
            CreateMergeItem("coffee", 2, "Bean Pile", "كومة حبوب", "Small pile of 3 roasted coffee beans", ItemRarity.Common, 2, 0.3f);
            CreateMergeItem("coffee", 3, "Ground Coffee", "بن مطحون", "Wooden bowl filled with fine brown powder", ItemRarity.Uncommon, 5, 0.15f);
            CreateMergeItem("coffee", 4, "Coffee Cup", "فنجان قهوة", "Steaming decorated ceramic coffee cup", ItemRarity.Uncommon, 10, 0.05f);
            CreateMergeItem("coffee", 5, "Coffee Pot", "دلّة قهوة", "Polished brass Arabic coffee pot with engraving", ItemRarity.Rare, 25, 0f);

            // Tea Chain
            CreateMergeItem("tea", 1, "Tea Leaf", "ورقة شاي", "Single green tea leaf, fresh", ItemRarity.Common, 1, 0.5f);
            CreateMergeItem("tea", 2, "Tea Bundle", "حزمة شاي", "Small bundle of dried tea leaves tied with twine", ItemRarity.Common, 2, 0.3f);
            CreateMergeItem("tea", 3, "Tea Bag", "كيس شاي", "Cute fabric tea bag with a tag", ItemRarity.Uncommon, 5, 0.15f);
            CreateMergeItem("tea", 4, "Tea Glass", "كأس شاي", "Golden-rimmed glass cup of amber tea with steam", ItemRarity.Uncommon, 10, 0.05f);
            CreateMergeItem("tea", 5, "Tea Set", "طقم شاي", "Complete ornate tea set on small brass tray", ItemRarity.Rare, 25, 0f);

            // Travel Chain
            CreateMergeItem("travel", 1, "Torn Paper", "ورقة ممزقة", "Small torn piece of yellowed paper", ItemRarity.Common, 1, 0.4f);
            CreateMergeItem("travel", 2, "Paper Scraps", "قصاصات ورق", "Several paper scraps pieced together", ItemRarity.Common, 2, 0.25f);
            CreateMergeItem("travel", 3, "Old Letter", "رسالة قديمة", "Folded letter with faded handwriting and wax seal", ItemRarity.Uncommon, 5, 0.15f);
            CreateMergeItem("travel", 4, "Envelope Bundle", "حزمة رسائل", "Bundle of envelopes tied with ribbon", ItemRarity.Uncommon, 10, 0.05f);
            CreateMergeItem("travel", 5, "Treasure Map", "خريطة كنز", "Hand-drawn world map with marked routes", ItemRarity.Rare, 25, 0f);
            CreateMergeItem("travel", 6, "Explorer's Journal", "دفتر المستكشف", "Leather-bound journal with maps, sketches, pressed flowers", ItemRarity.Epic, 50, 0f);

            // Tools Chain
            CreateMergeItem("tools", 1, "Fabric Scrap", "قطعة قماش", "Small beige fabric scrap", ItemRarity.Common, 1, 0.4f);
            CreateMergeItem("tools", 2, "Rope", "حبل", "Coiled brown rope", ItemRarity.Common, 2, 0.25f);
            CreateMergeItem("tools", 3, "Magnifying Glass", "عدسة مكبرة", "Brass-framed magnifying glass", ItemRarity.Uncommon, 5, 0.15f);
            CreateMergeItem("tools", 4, "Excavation Brush", "فرشاة حفريات", "Small artifact-cleaning brush", ItemRarity.Uncommon, 10, 0.05f);
            CreateMergeItem("tools", 5, "Chisel", "أزميل", "Small chisel with wooden handle", ItemRarity.Rare, 25, 0f);
            CreateMergeItem("tools", 6, "Explorer's Bag", "حقيبة مستكشف", "Leather bag containing all tools", ItemRarity.Epic, 50, 0f);
        }

        private static void CreateMergeItem(string chainId, int level, string en, string ar, string desc, ItemRarity rarity, int price, float weight)
        {
            MergeChainItemData asset = ScriptableObject.CreateInstance<MergeChainItemData>();
            asset.chainId = chainId;
            asset.level = level;
            asset.itemNameEN = en;
            asset.itemNameAR = ar;
            asset.visualDescription = desc;
            asset.rarity = rarity;
            asset.sellPrice = price;
            asset.spawnWeight = weight;
            CreateAsset(asset, $"MergeChains/{chainId}_{level}.asset");
        }

        private static void GenerateLevelRewards()
        {
            CreateLevelReward(1, 50, 75, 100, 5, "First Coffee Seed", 100, "-", "nar_001");
            CreateLevelReward(2, 75, 110, 150, 7, "Old Map (Part 1/3)", 150, "Shop", "nar_002");
            CreateLevelReward(3, 100, 150, 200, 10, "Golden Key", 200, "Passport", "nar_003");
            CreateLevelReward(4, 125, 180, 250, 12, "-", 250, "-", "nar_004");
            CreateLevelReward(5, 175, 245, 350, 14, "First Tool", 350, "Tools Chain", "nar_005");
            CreateLevelReward(6, 200, 280, 400, 15, "-", 400, "-", "nar_006");
            CreateLevelReward(7, 225, 315, 450, 16, "-", 450, "-", "nar_007");
            CreateLevelReward(8, 250, 350, 500, 17, "-", 500, "-", "nar_008");
            CreateLevelReward(9, 275, 385, 550, 18, "-", 550, "-", "nar_009");
            CreateLevelReward(10, 300, 420, 600, 50, "Exclusive Skin", 600, "Mini-Games", "nar_010");
            CreateLevelReward(15, 425, 595, 850, 23, "Lantern Item", 850, "Dark Tiles", "nar_015");
            CreateLevelReward(20, 550, 770, 1100, 100, "Strategist Title", 1100, "Move Limits", "nar_020");
            CreateLevelReward(25, 675, 945, 1350, 100, "Rare Card", 1350, "-", "nar_025");
            CreateLevelReward(30, 800, 1120, 1600, 100, "Bomb x5", 1600, "Chain Reactions", "nar_030");
            CreateLevelReward(40, 1050, 1470, 2100, 100, "Legendary Skin", 2100, "Element Evo", "nar_040");
            CreateLevelReward(50, 1300, 1820, 2600, 500, "Master Explorer Title + Legendary Skin", 2600, "Arc 2 Tease", "nar_050");
        }

        private static void CreateLevelReward(int level, int c80, int c90, int c100, int gems, string item, int xp, string feature, string nar)
        {
            LevelRewardData asset = ScriptableObject.CreateInstance<LevelRewardData>();
            asset.level = level;
            asset.coins80 = c80;
            asset.coins90 = c90;
            asset.coins100 = c100;
            asset.gems100 = gems;
            asset.itemReward = item;
            asset.xp = xp;
            asset.unlocksFeature = feature;
            asset.narrativeId = nar;
            CreateAsset(asset, $"LevelRewards/Level_{level}.asset");
        }

        private static void GenerateObstacles()
        {
            CreateObstacle("obs_lock", "Locked Tile", "مربع مقفل", "Wooden crate with iron lock", "Merge Lv3+ adjacent", "Wooden crate with iron lock, chains", "Lock breaking animation, chains falling", "sfx_wood_break");
            CreateObstacle("obs_ice", "Frozen Tile", "مربع مجمد", "Transparent blue ice layer", "Merge x2 adjacent", "Thick blue ice layer with frost patterns", "Ice cracking, shattering", "sfx_ice_crack");
            CreateObstacle("obs_keylock", "Key Lock Tile", "مربع مفتاح", "Special keyhole marking", "Requires Tool (Key) item", "Ornate keyhole with golden trim", "Key turning, door opening glow", "sfx_key_turn");
            CreateObstacle("obs_dark", "Dark Tile", "مربع مظلم", "Completely dark tile", "Merge light item adjacent", "Solid black with shadow wisps", "Light spreading outward", "sfx_light_burst");
            CreateObstacle("obs_gold", "Golden Tile", "مربع ذهبي", "Glowing golden tile (bonus)", "Normal reveal", "Pulsing golden glow", "Golden burst with coins", "sfx_gold_chime");
        }

        private static void CreateObstacle(string id, string en, string ar, string desc, string unlock, string v1, string v2, string sfx)
        {
            ObstacleData asset = ScriptableObject.CreateInstance<ObstacleData>();
            asset.obstacleId = id;
            asset.nameEN = en;
            asset.nameAR = ar;
            asset.description = desc;
            asset.unlockCondition = unlock;
            asset.visualState1 = v1;
            asset.visualState2 = v2;
            asset.soundEffect = sfx;
            CreateAsset(asset, $"Obstacles/{id}.asset");
        }

        private static void GeneratePowerups()
        {
            CreatePowerup("pu_fog", "Fog Clearer", "كاشف الضباب", "Reveals 3–5 random tiles instantly", 200, 10, 10, "icon_fog");
            CreatePowerup("pu_hammer", "Golden Hammer", "المطرقة الذهبية", "Opens any locked or frozen tile", 300, 15, 5, "icon_hammer");
            CreatePowerup("pu_bomb", "Bomb", "القنبلة", "Reveals all adjacent tiles in radius", 400, 20, 3, "icon_bomb");
            CreatePowerup("pu_hint", "Hint", "تلميح", "Highlights a mergeable pair", 100, 5, 10, "icon_bulb");
            CreatePowerup("pu_time", "Time Extender", "وقت إضافي", "Adds 10 moves", 150, 8, 5, "icon_clock");
            CreatePowerup("pu_vision", "Vision", "الرؤية", "3-second peek at complete image", 100, 5, 5, "icon_eye");
        }

        private static void CreatePowerup(string id, string en, string ar, string desc, int coins, int gems, int max, string icon)
        {
            PowerupData asset = ScriptableObject.CreateInstance<PowerupData>();
            asset.powerupId = id;
            asset.nameEN = en;
            asset.nameAR = ar;
            asset.effectDescription = desc;
            asset.coinPrice = coins;
            asset.gemPrice = gems;
            asset.maxHold = max;
            asset.iconRef = icon;
            CreateAsset(asset, $"Powerups/{id}.asset");
        }

        private static void GenerateHiddenImages()
        {
            CreateHiddenImage("img_001", 1, "Tea Corner", "ركن القهوة", "Nomad Cafe", "Cozy café corner...", "3x3", Difficulty.VeryEasy, "reward_lv1");
            CreateHiddenImage("img_002", 2, "Old Shelf", "الرف القديم", "Nomad Cafe", "Dusty wooden shelf...", "3x3", Difficulty.Easy, "reward_lv2");
            CreateHiddenImage("img_003", 3, "Street Window", "نافذة الشارع", "Nomad Cafe", "Ornate café window...", "4x3", Difficulty.MediumEasy, "reward_lv3");
        }

        private static void CreateHiddenImage(string id, int level, string en, string ar, string theme, string prompt, string grid, Difficulty diff, string reward)
        {
            HiddenImageData asset = ScriptableObject.CreateInstance<HiddenImageData>();
            asset.imageId = id;
            asset.level = level;
            asset.nameEN = en;
            asset.nameAR = ar;
            asset.theme = theme;
            asset.artistPrompt = prompt;
            asset.gridSize = grid;
            asset.difficulty = diff;
            asset.rewardId = reward;
            CreateAsset(asset, $"HiddenImages/{id}.asset");
        }

        private static void GenerateAchievements()
        {
            CreateAchievement("ach_begin", "The Beginning", "البداية", "Complete Level 1", "Complete Lv1", RewardType.Coins, 100, "icon_flag");
            CreateAchievement("ach_saver", "The Saver", "المقتصد", "Collect 1,000 Coins", "Coins >= 1000", RewardType.Gems, 10, "icon_piggy");
            CreateAchievement("ach_merge10", "Merge Master", "سيد الدمج", "Merge 100 items total", "Total merges >= 100", RewardType.Coins, 200, "icon_merge");
            CreateAchievement("ach_reveal", "The Revealer", "الكاشف", "Reveal 10 complete images", "100% images >= 10", RewardType.Gems, 50, "icon_picture");
            CreateAchievement("ach_speed", "Speed Runner", "العداء السريع", "Complete any level under 2 min", "Any level < 120s", RewardType.Gems, 20, "icon_timer");
            CreateAchievement("ach_perfect", "Perfectionist", "المثالي", "Get 3 stars on 10 levels", "3-star levels >= 10", RewardType.Gems, 100, "icon_star3");
            CreateAchievement("ach_collector", "Collector", "الجامع", "Fill 1 album page", "Album page complete", RewardType.Coins, 500, "icon_album"); // Value simplified
            CreateAchievement("ach_explorer", "Master Explorer", "المستكشف الأعظم", "Complete all 50 levels", "All levels 100%", RewardType.Title, 0, "icon_crown");
        }

        private static void CreateAchievement(string id, string en, string ar, string desc, string cond, RewardType type, int val, string icon)
        {
            AchievementData asset = ScriptableObject.CreateInstance<AchievementData>();
            asset.achId = id;
            asset.nameEN = en;
            asset.nameAR = ar;
            asset.description = desc;
            asset.condition = cond;
            asset.rewardType = type;
            asset.rewardValue = val;
            asset.icon = icon;
            CreateAsset(asset, $"Achievements/{id}.asset");
        }

        private static void GenerateDailyChallenges()
        {
            CreateDailyChallenge("daily_merge", ChallengeType.Merge, "Merge {target} items", "ادمج {target} عنصر", 50, 50, Difficulty.Easy);
            CreateDailyChallenge("daily_level", ChallengeType.Level, "Complete {target} levels", "أكمل {target} مستويات", 3, 100, Difficulty.Medium);
            CreateDailyChallenge("daily_stars", ChallengeType.Stars, "Earn {target} stars", "اجمع {target} نجوم", 5, 75, Difficulty.Medium);
            CreateDailyChallenge("daily_reveal", ChallengeType.Reveal, "Reveal {target} tiles", "اكشف {target} بلاطة", 30, 60, Difficulty.Easy);
            CreateDailyChallenge("daily_chain", ChallengeType.Chain, "Create a Level {target} item", "اصنع عنصر مستوى {target}", 5, 150, Difficulty.Hard);
            CreateDailyChallenge("daily_perfect", ChallengeType.Perfect, "Get 3 stars on {target} level(s)", "احصل على 3 نجوم في {target} مستوى", 1, 100, Difficulty.Hard);
            CreateDailyChallenge("daily_nohelp", ChallengeType.NoHelp, "Complete a level without powerups", "أكمل مستوى بدون مساعدات", 1, 120, Difficulty.Hard);
        }

        private static void CreateDailyChallenge(string id, ChallengeType type, string en, string ar, int coins, int gems, Difficulty diff)
        {
            DailyChallengeData asset = ScriptableObject.CreateInstance<DailyChallengeData>();
            asset.challengeId = id;
            asset.type = type;
            asset.descriptionEN = en;
            asset.descriptionAR = ar;
            asset.rewardCoins = coins;
            asset.rewardGems = gems;
            asset.difficulty = diff;
            CreateAsset(asset, $"DailyChallenges/{id}.asset");
        }
    }
}
