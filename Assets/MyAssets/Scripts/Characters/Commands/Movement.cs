using System.Collections;
using TMPro;
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

        [SerializeField]
        private MovementCompensator mMovementCompensator;

        private bool isClimbing = false;

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
            mMovementCompensator.HandleStepClimbin();
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

            //段差補正
            StartClimbStep(mMovementCompensator.StepGoalPosition);
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
        public void StartClimbStep(Vector3 hitPoint)
        {
            // 既に登っている最中、またはターゲットが無効なら何もしない
            if (isClimbing || hitPoint == Vector3.zero) { return; }

            // コルーチン（時間経過処理）を開始
            StartCoroutine(ProcessClimb(hitPoint));
        }

        private IEnumerator ProcessClimb(Vector3 targetPosition)
        {
            isClimbing = true;

            // 1. 物理演算の影響を一時的に切る
            // これをしないと、登っている最中に重力で落とされたり、壁の摩擦で引っかかったりします
            bool originalKinematic = mRigidbody.isKinematic;
            mRigidbody.isKinematic = true;

            // 移動開始前の座標と時間
            Vector3 startPos = transform.position;
            float elapsedTime = 0f;

            Vector3 finalPos = targetPosition + Vector3.up * 0.01f;

            // 2. 指定時間かけて滑らかに移動（Lerp）
            while (elapsedTime < mMovementCompensator.ClimbDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / mMovementCompensator.ClimbDuration;

                // EaseOut（最初は早く、最後はゆっくり）をかけると自然に見えます
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                // 座標を更新
                mRigidbody.MovePosition(Vector3.Lerp(startPos, finalPos, t));

                yield return null; // 1フレーム待機
            }

            // 念のため最終位置にきっちり合わせる
            mRigidbody.MovePosition(finalPos);

            // 3. 物理演算を元に戻す
            mRigidbody.isKinematic = originalKinematic;
            // mCollider.isTrigger = false;

            // 速度をリセット（登った勢いで吹っ飛ばないように）
            mRigidbody.linearVelocity = Vector3.zero;

            isClimbing = false;
        }
    }
}

