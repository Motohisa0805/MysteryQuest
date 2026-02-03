using UnityEngine;

namespace MyAssets
{
    public class SettingData : MonoBehaviour
    {
        private SettingMenuController mController;


        private void Awake()
        {
            mController = GetComponent<SettingMenuController>();
            if( mController == null )
            {
                Debug.LogError("Not Find SettingMenuController" + gameObject.name);
            }
        }

        public void OnEnable()
        {
            if(mController == null)
            {
                mController = GetComponent<SettingMenuController>();
            }
            DataManager.SettingDataAwake();
            SetInputRate(DataManager.SettingData.gInputRate);
            SetFrameRate(DataManager.SettingData.gFrameRate);
            SetSEVolume(DataManager.SettingData.gSEVolume);
            SetBGMVolume(DataManager.SettingData.gBGMVolume);
        }
        public void SetInputRate(float rate)
        {
            DataManager.SettingData.gInputRate = rate;
            DataManager.Save(DataManager.SettingData);
            switch(rate)
            {
                case 0.5f:
                    mController.SetOutputText(SettingMenuController.SettingEntryItem.Input, "íxÇ¢");
                    break;
                case 1.0f:
                    mController.SetOutputText(SettingMenuController.SettingEntryItem.Input, "ïÅí ");
                    break;
                case 1.5f:
                    mController.SetOutputText(SettingMenuController.SettingEntryItem.Input, "ë¨Ç¢");
                    break;
            }

        }

        public void SetFrameRate(float rate)
        {
            DataManager.SettingData.gFrameRate = rate;
            DataManager.Save(DataManager.SettingData);
            Application.targetFrameRate = (int)rate;
            mController.SetOutputText(SettingMenuController.SettingEntryItem.FrameRate, rate.ToString());
        }

        public void SetSEVolume(float volume)
        {
            DataManager.SettingData.gSEVolume = volume;
            DataManager.Save(DataManager.SettingData);
            mController.SetOutputText(SettingMenuController.SettingEntryItem.SE, volume.ToString());
        }

        public void SetBGMVolume(float volume)
        {
            DataManager.SettingData.gBGMVolume = volume;
            DataManager.Save(DataManager.SettingData);
            mController.SetOutputText(SettingMenuController.SettingEntryItem.BGM, volume.ToString());
        }
    }
}
