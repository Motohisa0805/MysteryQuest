using UnityEngine;

namespace MyAssets
{
    public class ImpactChecker : MonoBehaviour
    {
        [SerializeField]
        private float mImpactPower;

        public float ImpactPower => mImpactPower;

        private float mMinImpactPower = 1500f;
        private float mMaxImpactPower = 1600f;

        public bool IsEnabledDamage => mImpactPower > 0;
        public bool IsEnabledSmallDamage => mImpactPower > mMinImpactPower && mImpactPower < mMaxImpactPower;
        public bool IsEnabledBigDamage => mImpactPower > mMaxImpactPower && mImpactPower > mMinImpactPower;

        public void ApplyImpactPower(Collision collision)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            float impactVelocity = collision.relativeVelocity.magnitude;

            float mass = collision.rigidbody != null ? collision.rigidbody.mass : 1f;

            float impactPower = impactVelocity * mass;

            if(impactPower > mMinImpactPower)
            {
                mImpactPower = impactPower;
            }
        }
        public void ClearImpactPower()
        {
            mImpactPower = 0f;
        }
    }
}
