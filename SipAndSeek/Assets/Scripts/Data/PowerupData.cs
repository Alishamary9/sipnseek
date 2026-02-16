using UnityEngine;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewPowerup", menuName = "SipAndSeek/Data/Powerup")]
    public class PowerupData : ScriptableObject
    {
        public string powerupId;
        public string nameEN;
        public string nameAR;
        [TextArea] public string effectDescription;
        public int coinPrice;
        public int gemPrice;
        public int maxHold;
        public string iconRef; // Name of the sprite file
    }
}
