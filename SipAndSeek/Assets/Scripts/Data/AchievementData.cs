using UnityEngine;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewAchievement", menuName = "SipAndSeek/Data/Achievement")]
    public class AchievementData : ScriptableObject
    {
        public string achId;
        public string nameEN;
        public string nameAR;
        [TextArea] public string description;
        public string condition;
        public RewardType rewardType;
        public int rewardValue; // For complex rewards (Title+Skin), we might need a string or custom class, but int/string combo is fine for now.
        public string rewardValueString; // For non-numeric rewards
        public string icon;
    }
}
