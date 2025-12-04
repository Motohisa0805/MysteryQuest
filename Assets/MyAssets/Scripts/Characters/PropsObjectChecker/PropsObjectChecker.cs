using UnityEngine;

namespace MyAssets
{
    [RequireComponent(typeof(PlayableChracterController))]
    public class PropsObjectChecker : MonoBehaviour
    {
        private PlayableChracterController mController;

        [SerializeField]
        private ObjectSizeType mSmallObject;

        public ObjectSizeType TakedObject => mSmallObject;
        
        private bool mHasTakedObject;


        [SerializeField]
        private ObjectSizeType mLargeObject;
        public ObjectSizeType LargeObject => mLargeObject;

        private Collision mHitCollision;

        private float mLargeObjectMass;

        public float LargeObjectMass => mLargeObjectMass;

        [SerializeField]
        private bool mPushEnabled; //地面に接地しているかどうか

        public bool PushEnabled => mPushEnabled;

        private Vector3 mTargetPos;
        public Vector3 TargetPos => mTargetPos;

        private Quaternion mTargetRot;
        public Quaternion TargetRot => mTargetRot;

        public void SetReleaseTakedObject() 
        {
            mHasTakedObject = false;
            Rigidbody rigidbody = mSmallObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.useGravity = true;
            }
            Collider collider = mSmallObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
        }

        [SerializeField]
        private HandTransform[] mHandTransforms = new HandTransform[2];

        public Vector3 GetObjectsYouHavePoint()
        {
            if (mHandTransforms[0] == null || mHandTransforms[1] == null)
            {
                return transform.position;
            }
            Vector3 vec1 = mHandTransforms[0].gameObject.transform.position;
            Vector3 vec2 = mHandTransforms[1].gameObject.transform.position;
            return new Vector3((vec1.x + vec2.x) / 2, (vec1.y + vec2.y) / 2, (vec1.z + vec2.z) / 2);
        }

        private void Awake()
        {
            mController = GetComponent<PlayableChracterController>();

            mHandTransforms = transform.GetComponentsInChildren<HandTransform>();
        }

        public void CheckTheDistanceHandsAndObject()
        {
            if (mSmallObject == null||mHasTakedObject) return;

            float minDis = mSmallObject.transform.position.y - GetObjectsYouHavePoint().y;
            if (Mathf.Abs(minDis) < 0.01f)
            {
                mHasTakedObject = true;
                Rigidbody rigidbody = mSmallObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.useGravity = false;
                }
                Collider collider = mSmallObject.GetComponent<Collider>();
                if(collider != null)
                {
                    collider.isTrigger = true;
                }
            }
        }

        public void UpdateTakedObjectPosition()
        {
            if(!mHasTakedObject) return;
            mSmallObject.transform.position = Vector3.Lerp(mSmallObject.transform.position, GetObjectsYouHavePoint(), 1.0f);
            mSmallObject.transform.rotation = transform.rotation;
        }

        public void CalculateSnapTransform(Collision collision, Transform boxTransform, float playerRadius, out Vector3 targetPos, out Quaternion targetRot)
        {
            //衝突情報の取得
            ContactPoint contact = collision.GetContact(0);
            Vector3 rawNormal = contact.normal;// 箱からプレイヤーに向かうベクトル
            // ---------------------------------------------------------
            // A. 向きの計算（ボックスの軸にスナップさせる）
            // ---------------------------------------------------------

            //法線を箱のローカル座標系に変換
            Vector3 localNormal = boxTransform.InverseTransformDirection(rawNormal);

            // 最大の成分を見つけて、それ以外を0にする（軸合わせ）
            Vector3 snappedLocalNormal = Vector3.zero;
            if(Mathf.Abs(localNormal.x) > Mathf.Abs(localNormal.z))
            {
                // X軸方向の面（右か左）
                snappedLocalNormal.x = Mathf.Sign(localNormal.x);
            }
            else
            {
                // Z軸方向の面（前か後ろ）
                snappedLocalNormal.z = Mathf.Sign(localNormal.z);
            }
            //上下（Y軸）で押すことがない前提

            // ワールド座標に戻す
            Vector3 snappedWorldNormal = boxTransform.TransformDirection(snappedLocalNormal);

            // プレイヤーが向くべき方向は、法線の逆（箱に向かう方向）
            Vector3 lookDir = snappedWorldNormal;

            //Y軸成分を消して水平にする
            lookDir.y = 0;
            lookDir.Normalize();

            //目標の回転
            targetRot = Quaternion.LookRotation(lookDir);

            // ---------------------------------------------------------
            // B. 位置の計算（表面に吸着させる）
            // ---------------------------------------------------------

            // 衝突点から、法線方向（手前）に「プレイヤー半径」分だけ戻した位置
            // これにより、コライダーの表面同士がぴったり接触する位置になる
            targetPos = contact.point + (snappedWorldNormal * playerRadius);
            targetPos.y = transform.position.y;
        }

        public bool CheckPushReleaseCondition(float releaseThreshold = -0.5f)
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 targetMoveDirection = mController.Movement.CurrentInputVelocity;
            targetMoveDirection.y = 0;

            float dotProduct = Vector3.Dot(forward, targetMoveDirection);

            if(dotProduct < releaseThreshold)
            {
                return true;
            }
            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            ObjectSizeType obj = null;
            obj = other.GetComponent<ObjectSizeType>();
            if (obj != null)
            {
                if (!mHasTakedObject)
                {
                    if (obj.Size == ObjectSizeType.SizeType.Small)
                    {
                        mSmallObject = obj;
                    }
                }
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            ObjectSizeType obj = null;
            obj = collision.collider.GetComponent<ObjectSizeType>();
            if (obj != null)
            {
                if (obj.Size == ObjectSizeType.SizeType.Large)
                {
                    //プレイヤーの向きで押すか押さないか
                    float dotProduct = Vector3.Dot(transform.forward, mController.Movement.CurrentInputVelocity);
                    if (dotProduct > 0.8f)
                    {
                        mLargeObject = obj;
                        mHitCollision = collision;
                        mPushEnabled = true;

                        if (mLargeObject.GetComponent<Rigidbody>() != null)
                        {
                            mLargeObjectMass = mLargeObject.GetComponent<Rigidbody>().mass;
                        }
                    }
                    else
                    {
                        mPushEnabled = false;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ObjectSizeType obj = null;
            obj = other.GetComponent<ObjectSizeType>();
            if (obj != null)
            {
                if (!mHasTakedObject)
                {
                    if (obj.Size == ObjectSizeType.SizeType.Small)
                    {
                        mSmallObject = null;
                    }
                }
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            ObjectSizeType obj = null;
            obj = collision.collider.GetComponent<ObjectSizeType>();
            if (obj != null)
            {
                if (obj.Size == ObjectSizeType.SizeType.Large)
                {
                    mLargeObject = null;
                    mHitCollision = null;
                    mPushEnabled = false;
                }
            }
        }
    }
}
