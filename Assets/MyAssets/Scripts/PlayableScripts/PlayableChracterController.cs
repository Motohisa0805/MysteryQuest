using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.VersionControl.Asset;

namespace MyAssets
{
    [RequireComponent(typeof(PlayableInput))]
    [RequireComponent(typeof(Movement))]
    public class PlayableChracterController : MonoBehaviour
    {
        [SerializeField]
        private string mCurrentStateKey; //現在の状態
        [SerializeField]
        private StateMachine<string> stateMachine;
        public StateMachine<string> StateMachine => stateMachine;

        StateBase<string>[] states;

        [SerializeField]
        private IdleState mIdleState;
        [SerializeField]
        private MoveState mMoveState;
        [SerializeField]
        private JumpUpState mJumpUpState;
        [SerializeField]
        private JumpingState mJumpState;
        [SerializeField]
        private JumpDownState mJumpDownState;
        [SerializeField]
        private StandingToCrouchState mStandingToCrouchState;
        [SerializeField]
        private CrouchIdleState mCrouchIdleState;
        [SerializeField]
        private CrouchWalkState mCrouchMoveState;
        [SerializeField]
        private CrouchToStandingState mCrouchToStandingState;
        [SerializeField]
        private FallState mFallState;

        [SerializeField]
        private float mMaxSpeed; //最高速度
        public float MaxSpeed => mMaxSpeed;

        [SerializeField]
        private float mCrouchMaxSpeed; //しゃがみ時の最高速度
        public float CrouchMaxSpeed => mCrouchMaxSpeed;

        [SerializeField]
        private float mRotationSpeed; //回転速度


        private Rigidbody mRigidbody; //リジッドボディ

        private Animator mAnimator; //アニメーター

        private PlayableInput mInput; //インプット

        public PlayableInput Input => mInput;

        private Movement mMovement; //ムーブメント
        public Movement Movement => mMovement;

        // クラス変数に追加: アニメーションのブレンドを滑らかにするための変数
        [SerializeField]
        private float mAnimSpeed = 0f;          // 現在アニメーターに渡しているブレンド値
        [SerializeField]
        private float mAnimSmoothTime = 0.1f;   // ブレンドにかける時間 (0.1秒程度が滑らか)
        [SerializeField]
        private float mSmoothVelocity = 0f;     // SmoothDampで使用する参照速度（内部で自動更新される）

        // クラス変数に追加: アニメーションのブレンドを滑らかにするための変数
        [SerializeField]
        private float mCrouchAnimSpeed = 0f;          // 現在アニメーターに渡しているブレンド値
        [SerializeField]
        private float mCrouchAnimSmoothTime = 0.1f;   // ブレンドにかける時間 (0.1秒程度が滑らか)
        [SerializeField]
        private float mCrouchSmoothVelocity = 0f;     // SmoothDampで使用する参照速度（内部で自動更新される）


        [SerializeField]
        private bool mGrounded; //地面に接地しているかどうか

        public bool Grounded => mGrounded;

        [SerializeField]
        private float mRayLength = 0.1f; //地面判定用のRayの長さ

        [SerializeField]
        private float mRayRadius = 0.5f; //地面判定用のRayの半径

        [SerializeField]
        private LayerMask mGroundLayer; //地面判定用のレイヤー

        [SerializeField]
        private float mGroundCheckOffsetY; // 地面判定用の球の半径



        private void Awake()
        {
            stateMachine = new StateMachine<string>();
            states = new StateBase<string>[]
            {
                mIdleState,
                mMoveState,
                mJumpUpState,
                mJumpState,
                mJumpDownState,
                mStandingToCrouchState,
                mCrouchIdleState,
                mCrouchMoveState,
                mCrouchToStandingState,
                mFallState
            };
            stateMachine.Setup(states);
            foreach (var state in states)
            {
                state.Setup(gameObject);
            }
            stateMachine.ChangeState("Idle");


            mRigidbody = GetComponent<Rigidbody>();
            if (mRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found on " + gameObject.name);
            }

            mAnimator = GetComponentInChildren<Animator>();
            if (mAnimator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }

            mInput = GetComponent<PlayableInput>();
            if (mInput == null)
            {
                Debug.LogError("PlayableInput component not found on " + gameObject.name);
            }

            mMovement = GetComponent<Movement>();
            if (mMovement == null)
            {
                Debug.LogError("Movement component not found on " + gameObject.name);
            }

        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            float t = Time.deltaTime;
            GroundCheck();
            stateMachine.Update(t);
            mCurrentStateKey = stateMachine.CurrentState.Key;
        }

        public bool GroundCheck()
        {
            //当たった情報を取得
            RaycastHit hit;
            bool land = false;
            land = Physics.SphereCast(transform.position + mGroundCheckOffsetY * Vector3.up,
                mRayRadius, Vector3.down, out hit, mRayLength, mGroundLayer,
                QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position + mGroundCheckOffsetY * Vector3.up, Vector3.down * (mRayLength + mRayRadius), Color.red);
            //地面に接地しているかどうかの判定
            if (land)
            {
                Debug.Log("Grounded");
                mGrounded = true;
                return true;
            }
            mGrounded = false;
            Debug.Log("Not Grounded");
            return false;
        }

        public void UpdateAnimation()
        {
            if (mAnimator == null)
            {
                return;
            }
            // 1. 物理速度の絶対値を取得
            float targetSpeed = mRigidbody.linearVelocity.magnitude;

            // 2. 速度を最高速度で正規化し、0～1の値に変換（ブレンドツリーの範囲に合わせる）
            // ※ブレンドツリーの最大値が1の場合を想定
            float targetBlendValue = targetSpeed / mMaxSpeed;

            // 3. SmoothDampを使って、現在のブレンド値(mAnimSpeed)を目標値(targetBlendValue)へ滑らかに変化させる
            mAnimSpeed = Mathf.SmoothDamp(
                mAnimSpeed,             // 現在の値
                targetBlendValue,       // 目標の値
                ref mSmoothVelocity,    // 内部で使用される参照速度（毎回渡す）
                mAnimSmoothTime         // 目標値に到達するまでにかける時間
            );

            if (mAnimator.GetFloat("idleToRun") != mAnimSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                mAnimator.SetFloat("idleToRun", mAnimSpeed);
            }
        }

        public void UpdateCrouchAnimation()
        {
            if (mAnimator == null)
            {
                return;
            }
            float targetSpeed = mRigidbody.linearVelocity.magnitude;
            float targetBlendValue = targetSpeed / mCrouchMaxSpeed;
            mCrouchAnimSpeed = Mathf.SmoothDamp(
                mCrouchAnimSpeed,
                targetBlendValue,
                ref mCrouchSmoothVelocity,
                mCrouchAnimSmoothTime
            );
            if (mAnimator.GetFloat("crouch_IdleToWalk") != mCrouchAnimSpeed)
            {
                mAnimator.SetFloat("crouch_IdleToWalk", mCrouchAnimSpeed);
            }
        }

        private void FixedUpdate()
        {
            float t = Time.fixedDeltaTime;

            stateMachine.FixedUpdate(t);
            Vector3 v = mRigidbody.linearVelocity;
            v.y = 0;
            if (v.magnitude > 0.5f)
            {
                Vector3 velocity = mRigidbody.linearVelocity;
                velocity.y = 0; // 水平方向の速度のみを考慮
                Quaternion targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, mRotationSpeed);
            }
        }

        public void RotateYBody()
        {
            // カメラのY軸回転を取得
            Quaternion cameraRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

            // 入力ベクトル (前後にmInputMove.y、左右にmInputMove.x)
            Vector3 inputVector = new Vector3(mInput.InputMove.x, 0, mInput.InputMove.y);

            // 入力があれば正規化 (斜め移動の速度を一定にするため)
            if (inputVector.sqrMagnitude > 1f)
            {
                inputVector.Normalize();
            }

            // カメラの回転を適用し、目標移動方向を計算
            mMovement.CurrentVelocity = cameraRotation * inputVector;
        }

        private void LateUpdate()
        {
            float t = Time.deltaTime;
            stateMachine.LateUpdate(t);
        }

        private void OnTriggerEnter(Collider other)
        {
            stateMachine.TriggerEnter(gameObject, other);
        }
        private void OnTriggerStay(Collider other)
        {
            stateMachine.TriggerStay(gameObject, other);
        }
        private void OnTriggerExit(Collider other)
        {
            stateMachine.TriggerExit(gameObject, other);
        }

        private void OnDestroy()
        {
            stateMachine.Dispose();
        }
    }
}
