using UnityEngine;

namespace MyAssets
{
    [RequireComponent(typeof(PlayableAnimationFunction))]
    [RequireComponent(typeof(PlayableInput))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(PropsObjectChecker))]
    public class PlayableChracterController : MonoBehaviour
    {

        [Header("キャラクター内部の処理")]
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
        private SpritDushState mSpritDushState;
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
        private ToLiftState mToLiftState;
        [SerializeField]
        private ToLiftIdleState mToLiftIdleState;
        [SerializeField]
        private ToLiftRunState mToLiftRunState;
        [SerializeField]
        private ReleaseLiftState mReleaseLiftState;
        [SerializeField]
        private PushStartState mPushStartState;
        [SerializeField]
        private PushingState mPushingState;
        [SerializeField]
        private PushEndState mPushEndState;
        [SerializeField]
        private ClimbJumpingState mClimbJumpingState;

        [SerializeField]
        private float mMaxSpeed; //最高速度
        public float MaxSpeed => mMaxSpeed;

        [SerializeField]
        private float mDushMaxSpeed; //最高速度
        public float DushMaxSpeed => mDushMaxSpeed;

        [SerializeField]
        private float mCrouchMaxSpeed; //しゃがみ時の最高速度
        public float CrouchMaxSpeed => mCrouchMaxSpeed;

        [SerializeField]
        private float mRotationSpeed; //回転速度


        private Rigidbody mRigidbody; //リジッドボディ
        public Rigidbody Rigidbody => mRigidbody;

        private PlayableInput mInput; //インプット

        public PlayableInput Input => mInput;

        private Movement mMovement; //ムーブメント
        public Movement Movement => mMovement;



        [SerializeField]
        private bool mGrounded; //地面に接地しているかどうか
        public bool Grounded => mGrounded;

        private bool mIsPastGrounded; //前フレームで地面に接地していたかどうか
        [SerializeField]
        private Timer mFallTimer;
        public Timer FallTimer => mFallTimer;
        [SerializeField]
        private float mCount;

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
                mSpritDushState,
                mJumpUpState,
                mJumpState,
                mJumpDownState,
                mStandingToCrouchState,
                mCrouchIdleState,
                mCrouchMoveState,
                mCrouchToStandingState,
                mFallState,
                mToLiftState,
                mToLiftIdleState,
                mToLiftRunState,
                mReleaseLiftState,
                mPushStartState,
                mPushingState,
                mPushEndState,
                mClimbJumpingState,
            };
            stateMachine.Setup(states);
            foreach (var state in states)
            {
                state.Setup(gameObject);
            }
            stateMachine.ChangeState(mCurrentStateKey);


            mRigidbody = GetComponent<Rigidbody>();
            if (mRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found on " + gameObject.name);
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

            mFallTimer.Update(t);

            GroundCheck();
            stateMachine.Update(t);
            mCurrentStateKey = stateMachine.CurrentState.Key;
        }

        public bool GroundCheck()
        {
            mIsPastGrounded = mGrounded;
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
                mGrounded = true;
            }
            else
            {
                mGrounded = false;
                if(mIsPastGrounded != mGrounded)
                {
                    //地面から離れたときの処理
                    mFallTimer.Start(mCount);
                }
            }

            return true;
        }

        private void FixedUpdate()
        {
            float t = Time.fixedDeltaTime;

            stateMachine.FixedUpdate(t);
        }

        public void RotateBody()
        {
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

        public void InputVelocity()
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
            mMovement.CurrentInputVelocity = cameraRotation * inputVector;
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
