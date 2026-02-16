using UnityEngine;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewLevelReward", menuName = "SipAndSeek/Data/LevelReward")]
    public class LevelRewardData : ScriptableObject
    {
        public int level;
        public int coins80;
        public int coins90;
        public int coins100;
        public int gems100;
        public string itemReward; // e.g. "First Coffee Seed"
        public int xp;
        public string unlocksFeature;
        public string narrativeId;
    }
}
