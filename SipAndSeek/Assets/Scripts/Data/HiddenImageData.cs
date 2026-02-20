using UnityEngine;
using SipAndSeek;
using Sirenix.OdinInspector;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewHiddenImage", menuName = "SipAndSeek/Data/HiddenImage")]
    public class HiddenImageData : ScriptableObject
    {
        [TitleGroup("System Info")]
        [HorizontalGroup("System Info/Split")]
        public string imageId;
        
        [HorizontalGroup("System Info/Split")]
        public int level;

        [TitleGroup("Visuals")]
        [PreviewField(120, ObjectFieldAlignment.Center)]
        public Sprite backgroundSprite;

        [TitleGroup("Metadata")]
        public string nameEN;
        public string nameAR;
        public string theme;

        [TitleGroup("Details")]
        [TextArea] public string artistPrompt;
        public string gridSize = "3x3";
        [EnumPaging] public Difficulty difficulty;
        public string rewardId;
    }
}
