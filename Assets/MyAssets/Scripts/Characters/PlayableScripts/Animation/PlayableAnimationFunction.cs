using UnityEngine;

namespace MyAssets
{
    public class PlayableAnimationFunction : MonoBehaviour
    {
        private Animator mAnimator; //アニメーター

        public void SetAnimatorEnabled(bool enabled)
        {
            if(mAnimator == null) { return; }
            mAnimator.enabled = enabled;
        }

        private PlayableChracterController mController;

        //アニメーションのブレンドを滑らかにするための変数
        [SerializeField]
        private float mAnimIdleToRunSpeed = 0f;          // 現在アニメーターに渡しているブレンド値
        [SerializeField]
        private float mAnimSmoothTime = 0.1f;   // ブレンドにかける時間 (0.1秒程度が滑らか)
        [SerializeField]
        private float mSmoothVelocity = 0f;     // SmoothDampで使用する参照速度（内部で自動更新される）

        //アニメーションのブレンドを滑らかにするための変数
        [SerializeField]
        private float mSpritDushSpeed = 0f;          // 現在アニメーターに渡しているブレンド値
        [SerializeField]
        private float mSpritDushSmoothTime = 0.1f;   // ブレンドにかける時間 (0.1秒程度が滑らか)
        [SerializeField]
        private float mSpritDushSmoothVelocity = 0f;     // SmoothDampで使用する参照速度（内部で自動更新される）

        //アニメーションのブレンドを滑らかにするための変数
        [SerializeField]
        private float mToLiftIdleToRunSpeed = 0f;          // 現在アニメーターに渡しているブレンド値
        [SerializeField]
        private float mToLiftSmoothTime = 0.1f;   // ブレンドにかける時間 (0.1秒程度が滑らか)
        [SerializeField]
        private float mToLiftSmoothVelocity = 0f;     // SmoothDampで使用する参照速度（内部で自動更新される）

        //アニメーションのブレンドを滑らかにするための変数
        [SerializeField]
        private float mCrouchAnimSpeed = 0f;          // 現在アニメーターに渡しているブレンド値
        [SerializeField]
        private float mCrouchAnimSmoothTime = 0.1f;   // ブレンドにかける時間 (0.1秒程度が滑らか)
        [SerializeField]
        private float mCrouchSmoothVelocity = 0f;     // SmoothDampで使用する参照速度（内部で自動更新される）

        public void UpdateIdleToRunAnimation()
        {
            if (mAnimator == null)
            {
                return;
            }
            // 1. 物理速度の絶対値を取得
            float targetSpeed = mController.Rigidbody.linearVelocity.magnitude;

            // 2. 速度を最高速度で正規化し、0～1の値に変換（ブレンドツリーの範囲に合わせる）
            // ※ブレンドツリーの最大値が1の場合を想定
            float targetBlendValue = targetSpeed / mController.StatusProperty.MaxSpeed;

            // 3. SmoothDampを使って、現在のブレンド値(mAnimSpeed)を目標値(targetBlendValue)へ滑らかに変化させる
            mAnimIdleToRunSpeed = Mathf.SmoothDamp(
                mAnimIdleToRunSpeed,             // 現在の値
                targetBlendValue,       // 目標の値
                ref mSmoothVelocity,    // 内部で使用される参照速度（毎回渡す）
                mAnimSmoothTime         // 目標値に到達するまでにかける時間
            );

            if (mAnimator.GetFloat("idleToRun") != mAnimIdleToRunSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                mAnimator.SetFloat("idleToRun", mAnimIdleToRunSpeed);
            }
        }

        public void UpdateSpritDushAnimation()
        {
            if (mAnimator == null)
            {
                return;
            }

            // 1. 物理速度の絶対値を取得
            float targetSpeed = mController.Rigidbody.linearVelocity.magnitude;

            // 2. 速度を最高速度で正規化し、0～1の値に変換（ブレンドツリーの範囲に合わせる）
            // ※ブレンドツリーの最大値が1の場合を想定
            float targetBlendValue = targetSpeed / mController.StatusProperty.DushMaxSpeed;

            // 3. SmoothDampを使って、現在のブレンド値(mAnimSpeed)を目標値(targetBlendValue)へ滑らかに変化させる
            mSpritDushSpeed = Mathf.SmoothDamp(
                mSpritDushSpeed,             // 現在の値
                targetBlendValue,       // 目標の値
                ref mSpritDushSmoothVelocity,    // 内部で使用される参照速度（毎回渡す）
                mSpritDushSmoothTime         // 目標値に到達するまでにかける時間
            );

            if (mAnimator.GetFloat("spritDush") != mSpritDushSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                mAnimator.SetFloat("spritDush", mSpritDushSpeed);
            }
        }

        public void UpdateToLiftIdleToToLiftRunAnimation()
        {
            if (mAnimator == null)
            {
                return;
            }
            // 1. 物理速度の絶対値を取得
            float targetSpeed = mController.Rigidbody.linearVelocity.magnitude;

            // 2. 速度を最高速度で正規化し、0～1の値に変換（ブレンドツリーの範囲に合わせる）
            // ※ブレンドツリーの最大値が1の場合を想定
            float targetBlendValue = targetSpeed / mController.StatusProperty.MaxSpeed;

            // 3. SmoothDampを使って、現在のブレンド値(mAnimSpeed)を目標値(targetBlendValue)へ滑らかに変化させる
            mToLiftIdleToRunSpeed = Mathf.SmoothDamp(
                mToLiftIdleToRunSpeed,             // 現在の値
                targetBlendValue,       // 目標の値
                ref mToLiftSmoothVelocity,    // 内部で使用される参照速度（毎回渡す）
                mToLiftSmoothTime         // 目標値に到達するまでにかける時間
            );

            if (mAnimator.GetFloat("to Lift Blend") != mToLiftIdleToRunSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                mAnimator.SetFloat("to Lift Blend", mToLiftIdleToRunSpeed);
            }
        }

        public void SpritDushClear()
        {
            if (mAnimator == null)
            {
                return;
            }

            // 1. 速度を最高速度で正規化し、0～1の値に変換（ブレンドツリーの範囲に合わせる）
            // ※ブレンドツリーの最大値が1の場合を想定
            float targetBlendValue = 0;

            // 2. SmoothDampを使って、現在のブレンド値(mAnimSpeed)を目標値(targetBlendValue)へ滑らかに変化させる
            mSpritDushSpeed = Mathf.SmoothDamp(
                mSpritDushSpeed,             // 現在の値
                targetBlendValue,       // 目標の値
                ref mSpritDushSmoothVelocity,    // 内部で使用される参照速度（毎回渡す）
                mSpritDushSmoothTime         // 目標値に到達するまでにかける時間
            );

            if (mAnimator.GetFloat("spritDush") != mSpritDushSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                mAnimator.SetFloat("spritDush", mSpritDushSpeed);
            }
        }

        public void UpdateCrouchAnimation()
        {
            if (mAnimator == null)
            {
                return;
            }
            float targetSpeed = mController.Rigidbody.linearVelocity.magnitude;
            float targetBlendValue = targetSpeed / mController.StatusProperty.CrouchMaxSpeed;
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

        private void Awake()
        {
            mController = GetComponent<PlayableChracterController>();
            if(mController == null)
            {
                Debug.LogError("PlayableChracterController component not found on " + gameObject.name);
            }

            mAnimator = GetComponentInChildren<Animator>();
            if (mAnimator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }
        }
    }
}
