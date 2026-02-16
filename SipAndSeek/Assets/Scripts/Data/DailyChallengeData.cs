using UnityEngine;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewDailyChallenge", menuName = "SipAndSeek/Data/DailyChallenge")]
    public class DailyChallengeData : ScriptableObject
    {
        public string challengeId;
        public ChallengeType type;
        public string descriptionEN;
        public string descriptionAR;
        public string targetTemplate; // "Merge {target} items" - stored with placeholder
        public int rewardCoins;
        public int rewardGems;
        public Difficulty difficulty;
    }
}
