using System;
using UnityEngine;

namespace MyAssets
{
    //メニュー内で設定メニューを開閉するためのトリガークラス
    [Serializable]
    public class SettingMenuTrigger
    {
        private bool mSettingMenuActive;
        [SerializeField]
        private RectTransform[] mMenuCategorys = new RectTransform[0];

        [SerializeField]
        private RectTransform[] mSettingCategorys = new RectTransform[0];

        //せってメニューの処理一覧
        //設定メニューを有効にする
        public void EnableSetting()
        {
            foreach (RectTransform rect in mMenuCategorys)
            {
                rect.gameObject.SetActive(false);
            }
            foreach (RectTransform rect in mSettingCategorys)
            {
                rect.gameObject.SetActive(true);
            }
            mSettingMenuActive = true;
        }
        //設定メニューを無効にする
        public void DisableSetting()
        {
            foreach (RectTransform rect in mMenuCategorys)
            {
                rect.gameObject.SetActive(true);
            }
            foreach (RectTransform rect in mSettingCategorys)
            {
                rect.gameObject.SetActive(false);
            }
            mSettingMenuActive = false;
        }
        //設定メニューのアクティブ状態を設定する
        public void SetActiveSetting(bool active)
        {
            mSettingMenuActive = active;
            foreach (RectTransform rect in mMenuCategorys)
            {
                rect.gameObject.SetActive(!active);
            }
            foreach (RectTransform rect in mSettingCategorys)
            {
                rect.gameObject.SetActive(active);
            }
            mSettingMenuActive = active;
        }

        public void Update()
        {
            if (!mSettingMenuActive) return;

            if(InputManager.GetKeyDown(KeyCode.eSprint))
            {
                DisableSetting();
            }
        }
    }
}
