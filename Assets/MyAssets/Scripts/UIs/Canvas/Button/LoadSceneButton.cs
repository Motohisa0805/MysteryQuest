using UnityEngine;

namespace MyAssets
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField]
        public SceneList sceneTag;
        public void LoadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene((int)sceneTag);
        }

        public void SetTag(SceneList sceneTag)
        {
            this.sceneTag = sceneTag;
        }
    }
}
