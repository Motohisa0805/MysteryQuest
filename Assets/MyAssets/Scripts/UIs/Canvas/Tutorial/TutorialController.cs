using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    //チュートリアルの表示を制御するクラス
    // チュートリアル画像の表示・非表示を切り替える
    public class TutorialController : MonoBehaviour
    {
        [SerializeField]
        private Image[]                 mImages = new Image[0];

        private float[]                 mAlpha = new float[0];

        private bool                    mEnabled = false;

        [SerializeField]
        private TutorialMovementer[]    mTutorialMovementers = new TutorialMovementer[0];
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

        public void ActiveTutorial(int index)
        {
            //配列範囲外なら
            if(index > mTutorialMovementers.Length - 1) { return; }
            mTutorialMovementers[index].gameObject.SetActive(true);
        }
    }
}
