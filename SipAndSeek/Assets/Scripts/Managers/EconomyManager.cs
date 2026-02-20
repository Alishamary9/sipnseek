using UnityEngine;
using System;
using SipAndSeek.Data;

namespace SipAndSeek.Managers
{
    /// <summary>
    /// Orchestrates the game economy: reward granting, purchases, and daily bonuses.
    /// Works with PlayerDataManager for persistence and GameDatabase for reward data.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        // ===============================
        // Events
        // ===============================

        /// <summary>Fired when any reward is granted (coins, gems, XP).</summary>
        public static event Action<RewardType, int> OnRewardGranted;

        /// <summary>Fired when a purchase fails (insufficient funds).</summary>
        public static event Action<string> OnPurchaseFailed;

        // ===============================
        // Daily Login Reward Table (Day → Coins, Gems)
        // ===============================
        private static readonly int[] DailyCoins = { 0, 50, 75, 100, 125, 150, 200, 500 };
        private static readonly int[] DailyGems  = { 0,  0,  0,   5,   0,  10,   0,  25 };

        // ===============================
        // Lifecycle
        // ===============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[EconomyManager] Initialized.");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += HandleLevelCompleted;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= HandleLevelCompleted;
        }

        // ===============================
        // Level Rewards
        // ===============================

        /// <summary>
        /// Automatically called when a level is completed.
        /// Grants rewards based on star rating from LevelRewardData.
        /// </summary>
        private void HandleLevelCompleted(int level, int stars, float completionPercent)
        {
            GrantLevelRewards(level, stars, completionPercent);
        }

        /// <summary>
        /// Grant rewards for completing a level with the given star rating.
        /// </summary>
        public void GrantLevelRewards(int level, int stars, float completionPercent)
        {
            if (PlayerDataManager.Instance == null || GameDatabase.Instance == null) return;

            LevelRewardData reward = GameDatabase.Instance.GetLevelReward(level);
            if (reward == null)
            {
                Debug.LogWarning($"[EconomyManager] No reward data for level {level}");
                return;
            }

            // Coins based on completion tier
            int coins = 0;
            if (completionPercent >= 100f)
                coins = reward.coins100;
            else if (completionPercent >= 90f)
                coins = reward.coins90;
            else if (completionPercent >= 80f)
                coins = reward.coins80;

            if (coins > 0)
            {
                PlayerDataManager.Instance.AddCoins(coins);
                OnRewardGranted?.Invoke(RewardType.Coins, coins);
            }

            // Gems for perfect (3 stars / 100%)
            if (stars >= 3 && reward.gems100 > 0)
            {
                PlayerDataManager.Instance.AddGems(reward.gems100);
                OnRewardGranted?.Invoke(RewardType.Gems, reward.gems100);
            }

            // XP
            if (reward.xp > 0)
            {
                PlayerDataManager.Instance.AddXP(reward.xp);
            }

            Debug.Log($"[EconomyManager] 🎁 Level {level} Rewards: {coins} coins, " +
                      $"{(stars >= 3 ? reward.gems100 : 0)} gems, {reward.xp} XP");
        }

        // ===============================
        // Purchases
        // ===============================

        /// <summary>
        /// Attempt to purchase something with coins. Returns true if successful.
        /// </summary>
        public bool PurchaseWithCoins(int cost, string itemDescription = "")
        {
            if (PlayerDataManager.Instance == null) return false;

            if (PlayerDataManager.Instance.SpendCoins(cost))
            {
                Debug.Log($"[EconomyManager] 💰 Purchased '{itemDescription}' for {cost} coins");
                return true;
            }

            Debug.Log($"[EconomyManager] ❌ Cannot afford {cost} coins (have {PlayerDataManager.Instance.Coins})");
            OnPurchaseFailed?.Invoke($"Need {cost} coins");
            return false;
        }

        /// <summary>
        /// Attempt to purchase something with gems. Returns true if successful.
        /// </summary>
        public bool PurchaseWithGems(int cost, string itemDescription = "")
        {
            if (PlayerDataManager.Instance == null) return false;

            if (PlayerDataManager.Instance.SpendGems(cost))
            {
                Debug.Log($"[EconomyManager] 💎 Purchased '{itemDescription}' for {cost} gems");
                return true;
            }

            Debug.Log($"[EconomyManager] ❌ Cannot afford {cost} gems (have {PlayerDataManager.Instance.Gems})");
            OnPurchaseFailed?.Invoke($"Need {cost} gems");
            return false;
        }

        // ===============================
        // Daily Login Rewards
        // ===============================

        /// <summary>
        /// Grant the daily login reward based on consecutive login day (1-7).
        /// </summary>
        public void GrantDailyLoginReward(int day)
        {
            if (PlayerDataManager.Instance == null) return;

            day = Mathf.Clamp(day, 1, 7);

            int coins = DailyCoins[day];
            int gems = DailyGems[day];

            if (coins > 0)
            {
                PlayerDataManager.Instance.AddCoins(coins);
                OnRewardGranted?.Invoke(RewardType.Coins, coins);
            }

            if (gems > 0)
            {
                PlayerDataManager.Instance.AddGems(gems);
                OnRewardGranted?.Invoke(RewardType.Gems, gems);
            }

            Debug.Log($"[EconomyManager] 📅 Daily Login Day {day}: +{coins} coins, +{gems} gems");
        }
    }
}
