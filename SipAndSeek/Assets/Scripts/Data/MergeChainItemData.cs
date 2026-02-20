using UnityEngine;
using SipAndSeek;
using Sirenix.OdinInspector;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewMergeChainItem", menuName = "SipAndSeek/Data/MergeChainItem")]
    public class MergeChainItemData : ScriptableObject
    {
        [TitleGroup("Basic Info")]
        [HorizontalGroup("Basic Info/Split", 0.5f)]
        [VerticalGroup("Basic Info/Split/Left")]
        public string chainId;
        
        [VerticalGroup("Basic Info/Split/Left")]
        public int level;

        [PreviewField(80, ObjectFieldAlignment.Right)]
        [HorizontalGroup("Basic Info/Split", 80)]
        public Sprite icon;

        [TitleGroup("Localization")]
        public string itemNameEN;
        public string itemNameAR;

        [TitleGroup("Description")]
        [TextArea(3, 10)] public string visualDescription;

        [TitleGroup("Stats")]
        [EnumPaging] public ItemRarity rarity;
        public int sellPrice;
        [ProgressBar(0, 1, ColorGetter = "GetWeightColor")] public float spawnWeight;

        private Color GetWeightColor(float value)
        {
            return Color.Lerp(Color.red, Color.green, value);
        }
    }
}
