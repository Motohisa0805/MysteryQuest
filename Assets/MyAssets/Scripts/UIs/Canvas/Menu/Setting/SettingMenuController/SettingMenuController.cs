using UnityEngine;

namespace MyAssets
{
    public class SettingMenuController : MonoBehaviour
    {
        public enum SettingEntry
        {
            Input,
            Audio,
        }
        public enum SettingEntryItem
        {
            Input,
            FrameRate,
            SE,
            BGM,
        }

        [SerializeField]
        private SettingEntry    mEntry = SettingEntry.Input;

        [SerializeField]
        private RectTransform[] mSettingMenuEntrys = new RectTransform[0];

        [SerializeField]
        private OutputText[]    mSettingOutputTexts = new OutputText[0];

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
        }

        public void SetOutputText(SettingEntryItem entry,string text)
        {
            if(mSettingOutputTexts.Length - 1 < (int)entry) { return; }
            mSettingOutputTexts[(int)entry].SetText(text);
        }

        private void OnEnable()
        {
            mEntry = SettingEntry.Input;
            SetActiveEntry((int)mEntry);
        }
    }
}
