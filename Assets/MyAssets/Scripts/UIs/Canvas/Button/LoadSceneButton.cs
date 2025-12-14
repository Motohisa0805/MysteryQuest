using UnityEngine;

namespace MyAssets
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField]
        public SceneList sceneTag;

        private BlackoutController mBlackoutController;
        public void LoadScene()
        {
            mBlackoutController = FindAnyObjectByType<BlackoutController>();
            mBlackoutController.StartBlackout((int)sceneTag);
        }

        public void SetTag(SceneList sceneTag)
        {
            this.sceneTag = sceneTag;
        }
    }
}
