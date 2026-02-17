using UnityEngine;
using System;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Manages all player data: currency, progression, statistics, and powerups.
    /// Singleton MonoBehaviour with DontDestroyOnLoad.
    /// Auto-saves on every significant change.
    /// </summary>
    public class PlayerDataManager : MonoBehaviour
    {
        public static PlayerDataManager Instance { get; private set; }

        private const string SAVE_KEY = "player_data";

        private PlayerData _data;
        public PlayerData Data => _data;

        // ===============================
        // Events
        // ===============================
        public static event Action<int> OnCoinsChanged;
        public static event Action<int> OnGemsChanged;
        public static event Action<int, int> OnLevelStarsChanged; // level, stars
        public static event Action<int> OnXPChanged;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveData();
            }
        }

        private void OnApplicationQuit()
        {
            SaveData();
        }

        // ===============================
        // Currency — Coins
        // ===============================
        public int Coins => _data.coins;

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;
            _data.coins += amount;
            OnCoinsChanged?.Invoke(_data.coins);
            SaveData();
            Debug.Log($"[PlayerData] +{amount} Coins → Total: {_data.coins}");
        }

        /// <summary>
        /// Attempt to spend coins. Returns true if successful.
        /// </summary>
        public bool SpendCoins(int amount)
        {
            if (amount <= 0 || _data.coins < amount) return false;
            _data.coins -= amount;
            OnCoinsChanged?.Invoke(_data.coins);
            SaveData();
            Debug.Log($"[PlayerData] -{amount} Coins → Total: {_data.coins}");
            return true;
        }

        public bool CanAffordCoins(int amount)
        {
            return _data.coins >= amount;
        }

        // ===============================
        // Currency — Gems
        // ===============================
        public int Gems => _data.gems;

        public void AddGems(int amount)
        {
            if (amount <= 0) return;
            _data.gems += amount;
            OnGemsChanged?.Invoke(_data.gems);
            SaveData();
            Debug.Log($"[PlayerData] +{amount} Gems → Total: {_data.gems}");
        }

        /// <summary>
        /// Attempt to spend gems. Returns true if successful.
        /// </summary>
        public bool SpendGems(int amount)
        {
            if (amount <= 0 || _data.gems < amount) return false;
            _data.gems -= amount;
            OnGemsChanged?.Invoke(_data.gems);
            SaveData();
            Debug.Log($"[PlayerData] -{amount} Gems → Total: {_data.gems}");
            return true;
        }

        public bool CanAffordGems(int amount)
        {
            return _data.gems >= amount;
        }

        // ===============================
        // Level Progression
        // ===============================
        public int CurrentLevel => _data.currentLevel;
        public int HighestUnlockedLevel => _data.highestUnlockedLevel;

        public void SetLevelStars(int level, int stars)
        {
            stars = Mathf.Clamp(stars, 0, 3);
            _data.SetStarsForLevel(level, stars);

            // Track completion
            if (stars > 0)
            {
                _data.totalLevelsCompleted = Mathf.Max(_data.totalLevelsCompleted, level);
            }

            // Auto-unlock next level
            if (level >= _data.highestUnlockedLevel)
            {
                _data.highestUnlockedLevel = level + 1;
            }

            _data.currentLevel = Mathf.Max(_data.currentLevel, level);

            OnLevelStarsChanged?.Invoke(level, stars);
            SaveData();
        }

        public int GetLevelStars(int level)
        {
            return _data.GetStarsForLevel(level);
        }

        public bool IsLevelUnlocked(int level)
        {
            return level <= _data.highestUnlockedLevel;
        }

        // ===============================
        // XP
        // ===============================
        public int TotalXP => _data.totalXP;

        public void AddXP(int amount)
        {
            if (amount <= 0) return;
            _data.totalXP += amount;
            OnXPChanged?.Invoke(_data.totalXP);
            SaveData();
        }

        // ===============================
        // Statistics
        // ===============================
        public int TotalMerges => _data.totalMerges;
        public int TotalTilesRevealed => _data.totalTilesRevealed;

        public void AddMergeCount(int count = 1)
        {
            _data.totalMerges += count;
            // No auto-save here for performance; saved on level end
        }

        public void AddTilesRevealed(int count = 1)
        {
            _data.totalTilesRevealed += count;
            // No auto-save here for performance; saved on level end
        }

        /// <summary>
        /// Call this at end of level to persist stats accumulated during play.
        /// </summary>
        public void FlushStats()
        {
            SaveData();
        }

        // ===============================
        // Achievements
        // ===============================
        public bool IsAchievementUnlocked(string achId)
        {
            return _data.unlockedAchievements.Contains(achId);
        }

        public void UnlockAchievement(string achId)
        {
            if (!_data.unlockedAchievements.Contains(achId))
            {
                _data.unlockedAchievements.Add(achId);
                SaveData();
                Debug.Log($"[PlayerData] 🏆 Achievement unlocked: {achId}");
            }
        }

        // ===============================
        // Powerups
        // ===============================
        public int GetPowerupCount(string powerupId)
        {
            return _data.GetPowerupCount(powerupId);
        }

        public void AddPowerup(string powerupId, int amount = 1)
        {
            _data.AddPowerup(powerupId, amount);
            SaveData();
        }

        public bool UsePowerup(string powerupId)
        {
            bool success = _data.UsePowerup(powerupId);
            if (success) SaveData();
            return success;
        }

        // ===============================
        // Daily Login
        // ===============================
        public void ProcessDailyLogin()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (_data.lastLoginDate == today) return; // Already logged in today

            string yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            if (_data.lastLoginDate == yesterday)
            {
                _data.consecutiveLoginDays++;
                if (_data.consecutiveLoginDays > 7)
                {
                    _data.consecutiveLoginDays = 1; // Reset after 7-day cycle
                }
            }
            else
            {
                _data.consecutiveLoginDays = 1; // Streak broken
            }

            _data.lastLoginDate = today;
            SaveData();
            Debug.Log($"[PlayerData] Daily login: Day {_data.consecutiveLoginDays}");
        }

        public int ConsecutiveLoginDays => _data.consecutiveLoginDays;

        // ===============================
        // Save / Load / Reset
        // ===============================
        public void SaveData()
        {
            SaveSystem.Save(SAVE_KEY, _data);
        }

        public void LoadData()
        {
            _data = SaveSystem.Load<PlayerData>(SAVE_KEY);

            // First-time player defaults
            if (_data.highestUnlockedLevel < 1)
            {
                _data.highestUnlockedLevel = 1;
            }
        }

        /// <summary>
        /// Completely reset all player data. Use with caution!
        /// </summary>
        public void ResetData()
        {
            SaveSystem.DeleteSave(SAVE_KEY);
            _data = new PlayerData { highestUnlockedLevel = 1 };
            OnCoinsChanged?.Invoke(0);
            OnGemsChanged?.Invoke(0);
            Debug.Log("[PlayerData] ⚠️ All player data has been reset.");
        }
    }
}
