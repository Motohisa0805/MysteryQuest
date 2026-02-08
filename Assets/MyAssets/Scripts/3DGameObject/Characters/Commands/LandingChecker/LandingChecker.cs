using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class LandingChecker : MonoBehaviour
    {
        // 最後に安全だった場所
        private Vector3                     mLastSafePosition;
        public Vector3                      LastSafePosition => mLastSafePosition;
        // 保存間隔
        [SerializeField]
        private float                       mUpdateInterval = 0.5f;
        private PlayableChracterController  mController;

        private void Awake()
        {
            mController = GetComponent<PlayableChracterController>();
        }

        private void Start()
        {
            //初期位置を最初の安全圏として保存
            mLastSafePosition = transform.position;
            //定期的に座標を保存するループを開始
            StartCoroutine(UpdateSafePosition());
        }

        private IEnumerator UpdateSafePosition()
        {
            while (true)
            {
                // キャラクターが地上にいる場合、現在の位置を安全な位置として保存
                if (CheckIfGrounded())
                {
                    mLastSafePosition = transform.position;
                }
                yield return new WaitForSeconds(mUpdateInterval);
            }
        }

        private bool CheckIfGrounded()
        {
            return mController != null ? mController.Grounded : true;
        }
    }
}
