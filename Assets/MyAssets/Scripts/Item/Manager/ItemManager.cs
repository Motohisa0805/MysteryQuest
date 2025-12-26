using UnityEngine;

namespace MyAssets
{
    public class ItemManager : MonoBehaviour
    {
        private static ItemManager instance;
        public static ItemManager Instance => instance;

        [SerializeField]
        private ItemTable mItemTable;
        public ItemTable ItemTable => mItemTable;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetItem(int id)
        {
            ItemInfo itemInfo = mItemTable.Items[id];

        }

    }
}
