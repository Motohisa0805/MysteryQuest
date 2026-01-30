using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class TitleTextObject : MonoBehaviour
    {
        private Text mText;
        public Text Text => mText;

        private void Awake()
        {
            mText = GetComponent<Text>();
        }
    }
}
