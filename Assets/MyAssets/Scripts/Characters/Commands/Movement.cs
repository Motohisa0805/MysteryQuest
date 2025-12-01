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
        private Vector3 mPastVelocity;
        public Vector3 PastVelocity { get { return mPastVelocity; } set { mPastVelocity = value; } }

        [SerializeField]
        private MovementCompensator mMovementCompensator;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            if(mRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found on " + gameObject.name);
            }

            mMovementCompensator.Setup(transform);
        }

        private void Update()
        {
            if (mCurrentVelocity != mPastVelocity)
            {
                mMovementCompensator.HandleStepClimbin();
            }
        }

        public void FixedUpdate()
        {
            mPastVelocity = mRigidbody.linearVelocity;
            Gravity();
            if (mRigidbody.linearVelocity.y < -maxFallSpeed) 
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

            // 3. Y軸速度を強制補正
            // 補正速度を計算
            float stepUpVelocity = CalculateStepUpVelocity(
                mMovementCompensator.StepGoalPosition,
                mMovementCompensator.StepSmooth * 10f // *10fは感度調整
            );

            // XZ成分だけを更新したcurrentVelocityを準備
            currentVelocity.x += accelerationForce.x;
            currentVelocity.z += accelerationForce.z;

            // Y軸の処理
            // StepUpVelocityは、重力などの影響を打ち消して、目標Y座標に移動させるための速度です。
            // 重力の影響を打ち消すため、既存のY軸速度から一旦重力加速度分を引くか、
            // 補正速度を直接現在のY軸速度に加算します。
            if (mMovementCompensator.StepGoalPosition != Vector3.zero)
            {
                // シンプルな加算方式（Gravityの影響は残る）
                currentVelocity.y = stepUpVelocity;
            }

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
        }

        private float CalculateStepUpVelocity(Vector3 targetPosition, float stepSmooth)
        {
            // 段差が検知されていない場合は、補正速度はゼロ
            if (targetPosition == Vector3.zero)
            {
                return 0f;
            }

            // 1. 目標Y座標と現在のY座標の差分（目標までの残りの距離）
            float targetY = targetPosition.y;
            float currentY = transform.position.y;
            float heightDifference = targetY - currentY;

            // 2. 減衰処理を使った速度計算
            // 残りの距離（heightDifference）に滑らかさの係数（stepSmooth）を乗算することで、
            // 目標に近づくにつれて速度が落ちる（Lerpのような効果）垂直速度を算出します。
            // stepSmoothの値が大きいほど、追従速度が速くなります。
            float requiredVelocityY = heightDifference * stepSmooth;

            return requiredVelocityY;
        }
    }
}

