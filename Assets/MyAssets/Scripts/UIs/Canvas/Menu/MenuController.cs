using UnityEngine;

namespace MyAssets
{
    public class MenuController : MonoBehaviour
    {
        private bool mMenuFlag = false;

        [SerializeField]
        private SettingMenuTrigger mSettingMenu;

        [SerializeField]
        private bool mEnableMenu = true;

        private void Start()
        {
            if(mEnableMenu)
            {
                mMenuFlag = false;
                Time.timeScale = 1.0f;
                InputManager.SetLockedMouseMode();
                GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.Option);
                GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.HUD);
                GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.Tutorial);
            }
        }

        private void Update()
        {
            if (ResultManager.IsStopGameUIInput) { return; }
            if(mEnableMenu)
            {
                if(InputManager.GetKeyDown(KeyCode.eMenu))
                {
                    EnableMenu();
                    mSettingMenu.DisableSetting();
                }
            }
            mSettingMenu.Update();
        }

        public void EnableMenu()
        {
            if (ResultManager.IsStopGameUIInput) { return; }
            mMenuFlag = !mMenuFlag;
            if(mMenuFlag)
            {
                Time.timeScale = 0.0f;
                InputManager.SetNoneMouseMode();
                GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.Option);
                GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.HUD);
                GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.Tutorial);
                SoundManager.Instance.PlayOneShot2D("Open_Menu", false);
            }
            else
            {
                Time.timeScale = 1.0f;
                InputManager.SetLockedMouseMode();
                GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.Option);
                GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.HUD);
                GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.Tutorial);
                SoundManager.Instance.PlayOneShot2D("Close_Menu", false);
            }
        }

        public void SetSettingMenu(bool active)
        {
            mSettingMenu.SetActiveSetting(active);
            SoundManager.Instance.PlayOneShot2D("Decide_Button", false);
        }
    }
}
