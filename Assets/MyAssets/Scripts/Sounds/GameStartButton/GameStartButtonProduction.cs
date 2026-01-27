using UnityEngine;

namespace MyAssets
{
    //セレクトのゲームスタートボタンでしか使わない
    //クラス
    public class GameStartButtonProduction : MonoBehaviour
    {
        public void DecideProduction()
        {
            SoundManager.Instance.PlayOneShot2D("GameStart_Decide");
        }
    }
}
