using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class TutorialMovementer : MonoBehaviour
    {
        //ボタン管理クラス
        private InputButtonController   mInputButtonController;
        //操作するImage変数
        [SerializeField]
        private Image                   mMoveImage;

        private float                   mStartPosY = 0;

        private float                   mEndPosY = 0;

        private float                   mMoveTime = 0.1f;
        private float                   mCurrentMoveTime = 0.0f;

        private Coroutine               mMoveCoroutine;

        private void Awake()
        {
            InitSetting();
        }

        private void InitSetting()
        {
            mInputButtonController = GetComponentInChildren<InputButtonController>();
            if (mInputButtonController != null)
            {
                mInputButtonController.OnPublicAction += DisableImage;
            }
        }

        private void InitMoveImage()
        {
            if(mMoveImage == null) { return; }
            mCurrentMoveTime = 0.0f;
            mStartPosY = 1000f;
            mEndPosY = mMoveImage.rectTransform.anchoredPosition.y;
            Time.timeScale = 0;
            InputManager.SetNoneMouseMode();
            ResultManager.IsStopGameUIInput = true;
            SoundManager.Instance.PlayOneShot2D("Description_Window_Open");
            if (mMoveCoroutine != null) StopCoroutine(mMoveCoroutine);
            mMoveCoroutine = StartCoroutine(MoveImage());
        }

        private IEnumerator MoveImage()
        {
            //念のため、開始直前に初期座標へ配置
            mMoveImage.rectTransform.anchoredPosition = new Vector2(0, mStartPosY);

            while (mCurrentMoveTime < mMoveTime)
            {
                mCurrentMoveTime += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(mCurrentMoveTime / mMoveTime);

                mMoveImage.rectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(mStartPosY, mEndPosY, progress));
                yield return null;
            }

            mMoveImage.rectTransform.anchoredPosition = new Vector2(0, mEndPosY); // 最後にピタッと止める
            mMoveCoroutine = null;
        }

        public void DisableImage()
        {
            Time.timeScale = 1;
            InputManager.SetLockedMouseMode();
            ResultManager.IsStopGameUIInput = false;
            SoundManager.Instance.PlayOneShot2D("Decide_Button");
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            InitSetting();
            if ( mInputButtonController != null )
            {
                mInputButtonController.OnPublicAction += DisableImage;
            }
            InitMoveImage();
        }

        private void OnDisable()
        {
            if (mInputButtonController != null)
            {
                mInputButtonController.OnPublicAction -= DisableImage;
            }
        }
    }
}
