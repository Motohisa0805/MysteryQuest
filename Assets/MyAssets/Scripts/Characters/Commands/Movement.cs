using UnityEngine;

namespace MyAssets
{
    // 移動処理全般を担当するクラス
    public class Movement : MonoBehaviour
    {
        [SerializeField]
        private float               mGravityMultiply;

        private Rigidbody           mRigidbody;
        public Rigidbody            Rigidbody => mRigidbody;

        [SerializeField]
        private Vector3             mCurrentInputVelocity;
        public Vector3              CurrentInputVelocity {  get { return mCurrentInputVelocity; } set { mCurrentInputVelocity = value; } }

        private UpTimer             mClimbJumpingTimer = new UpTimer();

        public UpTimer              ClimbJumpingTimer => mClimbJumpingTimer;

        [SerializeField]
        private MovementCompensator mMovementCompensator;
        public MovementCompensator  MovementCompensator => mMovementCompensator;


        [SerializeField]
        private GravityCorrection   mGravityCorrection;
        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            if(mRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found on " + gameObject.name);
            }

            mMovementCompensator.Setup(transform);
        }

        private bool IsMovementChanged()
        {
            Vector2 current = new Vector2(mCurrentInputVelocity.x, mCurrentInputVelocity.z);
            return current.magnitude > 0;
        }

        //TODO : 応急処置、本来はMovement独立で判断するべきなので
        //オブジェクトがPropsObjectCheckerを持っていたら判定するようにしている
        //持っていなかったら常にtrueを返す
        private bool IsPropCheck()
        {
            PropsObjectChecker checker = GetComponent<PropsObjectChecker>();
            if (!checker)
            {
                return true;
            }
            return !checker.PushEnabled && !checker.HasTakedObject;
        }

        private void Update()
        {
            //よじ登りの時間管理
            mClimbJumpingTimer.Update(Time.deltaTime);
            // 一定の条件で段差チェック
            if (IsMovementChanged() && mMovementCompensator.Difference == 0.0f && IsPropCheck())
            {
                mMovementCompensator.HandleStepClimbin();
            }
        }

        public void FixedUpdate()
        {
            mGravityCorrection.Correction(mRigidbody);
        }
        // 停止処理
        public void Stop()
        {
            Vector3 currentVelocity = mRigidbody.linearVelocity;
            currentVelocity.x = 0;
            currentVelocity.z = 0;
            mRigidbody.linearVelocity = currentVelocity;
        }
        // 移動処理
        public void Move(float maxSpeed,float accele)
        {
            // 現在の速度を代入 (velocityに修正)
            Vector3 currentVelocity = mRigidbody.linearVelocity;

            // 加速の目標方向と大きさ
            // mCurrentVelocityは既に正規化済みのベクトル（長さ0～1）
            Vector3 targetVelocity = mCurrentInputVelocity * maxSpeed;

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
            Vector3 direction = transform.forward * mCurrentInputVelocity.magnitude;

            Vector3 currentVelocity = mRigidbody.linearVelocity;

            Vector3 targetVelocityXZ = direction * speed;

            currentVelocity.x = targetVelocityXZ.x;
            currentVelocity.z = targetVelocityXZ.z;

            mRigidbody.linearVelocity = currentVelocity;
        }

        public void Jump(float power)
        {
            /*
            float force = power;
            if(mRigidbody.linearVelocity.y < 0.0f)
            {
                force += Mathf.Abs(mRigidbody.linearVelocity.y) * mRigidbody.mass;
            }
            //上方向に力を加える
            mRigidbody.AddForce(Vector3.up * power, ForceMode.VelocityChange);
             */
            // 現在の速度を取得
            Vector3 velocity = mRigidbody.linearVelocity;

            // Y軸方向の速度だけをリセットし、計算したジャンプ速度を代入する
            // ジャンプ速度の計算式: v = sqrt(2 * gravity * height)
            float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * power);

            mRigidbody.linearVelocity = new Vector3(velocity.x, jumpVelocity, velocity.z);
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

        // 登りの動きを処理する関数
        public void Climb()
        {
            if (!mMovementCompensator.IsClimbJumping&& !mMovementCompensator.IsClimb) { return; }
            //  よじ登りの時間は指定時間で行う
            //仮
            float f = mClimbJumpingTimer.GetNormalize();
            //  左右は後半にかけて早く移動する
            float x = Mathf.Lerp(mMovementCompensator.StepStartPosition.x, mMovementCompensator.StepGoalPosition.x, Ease(f));
            float z = Mathf.Lerp(mMovementCompensator.StepStartPosition.z, mMovementCompensator.StepGoalPosition.z, Ease(f));
            //  上下は等速直線で移動
            float y = Mathf.Lerp(mMovementCompensator.StepStartPosition.y, mMovementCompensator.StepGoalPosition.y, f);
            //  座標を更新
            mRigidbody.MovePosition(new Vector3(x, y, z));
        }
        //  イージング関数
        private float Ease(float x)
        {
            return x * x * x;
        }

        //段差の高さ分ジャンプする処理
        public void ClimbJump(float h)
        {
            float g = Mathf.Abs(Physics.gravity.y) * 2.0f;
            float requiredVelocityY = Mathf.Sqrt(2 * g * h);

            // 必要な垂直速度を瞬間的に加える
            mRigidbody.AddForce(Vector3.up * requiredVelocityY, ForceMode.VelocityChange);
        }
    }
}

