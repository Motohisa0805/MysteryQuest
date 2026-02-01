using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MyAssets
{
    public class SettingMenuController : MonoBehaviour
    {
        public enum SettingEntry
        {
            Input,
            Audio,
        }

        [SerializeField]
        private SettingEntry mEntry = SettingEntry.Input;

        [SerializeField]
        private RectTransform[] mSettingMenuEntrys = new RectTransform[0];

        public void SetActiveEntry(int entry)
        {
            for (int i = 0; i < mSettingMenuEntrys.Length; i++)
            {
                if (i == entry)
                {
                    mSettingMenuEntrys[i].gameObject.SetActive(true);
                }
                else
                {
                    mSettingMenuEntrys[i].gameObject.SetActive(false);
                }
            }
            mEntry = (SettingEntry)entry;
            SoundManager.Instance.PlayOneShot2D("Decide_Button", false);
        }

        private void OnEnable()
        {
            mEntry = SettingEntry.Input;
            SetActiveEntry((int)mEntry);
        }
    }
}
