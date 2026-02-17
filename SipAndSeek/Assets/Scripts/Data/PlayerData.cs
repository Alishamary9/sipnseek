using System.Collections.Generic;

namespace SipAndSeek.Data
{
    /// <summary>
    /// Serializable container for all player state data.
    /// Used by PlayerDataManager for save/load via SaveSystem.
    /// Note: JsonUtility does not serialize Dictionary, so we use parallel lists.
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        // --- Currency ---
        public int coins;
        public int gems;

        // --- Progression ---
        public int currentLevel;
        public int highestUnlockedLevel;
        public int totalXP;

        // --- Statistics ---
        public int totalMerges;
        public int totalTilesRevealed;
        public int totalLevelsCompleted;

        // --- Level Stars (parallel lists — JsonUtility compatible) ---
        public List<int> starLevelNumbers = new List<int>();
        public List<int> starValues = new List<int>();

        // --- Unlocks ---
        public List<string> unlockedAchievements = new List<string>();
        public List<string> unlockedSkins = new List<string>();
        public string activeSkin = "";

        // --- Powerup Inventory (parallel lists — JsonUtility compatible) ---
        public List<string> powerupIds = new List<string>();
        public List<int> powerupCounts = new List<int>();

        // --- Daily / Login ---
        public int consecutiveLoginDays;
        public string lastLoginDate = "";
        public List<string> completedDailyChallenges = new List<string>();
        public string lastDailyChallengeDate = "";

        // --- Settings ---
        public float bgmVolume = 1f;
        public float sfxVolume = 1f;
        public bool isMuted;
        public int languageIndex; // 0 = English, 1 = Arabic

        // ===============================
        // Helper Methods
        // ===============================

        /// <summary>
        /// Get the star count for a specific level.
        /// Returns 0 if the level has not been completed.
        /// </summary>
        public int GetStarsForLevel(int level)
        {
            int index = starLevelNumbers.IndexOf(level);
            if (index >= 0 && index < starValues.Count)
            {
                return starValues[index];
            }
            return 0;
        }

        /// <summary>
        /// Set or update the star count for a specific level.
        /// Only updates if the new star count is higher than the existing one.
        /// </summary>
        public void SetStarsForLevel(int level, int stars)
        {
            int index = starLevelNumbers.IndexOf(level);
            if (index >= 0)
            {
                // Only update if new stars are higher
                if (stars > starValues[index])
                {
                    starValues[index] = stars;
                }
            }
            else
            {
                starLevelNumbers.Add(level);
                starValues.Add(stars);
            }
        }

        /// <summary>
        /// Get the count of a specific powerup in inventory.
        /// </summary>
        public int GetPowerupCount(string powerupId)
        {
            int index = powerupIds.IndexOf(powerupId);
            if (index >= 0 && index < powerupCounts.Count)
            {
                return powerupCounts[index];
            }
            return 0;
        }

        /// <summary>
        /// Add a powerup to inventory (or increment its count).
        /// </summary>
        public void AddPowerup(string powerupId, int amount = 1)
        {
            int index = powerupIds.IndexOf(powerupId);
            if (index >= 0)
            {
                powerupCounts[index] += amount;
            }
            else
            {
                powerupIds.Add(powerupId);
                powerupCounts.Add(amount);
            }
        }

        /// <summary>
        /// Use a powerup from inventory. Returns true if successful.
        /// </summary>
        public bool UsePowerup(string powerupId)
        {
            int index = powerupIds.IndexOf(powerupId);
            if (index >= 0 && powerupCounts[index] > 0)
            {
                powerupCounts[index]--;
                return true;
            }
            return false;
        }
    }
}
