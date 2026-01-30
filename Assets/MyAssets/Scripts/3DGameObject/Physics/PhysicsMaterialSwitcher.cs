using UnityEngine;

namespace MyAssets
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsMaterialSwitcher : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private PhysicsMaterial mMovingMaterial;   // 移動用（摩擦小など）
        [SerializeField]
        private PhysicsMaterial mIdleMaterial;      // 静止用（摩擦大など）
        [SerializeField]
        private float           mVelocityThreshold = 0.05f;   // 静止とみなす速度のしきい値

        private Rigidbody       mRigidbody;
        private Collider        mCollider;
        private bool            mIsMoveing = false;

        private bool            mIsVisible = false;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            mCollider = GetComponent<Collider>();
            enabled = false;
        }

        // 画面内に入った
        void OnBecameVisible()
        {
            mIsVisible = true;
            enabled = true; // Update/FixedUpdateを有効化
        }

        // 画面外に出た
        void OnBecameInvisible()
        {
            mIsVisible = false;
            enabled = false; // 処理を完全に停止

            // 画面外では勝手に動かないよう、静止用マテリアルを強制適用しておく
            SetMaterial(mIdleMaterial);
        }

        private void FixedUpdate()
        {
            if (!mIsVisible) return;
            // 速度の絶対値（magnitude）で判定
            float currentVelocity = mRigidbody.linearVelocity.magnitude;

            if (mIsMoveing && currentVelocity < mVelocityThreshold)
            {
                SetMaterial(mIdleMaterial);
                mIsMoveing = false;
            }
            else if (!mIsMoveing && currentVelocity >= mVelocityThreshold)
            {
                SetMaterial(mMovingMaterial);
                mIsMoveing = true;
            }
        }

        private void SetMaterial(PhysicsMaterial material)
        {
            mCollider.material = material;
        }
    }
}
