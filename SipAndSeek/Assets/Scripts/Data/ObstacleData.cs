using UnityEngine;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewObstacle", menuName = "SipAndSeek/Data/Obstacle")]
    public class ObstacleData : ScriptableObject
    {
        public string obstacleId;
        public string nameEN;
        public string nameAR;
        [TextArea] public string description;
        public string unlockCondition;
        public string visualState1;
        public string visualState2;
        public string soundEffect;
    }
}
