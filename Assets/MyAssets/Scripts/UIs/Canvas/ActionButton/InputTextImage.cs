using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class InputTextImage : ButtonHover
    {
        private Text    mText;
        public Text     Text => mText;

        [SerializeField]
        private int     mID;
        public int      ID => mID;

        private bool    mLookImage = false;
        public bool     LookImage
        {
            get { return mLookImage; }
            set { mLookImage = value; }
        }

        public override bool IsSound => false;

        public override void Awake()
        {
            base.Awake();
            mText = GetComponentInChildren<Text>();
        }

        public void SetText(string text,int size = 15,bool look = false)
        {
            mLookImage = look;
            mText.text = text;
            mText.fontSize = size;
        }
    }
}
