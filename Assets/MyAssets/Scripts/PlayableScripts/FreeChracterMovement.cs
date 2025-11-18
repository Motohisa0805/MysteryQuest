using UnityEngine;

namespace MyAssets
{
    //カメラのローカル座標を基準にしたキャラクターの自由移動用スクリプト
    public class FreeChracterMovement : MonoBehaviour
    {
        [SerializeField]
        private float mAcceleration; //加速度
        [SerializeField]
        private float mMaxSpeed; //最高速度

        [SerializeField]
        private float mRotationSpeed; //回転速度

        private Rigidbody mRigidbody; //リジッドボディ

        private Animator mAnimator; //アニメーター

        private Vector2 mInputMove = Vector3.zero; //入力方向

        private Vector3 mCurrentVelocity;

        // クラス変数に追加: アニメーションのブレンドを滑らかにするための変数
        [SerializeField]
        private float mAnimSpeed = 0f;          // 現在アニメーターに渡しているブレンド値
        [SerializeField]
        private float mAnimSmoothTime = 0.1f;   // ブレンドにかける時間 (0.1秒程度が滑らか)
        [SerializeField]
        private float mSmoothVelocity = 0f;     // SmoothDampで使用する参照速度（内部で自動更新される）

        private void Awake()
        {
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
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            // 入力処理
            XAndZInput();

            // アニメーション (velocityに修正が必要です)
            UpdateAnimation();
        }

        private void XAndZInput()
        {
           mInputMove = InputManager.GetKeyValue(KeyCode.eMove);
        }


        private void UpdateAnimation()
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

            if(mAnimator.GetFloat("idleToRun") != mAnimSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                mAnimator.SetFloat("idleToRun", mAnimSpeed);
            }
        }

        private void FixedUpdate()
        {
            RotateYBody();
            XAndZMove();
            if (mRigidbody.linearVelocity.magnitude > 0.5f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(mRigidbody.linearVelocity, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, mRotationSpeed);
            }
        }

        private void XAndZMove()
        {
            // 現在の速度を代入 (velocityに修正)
            Vector3 currentVelocity = mRigidbody.linearVelocity;

            // 加速の目標方向と大きさ
            // mCurrentVelocityは既に正規化済みのベクトル（長さ0～1）
            Vector3 targetVelocity = mCurrentVelocity * mMaxSpeed;

            // 現在のXZ速度と目標XZ速度の差分を計算
            // この差分が「必要な加速」となる
            Vector3 velocityChange = targetVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);

            // 加速の大きさを制限 (一気に加速するのを防ぐ)
            Vector3 accelerationForce = velocityChange.normalized * mAcceleration * Time.deltaTime;

            // ただし、目標速度を超えないようにする
            if (velocityChange.magnitude < accelerationForce.magnitude)
            {
                accelerationForce = velocityChange;
            }

            // 加速を現在の速度に適用
            currentVelocity.x += accelerationForce.x;
            currentVelocity.z += accelerationForce.z;
            Debug.Log(currentVelocity);

            // Y軸速度はそのまま維持
            // XZ成分だけを更新したcurrentVelocityをリジッドボディに代入 (velocityに修正)
            mRigidbody.linearVelocity = currentVelocity;
        }

        private void RotateYBody()
        {
            // カメラのY軸回転を取得
            Quaternion cameraRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

            // 入力ベクトル (前後にmInputMove.y、左右にmInputMove.x)
            Vector3 inputVector = new Vector3(mInputMove.x, 0, mInputMove.y);

            // 入力があれば正規化 (斜め移動の速度を一定にするため)
            if (inputVector.sqrMagnitude > 1f)
            {
                inputVector.Normalize();
            }

            // カメラの回転を適用し、目標移動方向を計算
            mCurrentVelocity = cameraRotation * inputVector;
        }
    }
}
