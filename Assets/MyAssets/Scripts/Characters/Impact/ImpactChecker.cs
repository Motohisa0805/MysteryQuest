using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

        public bool IsEnabledFallDamage => mImpactPower > 1000;

        public void ApplyImpactPower(Collision collision)
        {
            //キャラクター自身
            Rigidbody ri = GetComponent<Rigidbody>();
            //衝突者
            Rigidbody targetRb = collision.rigidbody;

            if(targetRb != null && ri.linearVelocity.magnitude < targetRb.linearVelocity.magnitude)
            {
                Apply(targetRb, collision);
            }
            else if(collision.relativeVelocity.magnitude > 10)
            {
                FallApply(ri, collision);
            }
        }

        private void Apply(Rigidbody rigidbody, Collision collision)
        {
            float impactVelocity = collision.relativeVelocity.magnitude;

            float mass = rigidbody != null ? rigidbody.mass : 1f;

            float impactPower = impactVelocity * mass;

            if (impactPower > mMinImpactPower)
            {
                mImpactPower = impactPower;
            }
        }

        private void FallApply(Rigidbody rigidbody, Collision collision)
        {
            float impactVelocity = collision.relativeVelocity.magnitude;

            float mass = rigidbody != null ? rigidbody.mass : 1f;

            float impactPower = impactVelocity * mass;

            if (impactPower > 1000)
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
