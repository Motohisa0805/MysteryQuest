using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        private PlayableChracterController  mPlayerController;

        [SerializeField]
        private Transform                   mThrowCircle;
        public Transform                    ThrowCircle => mThrowCircle;

        private LifeHUDController           mLifeHUDController;
        public LifeHUDController            LifeHUDController { get { return mLifeHUDController; }set {  mLifeHUDController = value; } }

        private StaminaHUDController        mStaminaHUDController;
        public StaminaHUDController         StaminaHUDController { get { return mStaminaHUDController; } set { mStaminaHUDController = value; } }

        private ActionButtonController      mButtonController;
        public ActionButtonController       ActionButtonController 
        {
            get 
            {
                if (mButtonController == null)
                {
                    mButtonController = FindAnyObjectByType<ActionButtonController>();
                }
                return mButtonController; 
            }
            set 
            {
                mButtonController = value; 
            } 
        }

        private DotImageController          mDotImageController;
        public DotImageController           DotImageController { get { return mDotImageController; }set{ mDotImageController = value; } }

        private void OnEnable()
        {
            // シーンがロードされた時のイベントに登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 破棄時にイベント解除
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void ReleaseUIStateChanger()
        {
            if (mPlayerController != null && mPlayerController.StateMachine != null)
            {
                mPlayerController.StateMachine.OnStateChanged -= HandleStateChanged;
            }
            mPlayerController = null;
        }

        // シーンがロードされるたびに呼ばれる
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //古いプレイヤーコントローラーのイベント登録を解除
            ReleaseUIStateChanger();
            // 新しいシーンのプレイヤーコントローラーを探してイベント登録
            mPlayerController = FindAnyObjectByType<PlayableChracterController>();
            if(mPlayerController)
            {
                // 新しいプレイヤーコントローラーの状態変更イベントに登録
                mPlayerController.StateMachine.OnStateChanged += HandleStateChanged;
                mPlayableInput = mPlayerController.GetComponent<PlayableInput>();
            }
            else
            {
                mPlayableInput = FindAnyObjectByType<PlayableInput>();
            }
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
            if(mDotImageController)
            {
                if(!mDotImageController.gameObject.activeSelf && mDotImageController.IsEnhanced)
                {
                    mDotImageController.gameObject.SetActive(true);
                }
                else if(mDotImageController.gameObject.activeSelf && !mDotImageController.IsEnhanced)
                {
                    mDotImageController.gameObject.SetActive(false);
                }
            }
        }
        private void HandleStateChanged(StateBase<string> newState)
        {
            // 1. まず全ボタンを非表示にする（リセット）
            ActionButtonController.AllDisableButton();

            // 2. 新しいステートが持っているボタン情報を取得
            var buttons = newState.GetActionButtons();

            // 3. UIに反映
            foreach (var btn in buttons)
            {
                ActionButtonController.ActiveButton(btn.gTag, btn.gLabel);
            }
        }
    }
}
