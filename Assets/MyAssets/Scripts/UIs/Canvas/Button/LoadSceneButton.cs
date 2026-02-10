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
        }

        public void SetTag(SceneList sceneTag)
        {
            this.mSceneTag = sceneTag;
        }
    }
}
