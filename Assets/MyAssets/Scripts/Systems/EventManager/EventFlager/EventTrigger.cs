using UnityEngine;
using UnityEngine.Events;

namespace MyAssets
{
    [RequireComponent(typeof(BoxCollider))]
    public class EventTrigger : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent                  mOnEvent;
        [Header("再度使えるようにするか")]
        [SerializeField]
        private bool                        mIsRePlay = false;
        [Header("コライダーに当たって起動するか【true:当たって起動、false:入力で起動】")]
        [SerializeField]
        private bool                        mHitPlay = false;


        private bool                        mHasTriggered = false;

        private PlayableChracterController  mPlayer;

        // Update is called once per frame
        private void Update()
        {
            if(mHasTriggered && InputManager.GetKeyDown(KeyCode.eInteract))
            {
                EventInvoke();
            }
        }

        private void EventInvoke()
        {
            mOnEvent?.Invoke();
            if (mIsRePlay)
            {
                mHasTriggered = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            mPlayer = other.GetComponentInParent<PlayableChracterController>();
            if (mPlayer != null)
            {
                if(!mHitPlay)
                {
                    if (!mHasTriggered)
                    {
                        mHasTriggered = true;
                        if(PlayerUIManager.Instance.ActionButtonController)
                        {
                            PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Right, "確認",20,true);
                        }
                    }
                }
                else
                {
                    EventInvoke();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayableChracterController controller = other.GetComponentInParent<PlayableChracterController>();
            if (controller != null)
            {
                if(mPlayer == controller)
                {
                    if (!mHitPlay)
                    {
                        if (mHasTriggered)
                        {
                            mHasTriggered = false;
                            if (PlayerUIManager.Instance.ActionButtonController)
                            {
                                PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Right,false);
                                mPlayer = null;
                            }
                        }
                    }
                    else
                    {
                        mHasTriggered = false;
                    }
                }
            }
        }
    }
}
