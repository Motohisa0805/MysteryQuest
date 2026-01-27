using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField]
        public SceneList            mSceneTag;

        private BlackoutController  mBlackoutController;
        public void LoadScene()
        {
            if (mBlackoutController != null)
            {
                mBlackoutController.StartBlackout((int)mSceneTag);
            }
            else
            {
                mBlackoutController = FindAnyObjectByType<BlackoutController>();
                mBlackoutController.StartBlackout((int)mSceneTag);
            }
            SoundManager.Instance.PlayOneShot2D("Decide_Button", false);
        }

        public void ReLoadScene()
        {
            if (mBlackoutController != null)
            {
                mBlackoutController.StartBlackout(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                mBlackoutController = FindAnyObjectByType<BlackoutController>();
                mBlackoutController.StartBlackout(SceneManager.GetActiveScene().buildIndex);
            }
            SoundManager.Instance.PlayOneShot2D("Decide_Button", false);
        }

        public void SetTag(SceneList sceneTag)
        {
            this.mSceneTag = sceneTag;
        }
    }
}
