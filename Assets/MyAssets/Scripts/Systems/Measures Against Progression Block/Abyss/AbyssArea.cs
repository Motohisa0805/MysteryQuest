using UnityEngine;

namespace MyAssets
{

    public class AbyssArea : MonoBehaviour
    {
        private Vector3 mResetPosint;

        private Collider mCollider;

        private BlackoutController mBlackoutController;

        private void Awake()
        {
            mBlackoutController = FindAnyObjectByType<BlackoutController>();
        }

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
                //Ç±Ç±Ç©ÇÁÉRÉãÅ[É`ÉìÇ≈èàóù
                EventManager.Instance.AbyssFallEvent().Forget();
            }
        }
    }
}
