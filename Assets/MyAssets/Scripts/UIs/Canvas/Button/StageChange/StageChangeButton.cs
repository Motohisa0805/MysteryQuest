using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class StageChangeButton : MonoBehaviour
    {
        private Button mButton;
        [SerializeField]
        private StageSelectController mController;

        [SerializeField]
        private bool mCorrectFlag = false;
        private void SetPageCount()
        {
            if (mCorrectFlag)
            {
                mController.ChangeStageInformationPage(1);
            }
            else
            {
                mController.ChangeStageInformationPage(-1);
            }
            SoundManager.Instance.PlayOneShot2D(1004, false);
        }

        private void Awake()
        {
            if(mController == null)
            {
                mController = FindAnyObjectByType<StageSelectController>();
            }
            mButton = GetComponent<Button>();
            mButton.onClick.AddListener(SetPageCount);
        }

        private void OnDisable()
        {
            mButton.onClick.RemoveListener(SetPageCount);
        }
    }
}

