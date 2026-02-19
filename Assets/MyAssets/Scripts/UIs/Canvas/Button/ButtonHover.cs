using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyAssets
{
    // ボタンホバー時の処理
    /// IPointerEnterHandler, IPointerExitHandlerを実装して、ポインターの入退出イベントを処理
    /// ボタンがホバーされたときにmHoveringをtrueに設定し、ホバーが解除されたときにfalseに設定
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform   mRectTransform;
        public RectTransform    RectTransform => mRectTransform;
        public Image            mImage;
        public Image            Image => mImage;
        [SerializeField]
        private bool            mHovering;
        public bool             IsHovering => mHovering;

        private bool            mPressed;
        public bool             IsPressed => mPressed;

        private Vector2         mBasePos = Vector2.zero;
        private Vector2         mBaseSize = Vector2.one;
        /// サウンドを再生するかどうか
        public virtual bool     IsSound => true;

        public virtual void Awake()
        {
            mRectTransform = GetComponent<RectTransform>();
            mImage = GetComponent<Image>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mHovering = true;
        }

        public IEnumerator OnEnter()
        {
            ChangeButton();
            // 0.05秒待機
            yield return new WaitForSecondsRealtime(0.05f);
            ResetButton();
        }

        public void ChangeButton()
        {
            if (IsSound)
            {
                SoundManager.Instance.PlayOneShot2D("Decide_Button", false);
            }
            mPressed = true;
            // 押された時の演出
            mBasePos = mRectTransform.anchoredPosition;
            mRectTransform.anchoredPosition = new Vector2(mBasePos.x, mBasePos.y - 5.0f);

            mBaseSize = mRectTransform.sizeDelta;
            mRectTransform.sizeDelta = new Vector2(mBaseSize.x * 0.9f, mBaseSize.y * 0.9f);

            if (ColorUtility.TryParseHtmlString("#767676", out Color pressedColor))
            {
                mImage.color = pressedColor;
            }
        }

        public void ResetButton()
        {
            // 2. 元に戻す演出
            mRectTransform.anchoredPosition = mBasePos;
            mRectTransform.sizeDelta = mBaseSize;
            mImage.color = Color.white; // #FFFFFFはColor.whiteで代用可能
            mPressed = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mHovering = false;
        }

        public void SetHovering(bool h)
        {
            mHovering = h;
        }

        private void OnDisable()
        {
            mHovering = false;
        }
    }
}
