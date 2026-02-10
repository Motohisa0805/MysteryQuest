using System;
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

        public void OnDisable()
        {
            SoundManager.Instance.SetSEVolumeAudios();
            SoundManager.Instance.SetBGMVolumeAudios();
        }
        // ì¸óÕë¨ìxê›íË
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
        // ÉtÉåÅ[ÉÄÉåÅ[Égê›íË
        public void SetFrameRate(float rate)
        {
            DataManager.SettingData.gFrameRate = rate;
            DataManager.Save(DataManager.SettingData);
            Application.targetFrameRate = (int)rate;
            mController.SetOutputText(SettingMenuController.SettingEntryItem.FrameRate, rate.ToString());
        }
        // SEâπó ê›íË
        public void SetSEVolume(float volume)
        {
            DataManager.SettingData.gSEVolume = volume;
            if (DataManager.SettingData.gSEVolume < 0f)
            {
                DataManager.SettingData.gSEVolume = 0f;
            }
            else if (DataManager.SettingData.gSEVolume > 1f)
            {
                DataManager.SettingData.gSEVolume = 1f;
            }
            DataManager.Save(DataManager.SettingData);
            float seVolume = DataManager.SettingData.gSEVolume * 100;
            mController.SetOutputText(SettingMenuController.SettingEntryItem.SE, Math.Ceiling(seVolume).ToString());
        }
        // SEâπó â¡éZê›íË
        public void AddSEVolume(float volume)
        {
            DataManager.SettingData.gSEVolume += volume;
            if(DataManager.SettingData.gSEVolume < 0f)
            {
                DataManager.SettingData.gSEVolume = 0f;
            }
            else if(DataManager.SettingData.gSEVolume > 1f)
            {
                DataManager.SettingData.gSEVolume = 1f;
            }
            DataManager.Save(DataManager.SettingData);
            float seVolume = DataManager.SettingData.gSEVolume * 100;
            mController.SetOutputText(SettingMenuController.SettingEntryItem.SE, Math.Ceiling(seVolume).ToString());
        }
        // BGMâπó ê›íË
        public void SetBGMVolume(float volume)
        {
            DataManager.SettingData.gBGMVolume = volume;
            if (DataManager.SettingData.gBGMVolume < 0f)
            {
                DataManager.SettingData.gBGMVolume = 0f;
            }
            else if (DataManager.SettingData.gBGMVolume > 1f)
            {
                DataManager.SettingData.gBGMVolume = 1f;
            }
            DataManager.Save(DataManager.SettingData);
            float bgmVolume = DataManager.SettingData.gBGMVolume * 100;
            mController.SetOutputText(SettingMenuController.SettingEntryItem.BGM, Math.Ceiling(bgmVolume).ToString());
        }
        // BGMâπó â¡éZê›íË
        public void AddBGMVolume(float volume)
        {
            DataManager.SettingData.gBGMVolume += volume;
            if (DataManager.SettingData.gBGMVolume < 0f)
            {
                DataManager.SettingData.gBGMVolume = 0f;
            }
            else if (DataManager.SettingData.gBGMVolume > 1f)
            {
                DataManager.SettingData.gBGMVolume = 1f;
            }
            DataManager.Save(DataManager.SettingData);
            float bgmVolume = DataManager.SettingData.gBGMVolume * 100;
            mController.SetOutputText(SettingMenuController.SettingEntryItem.BGM, Math.Ceiling(bgmVolume).ToString());
        }
    }
}
