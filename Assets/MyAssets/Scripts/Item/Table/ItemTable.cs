using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyAssets
{
    public enum ItemType
    {
        Wepon,
    }

    [Serializable]
    public struct ItemInfo
    {
        public int gId;
        public string gName;
        public Sprite gIcon;
        public ItemType gType;
        public AssetReferenceGameObject gObject;
    }

    [CreateAssetMenu(fileName = "ItemTable", menuName = "Scriptable Objects/ItemTable")]
    public class ItemTable : ScriptableObject
    {
        [SerializeField]
        private ItemInfo[] items;
        public ItemInfo[] Items => items;
    }
}
