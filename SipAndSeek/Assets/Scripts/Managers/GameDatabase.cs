using UnityEngine;
using System.Collections.Generic;
using SipAndSeek.Data;
using System.Linq;

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
                }
                return _instance;
            }
        }

        [Header("Data Tables")]
        public List<MergeChainItemData> mergeChainItems = new List<MergeChainItemData>();
        public List<LevelRewardData> levelRewards = new List<LevelRewardData>();
        public List<ObstacleData> obstacles = new List<ObstacleData>();
        public List<PowerupData> powerups = new List<PowerupData>();
        public List<HiddenImageData> hiddenImages = new List<HiddenImageData>();
        public List<AchievementData> achievements = new List<AchievementData>();
        public List<DailyChallengeData> dailyChallenges = new List<DailyChallengeData>();

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
    }
}
