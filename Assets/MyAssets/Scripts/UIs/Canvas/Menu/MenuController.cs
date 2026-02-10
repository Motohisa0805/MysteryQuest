using UnityEngine;

namespace MyAssets
{
    //メニュー画面の管理を行うクラス
    //メニューの開閉、各種メニュー画面の管理を行う
    //メニューが存在するCanvasにアタッチする
    public class MenuController : MonoBehaviour
    {
        private bool                mMenuFlag = false;

        [SerializeField]
        private SettingMenuTrigger  mSettingMenu;

        private void Start()
        {
            mMenuFlag = false;
            Time.timeScale = 1.0f;
            InputManager.SetLockedMouseMode();
            GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.Option);
            GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.HUD);
            GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.Tutorial);
        }

        private void Update()
        {
            if (ResultManager.IsStopGameUIInput) { return; }
            if(InputManager.GetKeyDown(KeyCode.eMenu) || InputManager.GetKeyDown(KeyCode.eSprint) && mMenuFlag)
            {
                EnableMenu();
                mSettingMenu.DisableSetting();
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
        }
    }
}
