using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField]
        public SceneList sceneTag;

        private BlackoutController mBlackoutController;
        public void LoadScene()
        {
            if (mBlackoutController != null)
            {
                mBlackoutController.StartBlackout((int)sceneTag);
            }
            else
            {
                mBlackoutController = FindAnyObjectByType<BlackoutController>();
                mBlackoutController.StartBlackout((int)sceneTag);
            }
            SoundManager.Instance.PlayOneShot2D(1004, false);
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
            SoundManager.Instance.PlayOneShot2D(1004, false);
        }

        public void SetTag(SceneList sceneTag)
        {
            this.sceneTag = sceneTag;
        }
    }
}
