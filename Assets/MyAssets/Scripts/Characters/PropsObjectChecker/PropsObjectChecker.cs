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

        Vector3 mThrowDirction;


        [SerializeField]
        private ObjectSizeType mLargeObject;
        public ObjectSizeType LargeObject => mLargeObject;

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
        //================================
        //オブジェクト取得関連の関数
        //================================
        //オブジェクトと手の距離をチェック
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
        //オブジェクトの位置を手の位置に更新
        public void UpdateTakedObjectPosition()
        {
            if(!mHasTakedObject) return;
            mSmallObject.transform.position = Vector3.Lerp(mSmallObject.transform.position, GetObjectsYouHavePoint(), 1.0f);
            mSmallObject.transform.rotation = transform.rotation;
        }
        //小オブジェクトを投げる方向を更新
        public void UpdateTakedObjectThrowDirection()
        {
            if (!mHasTakedObject) return;
            mThrowDirction = transform.forward + Vector3.up * 0.2f;
            /*
            //UIを落下予測地点に移動
            Rigidbody rigidbody = mSmallObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                float force = 20.0f;
                float impulse = Vector3.Dot(mThrowDirction, (Vector3.one * force));
                float initVelocity = impulse / rigidbody.mass;

                float h = mSmallObject.transform.position.y;
                float velocityY = initVelocity * h;

                
            }
             */
        }
        //オブジェクトを投げる処理
        public void UpdateTakedObjectThrowDirection(float throwForce)
        {
            //オブジェクトがあるかどうか
            if (!mHasTakedObject) return;
            Rigidbody rigidbody = mSmallObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(mThrowDirction * throwForce, ForceMode.VelocityChange);
            }
            mHasTakedObject = false;
            /*
            Collider collider = mSmallObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
            rigidbody.useGravity = true;
            mSmallObject = null;
             */
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
                        Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
                        if (rigidbody != null)
                        {
                            rigidbody.useGravity = true;
                        }
                        Collider collider = obj.GetComponent<Collider>();
                        if (collider != null)
                        {
                            collider.isTrigger = false;
                        }
                        mSmallObject = null;
                    }
                }
            }
        }

        //================================
        //Largeオブジェクトの接触判定3関数
        //================================
        public void CalculateSnapTransform(Collision collision, Transform transform, float playerRadius, out Quaternion targetRot)
        {
            //衝突情報の取得
            ContactPoint contact = collision.GetContact(0);
            Vector3 rawNormal = contact.normal;// オブジェクトからプレイヤーに向かうベクトル
            // ---------------------------------------------------------
            // A. 向きの計算（オブジェクトの軸にスナップさせる）
            // ---------------------------------------------------------

            //法線をオブジェクトのローカル座標系に変換
            Vector3 localNormal = transform.InverseTransformDirection(rawNormal);

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
            Vector3 snappedWorldNormal = transform.TransformDirection(snappedLocalNormal);

            // プレイヤーが向くべき方向は、法線の逆（箱に向かう方向）
            Vector3 lookDir = -snappedWorldNormal;

            //Y軸成分を消して水平にする
            lookDir.y = 0;
            lookDir.Normalize();

            //目標の回転
            targetRot = Quaternion.LookRotation(lookDir);
            base.transform.rotation = targetRot;
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
        private void LargeObjectHitEnter(Collision collision)
        {
            if(IsVerticalCollision(collision.GetContact(0)))
            {
                return;
            }
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
                        CalculateSnapTransform(collision, mLargeObject.transform, GetComponentInChildren<CapsuleCollider>().radius, out mTargetRot);
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
        private void LargeObjectHitStay(Collision collision)
        {
            if(mPushEnabled)
            {
                ObjectSizeType obj = null;
                obj = collision.collider.GetComponent<ObjectSizeType>();
                if (obj != null)
                {
                    if (obj == mLargeObject)
                    {
                        CalculateSnapTransform(collision, mLargeObject.transform, GetComponentInChildren<CapsuleCollider>().radius, out mTargetRot);
                    }
                }
            }
        }
        private void LargeObjectHitExit(Collision collision)
        {
            if (mPushEnabled)
            {
                ObjectSizeType obj = null;
                obj = collision.collider.GetComponent<ObjectSizeType>();
                if (obj != null)
                {
                    if (obj == mLargeObject)
                    {
                        mLargeObject = null;
                        mPushEnabled = false;
                    }
                }                
            }
        }

        private bool IsVerticalCollision(ContactPoint contact)
        {
            Vector3 normal = contact.normal;
            // 法線ベクトルのY成分が大きい場合、垂直衝突とみなす
            return Mathf.Abs(normal.y) > 0.4f;
        }

        // Largeオブジェクトに接触したとき
        private void OnCollisionEnter(Collision collision)
        {
            LargeObjectHitEnter(collision);
        }
        private void OnCollisionStay(Collision collision)
        {
            LargeObjectHitStay(collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            LargeObjectHitExit(collision);
        }


    }
}
