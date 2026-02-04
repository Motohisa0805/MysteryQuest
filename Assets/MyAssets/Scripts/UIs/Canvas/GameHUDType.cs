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

        [SerializeField]
        private RectTransform[] mUIPanels = new RectTransform[0];
        public RectTransform[] UIPanels => mUIPanels;

        private void OnEnable()
        {
            for (int i = 0; i < mUIPanels.Length; i++)
            {
                mUIPanels[i].gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < mUIPanels.Length; i++)
            {
                mUIPanels[i].gameObject.SetActive(false);
            }
        }
    }
}
