using UnityEngine;

namespace MyAssets
{
    public class Movement : MonoBehaviour
    {
        [SerializeField]
        private float mGravityMultiply;

        [SerializeField]
        private float maxFallSpeed = 20f;


        private Rigidbody mRigidbody;


        private Vector3 mCurrentVelocity;

        public Vector3 CurrentVelocity {  get { return mCurrentVelocity; } set { mCurrentVelocity = value; } }


        private bool mJumping;

        public bool Jumping => mJumping;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            if(mRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found on " + gameObject.name);
            }
        }

        public void FixedUpdate()
        {
            if(mRigidbody.linearVelocity.y < -maxFallSpeed) 
            {
                mRigidbody.linearVelocity = new Vector3(mRigidbody.linearVelocity.x, -maxFallSpeed, mRigidbody.linearVelocity.z); 
            }
        }


        public void Move(float maxSpeed,float accele)
        {
            // 現在の速度を代入 (velocityに修正)
            Vector3 currentVelocity = mRigidbody.linearVelocity;

            // 加速の目標方向と大きさ
            // mCurrentVelocityは既に正規化済みのベクトル（長さ0～1）
            Vector3 targetVelocity = mCurrentVelocity * maxSpeed;

            // 現在のXZ速度と目標XZ速度の差分を計算
            // この差分が「必要な加速」となる
            Vector3 velocityChange = targetVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);

            // 加速の大きさを制限 (一気に加速するのを防ぐ)
            Vector3 accelerationForce = velocityChange.normalized * accele * Time.deltaTime;

            // ただし、目標速度を超えないようにする
            if (velocityChange.magnitude < accelerationForce.magnitude)
            {
                accelerationForce = velocityChange;
            }

            // 加速を現在の速度に適用
            currentVelocity.x += accelerationForce.x;
            currentVelocity.z += accelerationForce.z;
            //Debug.Log(currentVelocity);

            // Y軸速度はそのまま維持
            // XZ成分だけを更新したcurrentVelocityをリジッドボディに代入 (velocityに修正)
            mRigidbody.linearVelocity = currentVelocity;
        }

        public void PushObjectMove(float speed)
        {
            //プレイヤーの移動
            Vector3 direction = transform.forward * mCurrentVelocity.magnitude;

            Vector3 currentVelocity = mRigidbody.linearVelocity;

            Vector3 targetVelocityXZ = direction * speed;

            currentVelocity.x = targetVelocityXZ.x;
            currentVelocity.z = targetVelocityXZ.z;

            mRigidbody.linearVelocity = currentVelocity;
        }
        public void Gravity()
        {
            mRigidbody.linearVelocity += Physics.gravity * mGravityMultiply * Time.deltaTime;
        }

        public void Jump(float power)
        {
            //上方向に力を加える
            mRigidbody.AddForce(Vector3.up * power, ForceMode.VelocityChange);
            mJumping = true;
        }

        public void JumpFragReset()
        {
            mJumping = false;
        }
    }
}

