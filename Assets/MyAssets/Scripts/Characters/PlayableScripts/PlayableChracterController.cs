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
        [Header("状態一覧")]
        [SerializeField]
        private IdleState mIdleState;
        [SerializeField]
        private MoveState mMoveState;
        [SerializeField]
        private SpritDushState mSpritDushState;
        [SerializeField]
        private FocusingMoveState mFocusingMoveState;
        [SerializeField]
        private JumpUpState mJumpUpState;
        [SerializeField]
        private JumpingState mJumpState;
        [SerializeField]
        private JumpDownState mJumpDownState;
        [Header("しゃがみ'4'状態")]
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
        [Header("投げ'3'状態")]
        [SerializeField]
        private ThrowStartState mThrowStartState;
        [SerializeField]
        private ThrowIdleState mThrowIdleState;
        [SerializeField]
        private ThrowingState mThrowingState;
        [Header("押し'3'状態")]
        [SerializeField]
        private PushStartState mPushStartState;
        [SerializeField]
        private PushingState mPushingState;
        [SerializeField]
        private PushEndState mPushEndState;
        [Header("登り'3'状態")]
        [SerializeField]
        private ClimbJumpingState mClimbJumpingState;
        [SerializeField]
        private ClimbJumpState mClimbJumpState;
        [SerializeField]
        private ClimbState mClimbState;

        [SerializeField]
        private SmallImpactPlayerState mSmallImpactPlayerState;
        [SerializeField]
        private BigImpactPlayerState mBigImpactPlayerState;
        [SerializeField]
        private StandingUpState mStandingUpState;

        [SerializeField]
        private WeaponTakingOutState mWeaponTakingOutState;
        [SerializeField]
        private WeaponStorageState mWeaponStorageState;
        [SerializeField]
        private ReadyFirstAttackState mReadyFirstAttackState;
        [SerializeField]
        private FirstAttackState mFirstAttackState;
        [SerializeField]
        private SecondAttackState mSecondAttackState;

        [Header("キャラクターのステータス")]
        [SerializeField]
        private PlayerStatusPropaty mStatusProperty;
        public PlayerStatusPropaty StatusProperty => mStatusProperty;

        private Rigidbody mRigidbody; //リジッドボディ
        public Rigidbody Rigidbody => mRigidbody;
        private PlayableInput mInput; //インプット
        public PlayableInput Input => mInput;
        private Movement mMovement; //ムーブメント
        public Movement Movement => mMovement;

        private ImpactChecker mImpactChecker;
        public ImpactChecker ImpactChecker => mImpactChecker;

        private TargetSearch mTargetSearch;
        public TargetSearch TargetSearch => mTargetSearch;


        [SerializeField]
        private bool mGrounded; //地面に接地しているかどうか
        public bool Grounded => mGrounded;

        private bool mIsPastGrounded; //前フレームで地面に接地していたかどうか

        [SerializeField]
        private bool mOverHead; //地面に接地しているかどうか
        public bool OverHead => mOverHead;

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


        [Header("キャラクターの手の位置")]
        [SerializeField]
        private SetItemTransform[] mHandTransforms = new SetItemTransform[2];
        public SetItemTransform[] HandTransforms 
        {
            get 
            { 
                if (mHandTransforms == null)
                {
                    mHandTransforms = transform.GetComponentsInChildren<SetItemTransform>();
                }
                return mHandTransforms; 
            } 
        }

        private void Awake()
        {
            mHandTransforms = transform.GetComponentsInChildren<SetItemTransform>();

            stateMachine = new StateMachine<string>();
            states = new StateBase<string>[]
            {
                mIdleState,
                mMoveState,
                mSpritDushState,
                mFocusingMoveState,
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
                mThrowStartState,
                mThrowIdleState,
                mThrowingState,
                mPushStartState,
                mPushingState,
                mPushEndState,
                mClimbJumpingState,
                mClimbJumpState,
                mClimbState,
                mSmallImpactPlayerState,
                mBigImpactPlayerState,
                mStandingUpState,
                mWeaponTakingOutState,
                mWeaponStorageState,
                mReadyFirstAttackState,
                mFirstAttackState,
                mSecondAttackState
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

            mTargetSearch = GetComponent<TargetSearch>();
            if(mTargetSearch == null)
            {
                Debug.LogError("TargetSearch component not found on " + gameObject.name);
            }

        }
        private void Start()
        {
            //TODO 仮でマウスをロック
            InputManager.SetLockedMouseMode();
        }

        private void Update()
        {
            float t = Time.deltaTime;

            mFallTimer.Update(t);

            GroundCheck();
            OverheadCheck();
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

        public void OverheadCheck()
        {
            //当たった情報を取得
            RaycastHit hit;
            bool overhead = false;
            overhead = Physics.SphereCast(transform.position + Vector3.up * (mGroundCheckOffsetY + 0.5f),
                mRayRadius, Vector3.up, out hit, mRayLength, mGroundLayer,
                QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position + Vector3.up * (mGroundCheckOffsetY + 0.5f), Vector3.up * (mRayLength + mRayRadius), Color.blue);
            if(overhead)
            {
                mOverHead = true;
            }
            else
            {
                mOverHead = false;
            }
        }

        private void FixedUpdate()
        {
            float t = Time.fixedDeltaTime;

            stateMachine.FixedUpdate(t);
        }

        public void FreeRotate()
        {
            Vector3 v = mRigidbody.linearVelocity;
            v.y = 0;
            if (v.magnitude > 0.5f)
            {
                Vector3 velocity = mRigidbody.linearVelocity;
                velocity.y = 0; // 水平方向の速度のみを考慮
                Quaternion targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, mStatusProperty.RotationSpeed);
            }
        }

        public void FocusingRotate()
        {
            if(mTargetSearch.TargetObject == null)
            {
                return;
            }
            Vector3 velocity = mTargetSearch.TargetObject.transform.position - transform.position;
            velocity.y = 0; // 水平方向の速度のみを考慮
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, mStatusProperty.RotationSpeed);
        }

        public void ShoulderViewRotate()
        {
            // カメラのY軸回転を格納するQuaternionを作成する
            // カメラのオイラー角からY軸回転だけを取り出す
            float cameraYAngle = Camera.main.transform.eulerAngles.y;

            // キャラクターの現在のX軸とZ軸の回転は維持しつつ、Y軸回転だけをカメラに合わせる
            // Quaternion.Eulerで角度から回転（Quaternion）を作成する
            Quaternion targetRotation = Quaternion.Euler(
                transform.eulerAngles.x,    // X軸回転（ピッチ）は維持
                cameraYAngle,               // Y軸回転（ヨー）をカメラに合わせる
                transform.eulerAngles.z     // Z軸回転（ロール）は維持
            );

            // キャラクターの回転を目標の回転へスムーズに補間する
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                StatusProperty.ShoulderViewRotationSpeed * Time.deltaTime
            );
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

        private void OnCollisionEnter(Collision collision)
        {
            stateMachine.CollisionEnter(gameObject, collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            stateMachine.CollisionStay(gameObject, collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            stateMachine.CollisionExit(gameObject, collision);
        }

        private void OnDestroy()
        {
            stateMachine.Dispose();
        }
    }
}
