using UnityEngine;

namespace MyAssets
{
    public class MenuController : MonoBehaviour
    {
        private bool mMenuFlag = false;

        private void Update()
        {
            if (ResultManager.IsResulting) { return; }
            if(InputManager.GetKeyDown(KeyCode.eMenu))
            {
                EnableMenu();
            }
        }

        public void EnableMenu()
        {
            if (ResultManager.IsResulting) { return; }
            mMenuFlag = !mMenuFlag;
            if(mMenuFlag)
            {
                Time.timeScale = 0.0f;
                InputManager.SetNoneMouseMode();
                GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.Option);
                GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.HUD);
            }
            else
            {
                Time.timeScale = 1.0f;
                InputManager.SetLockedMouseMode();
                GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.Option);
                GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.HUD);
            }
        }
    }
}
