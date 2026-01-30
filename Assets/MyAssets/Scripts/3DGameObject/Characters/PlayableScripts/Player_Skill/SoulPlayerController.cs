using UnityEngine;

namespace MyAssets
{
    //幽体離脱モード時の管理クラス
    public class SoulPlayerController : MonoBehaviour
    {
        private PlayableInput       mPlayableInput;
        public PlayableInput        PlayableInput => mPlayableInput;

        private FloatingMovement    mFloatingMovement;
        public FloatingMovement     FloatingMovement => mFloatingMovement;

        private CapsuleCollider     mCapsuleCollider;
        public CapsuleCollider      CapsuleCollider => mCapsuleCollider;

        private Rigidbody           mRigidbody;
        public Rigidbody            Rigidbody => mRigidbody;

        private ParticleSystem      mParticleSystem;

        private Vector3             mBasePosition = new Vector3(0, 1.3f, 0);

        private Transform           mParent;

        [SerializeField]
        private TakeObjectment      mTakeObjectment;
        public TakeObjectment       TakeObjectment => mTakeObjectment;
        private void Awake()
        {
            mPlayableInput = GetComponentInParent<PlayableInput>();
            if(mPlayableInput == null )
            {
                Debug.LogError("Not Found PlayableInput");
            }
            mFloatingMovement = GetComponent<FloatingMovement>();
            if(mFloatingMovement == null )
            {
                Debug.LogError("Not Found FloatingMovement");
            }
            mCapsuleCollider = GetComponent<CapsuleCollider>();
            if(mCapsuleCollider == null )
            {
                Debug.LogError("Not Found CapsuleCollider");
            }
            mRigidbody = GetComponent<Rigidbody>();
            if(mRigidbody == null )
            {
                Debug.LogError("Not Found Rigidbody");
            }
            mParticleSystem = GetComponentInChildren<ParticleSystem>();
            if(mParticleSystem == null )
            {
                Debug.LogError("Not Found ParticleSystem");
            }
        }

        private void Start()
        {
            mParent = transform.parent;
            DisableSoul();

            mTakeObjectment.Setup(transform);

            mFloatingMovement.SetAnchor(mParent, 5.0f);
        }

        public void DisableSoul()
        {
            transform.SetParent(mParent);

            mCapsuleCollider.isTrigger = true;
            mCapsuleCollider.enabled = false;
            mRigidbody.isKinematic = true;

            transform.localPosition = mBasePosition;
            mParticleSystem.Stop();
        }

        public void EnableSoul()
        {
            transform.parent = null;

            mCapsuleCollider.isTrigger = false;
            mCapsuleCollider.enabled = true;
            mRigidbody.isKinematic = false;
            mParticleSystem.Play();
        }

        public void SyncRotationWithCamera()
        {
            // カメラの現在の回転を取得
            Quaternion targetRotation = Camera.main.transform.rotation;

            // もし「常に垂直（直立）」を保ちたい場合はこちら：
            // Vector3 lookDir = Camera.main.transform.forward;
            // lookDir.y = 0; // 垂直成分をカット
            // Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            // 15.0f のスピードで滑らかにカメラの向きへ追従させる
            // 瞬時に向けたい場合は直接 transform.rotation = targetRotation; でOK
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 15.0f);
        }

        public void InputVelocity(Vector3 eventInput = new Vector3())
        {
            Vector3 moveDirection = Vector3.zero;

            // 入力値の取得
            float horizontal = (eventInput != Vector3.zero) ? eventInput.x : mPlayableInput.InputMove.x;
            float vertical = (eventInput != Vector3.zero) ? eventInput.z : mPlayableInput.InputMove.y;

            if (horizontal != 0 || vertical != 0)
            {
                // カメラの「向き」を基準にする
                Vector3 camForward = Camera.main.transform.forward;
                Vector3 camRight = Camera.main.transform.right;

                // カメラの前方向と右方向に入力を掛け合わせる（これで上下も含まれる）
                moveDirection = (camForward * vertical) + (camRight * horizontal);

                // 斜め移動で速くならないように正規化
                if (moveDirection.magnitude > 1f)
                {
                    moveDirection.Normalize();
                }
            }

            mFloatingMovement.CurrentInputVelocity = moveDirection;
        }
    }
}
