using UnityEngine;
using SipAndSeek;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewHiddenImage", menuName = "SipAndSeek/Data/HiddenImage")]
    public class HiddenImageData : ScriptableObject
    {
        public string imageId;
        public int level;
        public string nameEN;
        public string nameAR;
        public string theme;
        [TextArea] public string artistPrompt;
        public string gridSize; // e.g. "3x3"
        public Difficulty difficulty;
        public string rewardId;
    }
}
