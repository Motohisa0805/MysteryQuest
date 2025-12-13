using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyAssets
{
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform mRectTransform;
        public RectTransform RectTransform => mRectTransform;
        [SerializeField]
        private bool mHovering;
        public bool IsHovering => mHovering;

        private void Awake()
        {
            mRectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mHovering = true;
        }

        private IEnumerator OnEnter()
        {
            yield return null;
            mHovering = false;
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
