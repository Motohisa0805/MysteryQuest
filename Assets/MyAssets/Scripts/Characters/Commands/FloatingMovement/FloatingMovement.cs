using UnityEngine;

namespace MyAssets
{
    public class FloatingMovement : MonoBehaviour
    {
        private Rigidbody mRigidbody;
        public Rigidbody Rigidbody => mRigidbody;

        [SerializeField]
        private Vector3 mCurrentInputVelocity;
        public Vector3 CurrentInputVelocity { get { return mCurrentInputVelocity; } set { mCurrentInputVelocity = value; } }

        [Header("Movement Limit")]
        [SerializeField]
        private Transform mAnchorTransform;
        [SerializeField]
        private float mLimitRadius = 5.0f;


        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            if (mRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found on " + gameObject.name);
            }
        }

        // 起点を設定するメソッド（外部から呼ぶ用）
        public void SetAnchor(Transform anchor, float radius)
        {
            mAnchorTransform = anchor;
            mLimitRadius = radius;
        }

        // 移動処理
        public void Move(float maxSpeed, float accele)
        {
            Vector3 currentVelocity = mRigidbody.linearVelocity;
            Vector3 targetVelocity = mCurrentInputVelocity * maxSpeed;

            // --- ここから制限処理の追加 ---
            if (mAnchorTransform != null)
            {
                // 中心からのベクトル
                Vector3 offset = transform.position - mAnchorTransform.position;
                float distance = offset.magnitude;

                // 半径を超えている、かつ、さらに外側へ行こうとしている場合
                if (distance > mLimitRadius)
                {
                    // 中心へ向かう方向（正規化）
                    Vector3 toCenter = -offset.normalized;

                    // 目標速度が「外向き」成分を持っているかチェック (内積)
                    // ドット積がマイナス＝外側に向かっている
                    if (Vector3.Dot(targetVelocity, toCenter) < 0)
                    {
                        // 外向きの速度成分を打ち消す（壁ずり移動が可能になる）
                        // targetVelocityを「中心へ向かう面」に投影する
                        targetVelocity = Vector3.ProjectOnPlane(targetVelocity, toCenter);
                    }

                    // オプション: 完全に外に出ている場合、少しだけ中心に戻す力を足すと安定します
                    // （これがないと境界線上でピタッと止まりにくい場合があります）
                    Vector3 returnForce = toCenter * (distance - mLimitRadius) * 2.0f; // 係数は適宜調整
                    targetVelocity += returnForce;
                }
            }
            // --- 制限処理ここまで ---

            Vector3 velocityChange = targetVelocity - currentVelocity;
            Vector3 accelerationForce = velocityChange.normalized * accele * Time.deltaTime;

            if (velocityChange.magnitude < accelerationForce.magnitude)
            {
                accelerationForce = velocityChange;
            }

            currentVelocity += accelerationForce;
            mRigidbody.linearVelocity = currentVelocity;
        }
    }
}
