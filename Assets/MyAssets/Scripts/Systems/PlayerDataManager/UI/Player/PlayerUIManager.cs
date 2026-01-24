using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    // プレイヤーUIマネージャークラス
    // プレイヤーUI全体の管理を行うシングルトンクラス
    public class PlayerUIManager : MonoBehaviour
    {
        private static PlayerUIManager      mInstance;
        public static PlayerUIManager       Instance => mInstance;

        private PlayableInput               mPlayableInput;
        public PlayableInput PlayableInput
        {
            get 
            {
                if (mPlayableInput == null)
                {
                    mPlayableInput = FindAnyObjectByType<PlayableInput>();
                }
                return mPlayableInput; 
            }
        }

        [SerializeField]
        private Transform                   mThrowCircle;
        public Transform                    ThrowCircle => mThrowCircle;

        private LifeHUDController           mLifeHUDController;
        public LifeHUDController            LifeHUDController { get { return mLifeHUDController; }set {  mLifeHUDController = value; } }

        private StaminaHUDController        mStaminaHUDController;
        public StaminaHUDController         StaminaHUDController { get { return mStaminaHUDController; } set { mStaminaHUDController = value; } }

        private ActionButtonController      mButtonController;
        public ActionButtonController       ActionButtonController { get { return mButtonController; } set { mButtonController = value; } }

        private void OnEnable()
        {
            // シーンがロードされた時のイベントに登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 破棄時にイベント解除（お作法）
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // シーンがロードされるたびに呼ばれる
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            mPlayableInput = FindAnyObjectByType<PlayableInput>();
        }

        private void Awake()
        {
            if (mInstance != null)
            {
                Destroy(gameObject);
                return;
            }
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (mLifeHUDController && mLifeHUDController.transform.parent.gameObject.activeSelf)
            {
                mLifeHUDController.UpdateLifeHUD();
            }
            if(mStaminaHUDController)
            {
                mStaminaHUDController.CheckStamina();
                if (mStaminaHUDController.transform.parent.gameObject.activeSelf)
                {
                    mStaminaHUDController.UpdateStaminaHUD();
                }
            }
            if(mButtonController)
            {
                mButtonController.UpdateActionButton();
            }
        }
    }
}
