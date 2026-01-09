using UnityEngine;

namespace MyAssets
{
    // プレイヤーUIマネージャークラス
    // プレイヤーUI全体の管理を行うシングルトンクラス
    public class PlayerUIManager : MonoBehaviour
    {
        private static PlayerUIManager    instance;
        public static PlayerUIManager     Instance => instance;

        [SerializeField]
        private Transform                 mThrowCircle;
        public Transform                  ThrowCircle => mThrowCircle;

        private LifeHUDController         mLifeHUDController;
        public LifeHUDController          LifeHUDController { get { return mLifeHUDController; }set {  mLifeHUDController = value; } }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (mLifeHUDController && mLifeHUDController.transform.parent.gameObject.activeSelf)
            {
                mLifeHUDController.UpdateLifeHUD();
            }
        }
    }
}
