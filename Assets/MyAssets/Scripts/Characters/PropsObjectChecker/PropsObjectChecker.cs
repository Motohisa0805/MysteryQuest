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
        [SerializeField]
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
        public void UpdateTakedObjectThrowDirection(float throwForce)
        {
            if (!mHasTakedObject) return;
            mThrowDirction = Camera.main.transform.forward;
            //UIを落下予測地点に移動
            Rigidbody rigidbody = mSmallObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                // 投げる力の大きさを定義 (AddForceに与える値)
                float forceMagnitude = throwForce;
                float g = -Physics.gravity.y;

                //初速度ベクトルの決定
                Vector3 initialVelocityVector = mThrowDirction * forceMagnitude;

                //必要な成分の抽出
                float v0y = initialVelocityVector.y;//垂直成分
                float v0x = new Vector3(initialVelocityVector.x, 0, initialVelocityVector.z).magnitude;//水平成分
                float h = mSmallObject.transform.position.y;//投げる高さ

                //地面到達時間 t_hit の計算
                float discriminant = (v0y * v0y) + (2 * g * h);
                float t_hit = (v0y + Mathf.Sqrt(discriminant)) / g;

                //水平移動距離 x_dist の計算
                float x_dist = v0x * t_hit;

                //落下予測地点のワールド座標 P_hit の計算
                Vector3 startPos = mSmallObject.transform.position;
                //水平方向ベクトル (y成分を0にして正規化)
                Vector3 directionXZ = new Vector3(mThrowDirction.x, 0, mThrowDirction.z).normalized;
                Vector3 hitPosition = startPos + (directionXZ * x_dist);

                // レイキャストによる衝突予測
                //TODO : レイキャストの最適化（計算量削減）
                //放物線に沿った複数の点を計算し、その間をレイキャストで結ぶ方法を使用
                //パフォーマンス低下の可能性があるため、必要に応じて調整

                // レイキャストのパラメータ設定
                float timeStep = 0.05f; // 微小な時間刻み（例: 0.05秒）
                Vector3 currentPos = startPos;
                Vector3 hitPoint = Vector3.zero;
                Vector3 hitNormal = Vector3.up;
                bool didHit = false;

                // T_HITまで計算すると計算量が多すぎるため、上限時間を設ける（例: 5秒）
                float maxTime = 5.0f;
                float t = 0.0f;

                while (t < maxTime)
                {
                    // 次の時刻 t + dt を計算
                    t += timeStep;

                    // 時刻 t における放物線上の次の位置を計算 (運動方程式)
                    // P = P_start + v0*t + 0.5*g*t^2
                    Vector3 nextPos = startPos
                                    + initialVelocityVector * t
                                    + Physics.gravity * (0.5f * t * t);

                    // Raycastの方向と距離を計算
                    Vector3 direction = nextPos - currentPos;
                    float distance = direction.magnitude;

                    RaycastHit hit;
                    // Raycastの実行
                    if (Physics.Raycast(currentPos, direction.normalized, out hit, distance))
                    {
                        // 衝突した場合、その地点を予測地点とする
                        hitPoint = hit.point;
                        hitNormal = hit.normal;
                        didHit = true;
                        break; // ループを終了
                    }

                    // 衝突しなかった場合、次のセグメントへ進む準備
                    currentPos = nextPos;

                    // Y座標がスタート位置より低く、Y=0を下回った場合は強制終了
                    // ただし、レイキャストが当たらない限り、この判定は不要な場合が多い
                    if (currentPos.y < startPos.y && currentPos.y < -1f)
                    {
                        break;
                    }
                }

                // Raycastが地形に当たらなかった場合、UIを非表示にする、または遠くに配置する
                if (!didHit)
                {
                    // 遠すぎる、または空中に飛んでいった場合、UIを非表示にするなど
                    PlayerUIManager.Instance.ThrowCircle.gameObject.SetActive(false);
                    return;
                }

                // UIの座標と回転を設定
                PlayerUIManager.Instance.ThrowCircle.gameObject.SetActive(true);
                PlayerUIManager.Instance.ThrowCircle.position = hitPoint;
                PlayerUIManager.Instance.ThrowCircle.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
            }
        }
        
        //オブジェクトを投げる処理
        public void Throw(float throwForce)
        {
            //オブジェクトがあるかどうか
            if (!mHasTakedObject) return;
            Rigidbody rigidbody = mSmallObject.GetComponent<Rigidbody>();
            rigidbody.linearVelocity = Vector3.zero;
            if (rigidbody != null)
            {
                rigidbody.AddForce(mThrowDirction * throwForce, ForceMode.VelocityChange);
            }
            mHasTakedObject = false;
        }

        private void Update()
        {
            SmallObjectSphereCastChecker();
        }

        //Smallオブジェクトの接触判定関数
        public void SmallObjectSphereCastChecker()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit[] hits;
            float sphereRadius = 0.5f;
            hits = Physics.SphereCastAll(ray, sphereRadius, 1.0f);

            if(hits.Length != 0)
            {
                ObjectSizeType obj = null;
                for (int i = 0; i < hits.Length; i++)
                {
                    obj = hits[i].collider.GetComponent<ObjectSizeType>();
                    if (obj != null)
                    {
                        if (!mHasTakedObject)
                        {
                            if (obj.Size == ObjectSizeType.SizeType.Small)
                            {
                                mSmallObject = obj;
                                break;
                            }
                        }
                    }
                }
                if(!obj&& mSmallObject && !mHasTakedObject)
                {
                    ClearSmallObject();
                }
            }
            else
            {
                if (!mHasTakedObject)
                {
                    if(mSmallObject)
                    {
                        ClearSmallObject();
                    }
                }
            }
        }

        private void ClearSmallObject()
        {
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
            mSmallObject = null;
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

            float absX = Mathf.Abs(localNormal.x);
            float absY = Mathf.Abs(localNormal.y);
            float absZ = Mathf.Abs(localNormal.z);
            if (absX > absY && absX > absZ)
            {
                // X軸方向の面が一番近い
                snappedLocalNormal.x = Mathf.Sign(localNormal.x);
            }
            else if (absY > absX && absY > absZ)
            {
                // Y軸方向の面が一番近い（回転している場合、これが横に来ることがある）
                snappedLocalNormal.y = Mathf.Sign(localNormal.y);
            }
            else
            {
                // Z軸方向の面が一番近い
                snappedLocalNormal.z = Mathf.Sign(localNormal.z);
            }
            // ワールド座標に戻す
            Vector3 snappedWorldNormal = transform.TransformDirection(snappedLocalNormal);

            // プレイヤーが向くべき方向は、法線の逆（箱に向かう方向）
            Vector3 lookDir = -snappedWorldNormal;

            //Y軸成分を消して水平にする
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                lookDir.Normalize();
                targetRot = Quaternion.LookRotation(lookDir);
            }
            else
            {
                // 例外処理：向きが確定できない場合は今の向きを維持、あるいは法線をそのまま使うなど
                targetRot = base.transform.rotation;
            }
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
