using UnityEngine;

namespace MyAssets
{
    //全シーンで使えるUIの処理を行う
    //Canvasにアタッチする
    public class CanvasController : MonoBehaviour
    {
        private BlackoutController mBlackoutController;


        private void Awake()
        {
            mBlackoutController = FindAnyObjectByType<BlackoutController>();
        }

        private void Start()
        {
            if(mBlackoutController != null)
            {
                mBlackoutController.StartFadeIn();
            }
        }
    }
}
