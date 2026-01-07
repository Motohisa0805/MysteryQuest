using UnityEngine;

namespace MyAssets
{
    public class GameHUDType : MonoBehaviour
    {
        public enum GameUIPanelType
        {
            HUD,
            Option,
            Result,
        }
        [SerializeField]
        private GameUIPanelType mGameUIPanelType;
        public GameUIPanelType GameUIType => mGameUIPanelType;
    }
}
