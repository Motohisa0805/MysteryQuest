using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ボタン操作のUIの管理処理クラス
    public class ActionButtonController : MonoBehaviour
    {
        public enum ActionButtonTag
        {
            None = -1,
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
        }

        private List<InputTextImage> mButtonTextures = new List<InputTextImage>();

        private void Awake()
        {
            InputTextImage[] inputTextImages = GetComponentsInChildren<InputTextImage>();
            mButtonTextures.Clear();
            mButtonTextures = new List<InputTextImage>(inputTextImages);
            foreach(var image in inputTextImages)
            {
                image.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            PlayerUIManager.Instance.ActionButtonController = this;
        }

        public void UpdateActionButton()
        {
            if (PlayerUIManager.Instance.PlayableInput.InputJump)
            {
                EnterButton((int)ActionButtonTag.Up);
            }
            if (PlayerUIManager.Instance.PlayableInput.Sprit)
            {
                StayButton((int)ActionButtonTag.Down);
            }
            else
            {
                ExitButton((int)ActionButtonTag.Down);
            }
            if (PlayerUIManager.Instance.PlayableInput.Attack)
            {
                EnterButton((int)ActionButtonTag.Left);
            }
            if (PlayerUIManager.Instance.PlayableInput.Interact)
            {
                EnterButton((int)ActionButtonTag.Right);
            }

        }

        public void ActiveButton(int id,string text,int fontsize = 20)
        {
            foreach (var image in mButtonTextures)
            {
                if(image.ID == id)
                {
                    image.gameObject.SetActive(true);
                    image.SetText(text,fontsize);
                    break;
                }
            }
        }


        public void EnterButton(int id)
        {
            foreach (var image in mButtonTextures)
            {
                if (!image.gameObject.activeSelf) { continue; }
                if (image.ID == id)
                {
                    if (image.IsPressed) { return; }
                    StartCoroutine(image.OnEnter());
                    break;
                }
            }
        }

        public void StayButton(int id)
        {
            foreach (var image in mButtonTextures)
            {
                if (!image.gameObject.activeSelf) { continue; }
                if (image.ID == id)
                {
                    if (image.IsPressed) { return; }
                    image.ChangeButton();
                    break;
                }
            }
        }
        public void ExitButton(int id)
        {
            foreach (var image in mButtonTextures)
            {
                if (!image.gameObject.activeSelf) { continue; }
                if (image.ID == id)
                {
                    if (!image.IsPressed) { return; }
                    image.ResetButton();
                    break;
                }
            }
        }

        public void DisableButton(int id)
        {
            foreach (var image in mButtonTextures)
            {
                if (image.ID == id)
                {
                    image.gameObject.SetActive(false);
                    break;
                }
            }
        }

        public void AllDisableButton()
        {
            foreach (var image in mButtonTextures)
            {
                image.gameObject.SetActive(false);
            }
        }

    }
}
