using UnityEngine;
using System.Collections.Generic;
using SipAndSeek.Data;
using System.Linq;
using Sirenix.OdinInspector;

namespace SipAndSeek.Managers
{
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "SipAndSeek/Managers/GameDatabase")]
    public class GameDatabase : ScriptableObject
    {
        private static GameDatabase _instance;
        public static GameDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameDatabase>("GameDatabase");
                    
                    if (_instance == null)
                    {
                        var all = Resources.LoadAll<GameDatabase>("");
                        if (all != null && all.Length > 0)
                            _instance = all[0];
                    }
                }
                return _instance;
            }
        }

        [TabGroup("Core Gameplay")]
        [Searchable, ListDrawerSettings(ShowIndexLabels = true)]
        public List<MergeChainItemData> mergeChainItems = new List<MergeChainItemData>();

        [TabGroup("Levels & Hidden")]
        [Searchable]
        public List<HiddenImageData> hiddenImages = new List<HiddenImageData>();

        [TabGroup("Rewards & Meta")]
        public List<LevelRewardData> levelRewards = new List<LevelRewardData>();
        public List<AchievementData> achievements = new List<AchievementData>();
        public List<DailyChallengeData> dailyChallenges = new List<DailyChallengeData>();

        [TabGroup("Settings")]
        public List<ObstacleData> obstacles = new List<ObstacleData>();
        public List<PowerupData> powerups = new List<PowerupData>();

        // Helper methods for easy lookup
        public MergeChainItemData GetMergeItem(string chainId, int level)
        {
            return mergeChainItems.FirstOrDefault(i => i.chainId == chainId && i.level == level);
        }

        public LevelRewardData GetLevelReward(int level)
        {
            return levelRewards.FirstOrDefault(r => r.level == level);
        }

        public ObstacleData GetObstacle(string id)
        {
            return obstacles.FirstOrDefault(o => o.obstacleId == id);
        }

        public AchievementData GetAchievement(string achId)
        {
            return achievements.FirstOrDefault(a => a.achId == achId);
        }

        public PowerupData GetPowerup(string powerupId)
        {
            return powerups.FirstOrDefault(p => p.powerupId == powerupId);
        }

        public DailyChallengeData GetDailyChallenge(string challengeId)
        {
            return dailyChallenges.FirstOrDefault(dc => dc.challengeId == challengeId);
        }
    }
}
