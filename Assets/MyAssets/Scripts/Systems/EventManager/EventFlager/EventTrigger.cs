using System;
using UnityEngine;
using UnityEngine.Events;

namespace MyAssets
{
    [RequireComponent(typeof(BoxCollider))]
    public class EventTrigger : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent mOnEvent;

        [SerializeField]
        private bool mIsRePlay = false;

        private bool mHasTriggered = false;

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
            PlayableChracterController controller = other.GetComponentInParent<PlayableChracterController>();
            if (controller != null)
            {
                if (!mHasTriggered)
                {
                    mHasTriggered = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayableChracterController controller = other.GetComponentInParent<PlayableChracterController>();
            if (controller != null)
            {
                if (mIsRePlay)
                {
                    mHasTriggered = false;
                }
            }
        }
    }
}
