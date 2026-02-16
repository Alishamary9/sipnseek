using UnityEngine;

namespace SipAndSeek.Data
{
    [CreateAssetMenu(fileName = "NewMergeChainItem", menuName = "SipAndSeek/Data/MergeChainItem")]
    public class MergeChainItemData : ScriptableObject
    {
        public string chainId;
        public int level;
        public string itemNameEN;
        public string itemNameAR;
        [TextArea] public string visualDescription;
        public ItemRarity rarity;
        public int sellPrice;
        [Range(0f, 1f)] public float spawnWeight;
    }
}
