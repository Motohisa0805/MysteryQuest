using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField]
        private Image[] mImages = new Image[0];

        private float[] mAlpha = new float[0];

        private bool mEnabled = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            mAlpha = new float[mImages.Length];
            if(mImages.Length > 0)
            {
                for(int i = 0; i < mImages.Length; i++)
                {
                    mAlpha[i] = mImages[0].color.a;
                }
            }
            mEnabled = false;
            SetImageEnabled();
        }

        private void SetImageEnabled()
        {
            for (int i = 0; i < mImages.Length; i++)
            {
                mImages[i].enabled = mEnabled;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if(InputManager.GetKeyDown(KeyCode.eTutorialMenu))
            {
                mEnabled = !mEnabled;
                SetImageEnabled();
            }
        }
    }
}
