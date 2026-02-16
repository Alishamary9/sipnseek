using UnityEngine;

namespace SipAndSeek
{
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum RewardType
    {
        Coins,
        Gems,
        Item,
        Powerup,
        Title,
        Skin,
        FeatureUnlock
    }

    public enum ChallengeType
    {
        Merge,
        Level,
        Stars,
        Reveal,
        Chain,
        Perfect,
        NoHelp
    }

    public enum Difficulty
    {
        VeryEasy,
        Easy,
        MediumEasy,
        Medium,
        Hard,
        VeryHard
    }

    public enum ObstacleType
    {
        Lock,
        Ice,
        KeyLock,
        Dark,
        Golden
    }
}
