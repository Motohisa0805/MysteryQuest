using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace MyAssets
{
    public class PlayableAnimationFunction : MonoBehaviour
    {
        private PlayableChracterController mController;

        private Animator mAnimator; //アニメーター
        public Animator Animator 
        {
            get 
            {
                if (mAnimator == null)
                {
                    mAnimator = GetComponentInChildren<Animator>();
                }
                return mAnimator; 
            } 
        }
        public void SetAnimatorEnabled(bool enabled)
        {
            if(Animator == null) { return; }
            Animator.enabled = enabled;
        }

        private float mCurrentLayer3Weight;
        private float mTargetLayer3Weight;
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        [Header("Layer3の変数")]
        [Header("ブレンドにかける時間")]
        [SerializeField]
        private float mLayer3WeightSmoothTime = 0.1f;
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mLayer3WeightSmoothVelocity = 0f;

        public void SetAnimatorLayerWeight(int layer,float layerWeight)
        {
            Animator.SetLayerWeight(layer, layerWeight);
        }

        public void StartUpdateAnimatorLayerWeight(int layer, float layerWeight)
        {
            if (layer == 0)
            {
                Animator.SetLayerWeight(layer, layerWeight);
            }
            else if (layer == 1)
            {
                Animator.SetLayerWeight(layer, layerWeight);
            }
            else if (layer == 2)
            {
                mTargetLayer3Weight = layerWeight;
            }
        }

        public void UpdateLayer3Weight()
        {
            if (Animator == null)
            {
                return;
            }

            // 2. SmoothDampを使って、現在のブレンド値(mAnimSpeed)を目標値(targetBlendValue)へ滑らかに変化させる
            mCurrentLayer3Weight = Mathf.SmoothDamp(
                mCurrentLayer3Weight,               // 現在の値
                mTargetLayer3Weight,                // 目標の値
                ref mLayer3WeightSmoothVelocity,    // 内部で使用される参照速度（毎回渡す）
                mLayer3WeightSmoothTime             // 目標値に到達するまでにかける時間
            );

            if (Animator.GetLayerWeight(2) != mCurrentLayer3Weight)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                Animator.SetLayerWeight(2, mCurrentLayer3Weight);
            }
            
        }

        //アニメーションのブレンドを滑らかにするための変数
        // 現在アニメーターに渡しているブレンド値
        private float mAnimIdleToRunSpeed = 0f;          
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        private float mAnimSmoothTime = 0.1f;   
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mSmoothVelocity = 0f;     

        //アニメーションのブレンドを滑らかにするための変数
        // 現在アニメーターに渡しているブレンド値
        private float mSpritDushSpeed = 0f;          
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        private float mSpritDushSmoothTime = 0.1f;   
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mSpritDushSmoothVelocity = 0f;     

        //アニメーションのブレンドを滑らかにするための変数
        // 現在アニメーターに渡しているブレンド値
        private float mFocusingMoveXSpeed = 0f;          
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        private float mFocusingMoveXSmoothTime = 0.1f;   
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mFocusingMoveXSmoothVelocity = 0f;     

        //アニメーションのブレンドを滑らかにするための変数
        // 現在アニメーターに渡しているブレンド値
        private float mFocusingMoveYSpeed = 0f;          
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        private float mFocusingMoveYSmoothTime = 0.1f;   
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mFocusingMoveYSmoothVelocity = 0f;     

        //アニメーションのブレンドを滑らかにするための変数
        // 現在アニメーターに渡しているブレンド値
        private float mToLiftIdleToRunSpeed = 0f;          
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        private float mToLiftSmoothTime = 0.1f;   
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mToLiftSmoothVelocity = 0f;     

        //アニメーションのブレンドを滑らかにするための変数
        // 現在アニメーターに渡しているブレンド値
        private float mCrouchAnimSpeed = 0f;          
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        private float mCrouchAnimSmoothTime = 0.1f;   
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mCrouchSmoothVelocity = 0f;


        private float mCurrentModeBlend = 0;
        private float mTargetModeBlend = 0;
        // ブレンドにかける時間 (0.1秒程度が滑らか)
        private float mModeBlendSmoothTime = 0.1f;
        // SmoothDampで使用する参照速度（内部で自動更新される）
        private float mModeBlendSmoothVelocity = 0f;
        public float GetModeBlend()
        {
            return Animator.GetFloat("modeBlend");
        }
        public void SetModeBlend(int blend)
        {
            mTargetModeBlend = blend;
        }
        public void UpdateModeBlend()
        {
            if (Animator == null)
            {
                return;
            }

            // 2. SmoothDampを使って、現在のブレンド値(mAnimSpeed)を目標値(targetBlendValue)へ滑らかに変化させる
            mCurrentModeBlend = Mathf.SmoothDamp(
                mCurrentModeBlend,             // 現在の値
                mTargetModeBlend,       // 目標の値
                ref mModeBlendSmoothVelocity,    // 内部で使用される参照速度（毎回渡す）
                mModeBlendSmoothTime         // 目標値に到達するまでにかける時間
            );

            if (Animator.GetFloat("modeBlend") != mCurrentModeBlend)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                Animator.SetFloat("modeBlend", mCurrentModeBlend);
            }
        }



        public bool IsToolState()
        {
            return Animator.GetBool("tool State");
        }

        public void SetToolState(bool state)
        {
            Animator.SetBool("tool State", state);
        }
        public bool IsBattleMode()
        {
            return Animator.GetBool("battle Mode");
        }

        public void SetBattleMode(bool state)
        {
            Animator.SetBool("battle Mode", state);
        }

        public void MoveStateClear()
        {
            if (Animator == null)
            {
                return;
            }
            Animator.SetFloat("idleToRun", 0);
            Animator.SetFloat("spritDush", 0);
            Animator.SetFloat("focusing MoveX", 0);
            Animator.SetFloat("focusing MoveY", 0);
            Animator.SetFloat("spritDush", 0);
        }

        public void UpdateIdleToRunAnimation()
        {
            if (Animator == null)
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

            if (Animator.GetFloat("idleToRun") != mAnimIdleToRunSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                Animator.SetFloat("idleToRun", mAnimIdleToRunSpeed);
            }
        }

        public void UpdateFocusingMoveAnimation()
        {
            if (Animator == null)
            {
                return;
            }
            float x = mController.Input.InputMove.x;
            mFocusingMoveXSpeed = Mathf.SmoothDamp(
                mFocusingMoveXSpeed,                // 現在の値
                x,                                  // 目標の値
                ref mFocusingMoveXSmoothVelocity,   // 内部で使用される参照速度（毎回渡す）
                mFocusingMoveXSmoothTime            // 目標値に到達するまでにかける時間
            );
            if (Animator.GetFloat("focusing MoveX") != mFocusingMoveXSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                Animator.SetFloat("focusing MoveX", mFocusingMoveXSpeed);
            }
            float y = mController.Input.InputMove.y;
            mFocusingMoveYSpeed = Mathf.SmoothDamp(
                mFocusingMoveYSpeed,                // 現在の値
                y,                                  // 目標の値
                ref mFocusingMoveYSmoothVelocity,   // 内部で使用される参照速度（毎回渡す）
                mFocusingMoveYSmoothTime            // 目標値に到達するまでにかける時間
            );
            if (Animator.GetFloat("focusing MoveY") != mFocusingMoveYSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                Animator.SetFloat("focusing MoveY", mFocusingMoveYSpeed);
            }
        }

        public void UpdateSpritDushAnimation()
        {
            if (Animator == null)
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

            if (Animator.GetFloat("spritDush") != mSpritDushSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                Animator.SetFloat("spritDush", mSpritDushSpeed);
            }
        }

        public void UpdateToLiftIdleToToLiftRunAnimation()
        {
            if (Animator == null)
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

            if (Animator.GetFloat("to Lift Blend") != mToLiftIdleToRunSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                Animator.SetFloat("to Lift Blend", mToLiftIdleToRunSpeed);
            }
        }

        public void SpritDushClear()
        {
            if (Animator == null)
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

            if (Animator.GetFloat("spritDush") != mSpritDushSpeed)
            {
                // 4. アニメーターに滑らかになったブレンド値を渡す
                // mAnimator.SetFloat("idleToRun", mRigidbody.linearVelocity.magnitude); // 修正前
                Animator.SetFloat("spritDush", mSpritDushSpeed);
            }
        }

        public void UpdateCrouchAnimation()
        {
            if (Animator == null)
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
            if (Animator.GetFloat("crouch_IdleToWalk") != mCrouchAnimSpeed)
            {
                Animator.SetFloat("crouch_IdleToWalk", mCrouchAnimSpeed);
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
