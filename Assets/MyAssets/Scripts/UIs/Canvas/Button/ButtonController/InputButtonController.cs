using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class InputButtonController : MonoBehaviour
    {
        //選択中の画像
        [SerializeField]
        private Image           mSelectImage;
        //選択してる要素数
        private int             mSelectIndex;
        //子オブジェクトにボタン
        [SerializeField]
        private Button[]        mButtons = new Button[0];
        private ButtonHover[]   mHovers = new ButtonHover[0];
        [SerializeField]
        private float           mAddDecideButtonSizeOffset = 50f;

        private bool            mDecideFlag;

        private Action          mOnPublicAction;
        public Action           OnPublicAction { get { return mOnPublicAction; }set { mOnPublicAction = value; } }
        private void Awake()
        {
            //配列に要素がなければ
            if(mButtons.Length <= 0)
            {
                Button[] b = GetComponentsInChildren<Button>();
                mButtons = b;
                ButtonHover[] h = GetComponentsInChildren<ButtonHover>();
                mHovers = h;
            }
            //すでにあれば
            else
            {
                foreach(Button b in mButtons)
                {
                    ButtonHover hover = b.GetComponent<ButtonHover>();
                    if (hover)
                    {
                        Array.Resize(ref mHovers, mHovers.Length + 1);
                        mHovers[mHovers.Length - 1] = hover;
                    }
                }
            }

        }

        private void Start()
        {

            mDecideFlag = false;
            mSelectIndex = 0;
            SetSelectImagePosition(mSelectIndex);
        }
        //選択画像のサイズを決定
        private void SetSelectImageSize()
        {
            if (mSelectImage == null) { return; }
            Vector2 size = mSelectImage.rectTransform.sizeDelta;
            size.x += mAddDecideButtonSizeOffset;
            size.y += mAddDecideButtonSizeOffset;
            mSelectImage.rectTransform.sizeDelta = size;
        }

        //選択中のボタン位置に選択画像を移動
        private void SetSelectImagePosition(int index)
        {
            if (mSelectImage == null) { return; }
            if (index < 0) { return; }
            Vector2 pos = mHovers[index].RectTransform.anchoredPosition;
            mSelectImage.rectTransform.anchoredPosition = pos;
            mSelectImage.rectTransform.sizeDelta = mHovers[index].RectTransform.sizeDelta;
            SetSelectImageSize();
            mSelectImage.rectTransform.localScale = mHovers[index].RectTransform.localScale;
        }
        //選択画像の有効無効
        private void SetActivateSelectImage(bool b)
        {
            mSelectImage.enabled = b;
        }
        private void Update()
        {
            if(InputManager.IsCurrentControlSchemeKeyBoard)
            {
                MouseInput();
            }
            else
            {
                GamePadInput();
            }

        }
        //マウス入力処理
        private void MouseInput()
        {
            for (int i = 0; i < mHovers.Length; i++)
            {
                if (mHovers[i].IsHovering)
                {
                    if (mSelectIndex != i)
                    {
                        SoundManager.Instance.PlayOneShot2D("Select_Button", false);
                        mSelectIndex = i;
                        SetSelectImagePosition(mSelectIndex);
                        SetActivateSelectImage(true);
                    }
                }
            }
        }
        //ゲームパッド入力処理
        private void GamePadInput()
        {
            Vector2 selectVec2 = Vector2.zero;
            if(InputManager.GetKeyDown(KeyCode.eUpSelect))
            {
                selectVec2.y = 1;
            }
            else if(InputManager.GetKeyDown(KeyCode.eDownSelect))
            {
                selectVec2.y = -1;
            }
            if(InputManager.GetKeyDown(KeyCode.eLeftSelect))
            {
                selectVec2.x = -1;
            }
            else if(InputManager.GetKeyDown(KeyCode.eRightSelect))
            {
                selectVec2.x = 1;
            }
            if(selectVec2 != Vector2.zero)
            {
                SelectVec2Input(selectVec2);
            }
            if (InputManager.GetKeyDown(KeyCode.eDecide))
            {
                OnDecide();
            }
        }
        //ベクトル入力処理
        private void SelectVec2Input(Vector2 select)
        {
            int currentIndex = mSelectIndex;

            int decideIndex = -1;
            for (int i = 0; i < mHovers.Length; i++)
            {
                if (!mHovers[i].transform.parent.gameObject.activeSelf) continue;

                if (select.x > 0)
                {
                    if (mHovers[currentIndex].RectTransform.anchoredPosition.x < mHovers[i].RectTransform.anchoredPosition.x)
                    {
                        decideIndex = CheckDecideIndex(currentIndex, decideIndex, i);
                    }
                }
                else if (select.x < 0)
                {
                    if (mHovers[currentIndex].RectTransform.anchoredPosition.x > mHovers[i].RectTransform.anchoredPosition.x)
                    {
                        decideIndex = CheckDecideIndex(currentIndex, decideIndex, i);
                    }
                }

                if (select.y > 0)
                {
                    if (mHovers[currentIndex].RectTransform.anchoredPosition.y < mHovers[i].RectTransform.anchoredPosition.y)
                    {
                        decideIndex = CheckDecideIndex(currentIndex, decideIndex, i);
                    }
                }
                else if (select.y < 0)
                {
                    if (mHovers[currentIndex].RectTransform.anchoredPosition.y > mHovers[i].RectTransform.anchoredPosition.y)
                    {
                        decideIndex = CheckDecideIndex(currentIndex, decideIndex, i);
                    }
                }
            }
            if (decideIndex < 0) { return; }
            SoundManager.Instance.PlayOneShot2D("Select_Button", false);
            mSelectIndex = decideIndex;
            SetSelectImagePosition(mSelectIndex);
        }
        //決定インデックスチェック
        private int CheckDecideIndex(int currentNum, int decideNum, int newNum)
        {
            if (decideNum < 0) { return newNum; }
            Vector2 currentSub = mHovers[currentNum].RectTransform.anchoredPosition - mHovers[decideNum].RectTransform.anchoredPosition;
            Vector2 newSub = mHovers[currentNum].RectTransform.anchoredPosition - mHovers[newNum].RectTransform.anchoredPosition;

            if (Mathf.Abs(currentSub.magnitude) > Mathf.Abs(newSub.magnitude))
            {
                return newNum;
            }
            return decideNum;
        }
        //決定処理
        public void OnDecide()
        {
            if (mDecideFlag) { return; }
            StartCoroutine(DecideUpdate());
        }
        //決定コルーチン
        private IEnumerator DecideUpdate()
        {
            //mDecideFlag = true;
            StartCoroutine(mHovers[mSelectIndex].OnEnter());
            yield return new WaitForSecondsRealtime(0.1f);
            mButtons[mSelectIndex].onClick?.Invoke();
            //外部からの処理があるなら
            mOnPublicAction?.Invoke();
        }

        public void ActivateStart()
        {
            this.enabled = true;
        }
    }
}
