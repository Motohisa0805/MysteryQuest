using UnityEngine;

namespace MyAssets
{
    //奈落エリアを管理するクラス
    public class AbyssArea : MonoBehaviour
    {
        private Vector3 mResetPosint;

        private Collider mCollider;

        public void ObjectPositionReset()
        {
            if (!mCollider) return;

            mCollider.transform.position = mResetPosint;

            mCollider = null;
            mResetPosint = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ResultManager.IsPlayerDeath) return;

            LandingChecker landingChecker = other.GetComponent<LandingChecker>();
            if (landingChecker != null)
            {
                mCollider = other;
                mResetPosint = landingChecker.LastSafePosition;
                //ここからコルーチンで処理
                EventManager.Instance.AbyssFallEvent().Forget();
            }
        }
    }
}
