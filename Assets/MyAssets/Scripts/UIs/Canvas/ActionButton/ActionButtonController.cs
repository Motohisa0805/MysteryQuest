using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public struct ActionButtonInfo
    {
        public int gTag;
        public string gLabel;

        public ActionButtonInfo(int tag, string label)
        {
            gTag = tag;
            gLabel = label;
        }
    }
    //ボタン操作のUIの管理処理クラス
    public class ActionButtonController : MonoBehaviour
    {
        public enum ActionButtonTag
        {
            None = -1,
            Up = 0,
            Down,
            Left,
            Right,
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

            ActiveButton(4, "メニュー");
            ActiveButton(5, "L1");
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

        public void CheckAndSetButtonUI()
        {

        }

        public void ActiveButton(int id,string text,int fontsize = 20,bool look = false)
        {
            foreach (var image in mButtonTextures)
            {
                if(image.ID == id)
                {
                    image.gameObject.SetActive(true);
                    image.SetText(text,fontsize, look);
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

        public void DisableButton(int id,bool look = false)
        {
            foreach (var image in mButtonTextures)
            {
                if (image.ID == id)
                {
                    image.LookImage = look;
                    image.gameObject.SetActive(false);
                    break;
                }
            }
        }

        public void AllDisableButton()
        {
            foreach (var image in mButtonTextures)
            {
                if (!image.LookImage)
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

    }
}
