using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class GameUserInterfaceManager : MonoBehaviour
    {
        private static GameUserInterfaceManager mInstance;
        public static GameUserInterfaceManager Instance => mInstance;

        private Dictionary<GameHUDType.GameUIPanelType, GameHUDType> mGameHUDTypes = new Dictionary<GameHUDType.GameUIPanelType, GameHUDType>();

        public void InitHUD()
        {
            mGameHUDTypes.Clear();
            GameHUDType[] types = FindObjectsByType<GameHUDType>(FindObjectsSortMode.None);
            foreach (var type in types)
            {
                mGameHUDTypes.Add(type.GameUIType, type);
                type.transform.gameObject.SetActive(false);
            }
            SetActiveHUD(true, GameHUDType.GameUIPanelType.HUD);
        }

        private void Awake()
        {
            if(mInstance != null)
            {
                mInstance.InitHUD();
                Destroy(gameObject);
                return;
            }
            mInstance = this;
            DontDestroyOnLoad(gameObject);
            InitHUD();

        }

        public void SetActiveHUD(bool active,GameHUDType.GameUIPanelType enumType)
        {
            mGameHUDTypes.TryGetValue(enumType, out GameHUDType hud);
            if (hud != null)
            {
                hud.gameObject.SetActive(active);
            }
        }
    }
}
