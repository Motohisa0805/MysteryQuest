using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class DotImageController : MonoBehaviour
    {
        private float   mSize = 1.0f;

        private Color   mColor = Color.white;

        private Vector2 mOriginalSize = new Vector2(20.0f, 20.0f);

        private Image   mImage;

        private bool    mEnabled = false;
        public bool     IsEnhanced {  get { return mEnabled; }set { mEnabled = value; } }
        private void Awake()
        {
            mImage = GetComponent<Image>();
        }

        private void Start()
        {
            PlayerUIManager.Instance.DotImageController = this;
            mImage.color = mColor;
            mImage.rectTransform.sizeDelta = mOriginalSize * mSize;
            gameObject.SetActive(false);
        }
    }
}
