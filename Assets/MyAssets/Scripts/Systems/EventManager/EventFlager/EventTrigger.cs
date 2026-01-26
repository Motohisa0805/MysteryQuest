using UnityEngine;
using UnityEngine.Events;

namespace MyAssets
{
    [RequireComponent(typeof(BoxCollider))]
    public class EventTrigger : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent                  mOnEvent;

        [SerializeField]
        private bool                        mIsRePlay = false;

        private bool                        mHasTriggered = false;

        private PlayableChracterController  mPlayer;

        // Update is called once per frame
        private void Update()
        {
            if(mHasTriggered && InputManager.GetKeyDown(KeyCode.eInteract))
            {
                mOnEvent?.Invoke();
                if (mIsRePlay)
                {
                    mHasTriggered = false;
                }
                else
                {
                    enabled = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            mPlayer = other.GetComponentInParent<PlayableChracterController>();
            if (mPlayer != null)
            {
                if (!mHasTriggered)
                {
                    mHasTriggered = true;
                    if(PlayerUIManager.Instance.ActionButtonController)
                    {
                        PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Right, "Šm”F",20);
                    }
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
                    if (mHasTriggered)
                    {
                        mHasTriggered = false;
                        if (PlayerUIManager.Instance.ActionButtonController)
                        {
                            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Right);
                            mPlayer = null;
                        }
                    }
                }
            }
        }
    }
}
