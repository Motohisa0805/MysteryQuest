using UnityEngine;

namespace MyAssets
{
    public class GameHUDType : MonoBehaviour
    {
        public enum GameUIPanelType
        {
            Tutorial = 0,
            HUD,
            Option,
            Result,
        }
        [SerializeField]
        private GameUIPanelType mGameUIPanelType;
        public GameUIPanelType GameUIType => mGameUIPanelType;
    }
}
