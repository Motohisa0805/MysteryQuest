using Unity.VisualScripting;
using UnityEngine;

namespace MyAssets
{
    //足部分に関係する演出を処理クラス
    //例：サウンド、エフェクトなど
    public class FootPerformer : MonoBehaviour
    {
        [SerializeField]
        private LayerMask mGroundLayer;
        [SerializeField]
        private float mRaycastDistance = 0.2f;
        public float RaycastDistance => mRaycastDistance;
        [SerializeField]
        private bool mPastGrounded = false;

        private void Start()
        {
            mPastGrounded = false;
        }

        private void FixedUpdate()
        {
            // レイの開始位置を少し高くすると、めり込みにも強くなります
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            bool isGrounded = Physics.SphereCast(origin,0.05f, Vector3.down,out RaycastHit hit, mRaycastDistance + 0.1f, mGroundLayer);

            if (isGrounded && !mPastGrounded)
            {
                SoundManager.Instance.PlayOneShot3D("Foot_Step", transform.position);
            }

            mPastGrounded = isGrounded;
        }
    }
}
