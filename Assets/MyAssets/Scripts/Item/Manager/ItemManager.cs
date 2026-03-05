using UnityEngine;

namespace MyAssets
{
    //アイテムの生成、管理を行うクラス
    public class ItemManager : MonoBehaviour
    {
        private static ItemManager  mInstance;
        public static ItemManager   Instance => mInstance;

        [SerializeField]
        private ItemTable           mItemTable;
        public ItemTable            ItemTable => mItemTable;

        private void Awake()
        {
            if(mInstance != null)
            {
                Destroy(gameObject);
                return;
            }
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetItem(int id)
        {
            ItemInfo itemInfo = mItemTable.Items[id];

        }

    }
}
