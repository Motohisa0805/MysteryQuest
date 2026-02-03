using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class OutputText : MonoBehaviour
    {
        private Text mText;

        private void Awake()
        {
            mText = GetComponent<Text>();
            if (mText == null)
            {
                Debug.LogError("Not Find Text Component" + gameObject.name);
            }
        }

        private void OnEnable()
        {
            if(mText == null)
            {
                mText = GetComponent<Text>();
            }
        }

        public void SetText(string text)
        {
            if (mText == null)
            {
                mText = GetComponent<Text>();
            }
            mText.text = text;
        }
    }
}
