using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class InputTextImage : ButtonHover
    {
        private Text mText;
        public Text Text => mText;

        [SerializeField]
        private int mID;
        public int ID => mID;

        public override void Awake()
        {
            base.Awake();
            mText = GetComponentInChildren<Text>();
        }

        public void SetText(string text,int size = 15)
        {
            mText.text = text;
            mText.fontSize = size;
        }
    }
}
