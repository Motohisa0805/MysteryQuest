using UnityEngine;

namespace MyAssets
{
    public class SettingEntry_Information : MonoBehaviour
    {
        [SerializeField]
        private RectTransform[] mEntrys = new RectTransform[0];

        private void OnEnable()
        {
            foreach (var entry in mEntrys)
            {
                entry.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            foreach (var entry in mEntrys)
            {
                entry.gameObject.SetActive(false);
            }
        }
    }
}
