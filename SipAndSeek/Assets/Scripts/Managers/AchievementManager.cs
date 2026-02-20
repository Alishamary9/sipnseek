using UnityEngine;
using System;
using System.Collections.Generic;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Evaluates achievement conditions after game events and unlocks achievements.
    /// Conditions are parsed from AchievementData.condition strings.
    /// Format: "type>=value" e.g. "merges>=100", "levels>=10", "stars>=30", "tiles>=500"
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when a new achievement is unlocked.</summary>
        public static event Action<AchievementData> OnAchievementUnlocked;

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[AchievementManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += OnLevelCompleted;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= OnLevelCompleted;
        }

        private void OnLevelCompleted(int level, int stars, float percent)
        {
            CheckAllAchievements();
        }

        // ===============================
        // Achievement Checking
        // ===============================

        /// <summary>
        /// Check all achievements and unlock any that have been met.
        /// Call this after significant game events (level complete, merge milestones, etc).
        /// </summary>
        public void CheckAllAchievements()
        {
            if (GameDatabase.Instance == null || PlayerDataManager.Instance == null) return;

            List<AchievementData> achievements = GameDatabase.Instance.achievements;
            if (achievements == null) return;

            foreach (AchievementData ach in achievements)
            {
                if (string.IsNullOrEmpty(ach.achId)) continue;

                // Skip already unlocked
                if (PlayerDataManager.Instance.IsAchievementUnlocked(ach.achId)) continue;

                // Check condition
                if (EvaluateCondition(ach.condition))
                {
                    UnlockAchievement(ach);
                }
            }
        }

        /// <summary>
        /// Parse and evaluate a condition string like "merges>=100".
        /// </summary>
        private bool EvaluateCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition)) return false;

            // Parse "type>=value"
            string[] parts = condition.Split(new[] { ">=" }, StringSplitOptions.None);
            if (parts.Length != 2) return false;

            string type = parts[0].Trim().ToLower();
            if (!int.TryParse(parts[1].Trim(), out int target)) return false;

            int current = GetStatValue(type);
            return current >= target;
        }

        /// <summary>
        /// Get the current value of a tracked statistic.
        /// </summary>
        private int GetStatValue(string type)
        {
            if (PlayerDataManager.Instance == null) return 0;

            switch (type)
            {
                case "merges":
                    return PlayerDataManager.Instance.TotalMerges;

                case "levels":
                    return PlayerDataManager.Instance.Data.totalLevelsCompleted;

                case "stars":
                    // Total stars collected across all levels
                    int totalStars = 0;
                    var data = PlayerDataManager.Instance.Data;
                    for (int i = 0; i < data.starValues.Count; i++)
                    {
                        totalStars += data.starValues[i];
                    }
                    return totalStars;

                case "tiles":
                    return PlayerDataManager.Instance.TotalTilesRevealed;

                case "coins":
                    return PlayerDataManager.Instance.Coins;

                case "gems":
                    return PlayerDataManager.Instance.Gems;

                case "login":
                    return PlayerDataManager.Instance.ConsecutiveLoginDays;

                default:
                    Debug.LogWarning($"[AchievementManager] Unknown stat type: {type}");
                    return 0;
            }
        }

        /// <summary>
        /// Unlock an achievement and grant its reward.
        /// </summary>
        private void UnlockAchievement(AchievementData ach)
        {
            PlayerDataManager.Instance.UnlockAchievement(ach.achId);

            // Grant reward
            if (ach.rewardValue > 0 && EconomyManager.Instance != null)
            {
                switch (ach.rewardType)
                {
                    case RewardType.Coins:
                        PlayerDataManager.Instance.AddCoins(ach.rewardValue);
                        break;
                    case RewardType.Gems:
                        PlayerDataManager.Instance.AddGems(ach.rewardValue);
                        break;
                    case RewardType.Powerup:
                        PlayerDataManager.Instance.AddPowerup(ach.rewardValueString, ach.rewardValue);
                        break;
                }
            }

            Debug.Log($"[AchievementManager] 🏆 Achievement Unlocked: {ach.nameEN} " +
                      $"(Reward: {ach.rewardValue} {ach.rewardType})");
            OnAchievementUnlocked?.Invoke(ach);
        }

        // ===============================
        // Queries
        // ===============================

        /// <summary>
        /// Get the total number of unlocked achievements.
        /// </summary>
        public int UnlockedCount
        {
            get
            {
                return PlayerDataManager.Instance != null
                    ? PlayerDataManager.Instance.Data.unlockedAchievements.Count
                    : 0;
            }
        }

        /// <summary>
        /// Get the total number of achievements.
        /// </summary>
        public int TotalCount
        {
            get
            {
                return GameDatabase.Instance != null
                    ? GameDatabase.Instance.achievements.Count
                    : 0;
            }
        }
    }
}
