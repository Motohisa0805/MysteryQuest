using UnityEngine;

namespace MyAssets
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class CapsuleColliderController : MonoBehaviour
    {
        private CapsuleCollider mCapsuleCollider;
        public CapsuleCollider CapsuleCollider => mCapsuleCollider;

        [SerializeField]
        private Vector3 mBaseCenter;

        [SerializeField]
        private float mBaseHeight;

        [SerializeField]
        private float mBaseRadius;

        private void Awake()
        {
            mCapsuleCollider = GetComponent<CapsuleCollider>();
        }
        private void Start()
        {
            mBaseCenter = mCapsuleCollider.center;
            mBaseHeight = mCapsuleCollider.height;
            mBaseRadius = mCapsuleCollider.radius;
        }

        public void ResetCollider()
        {
            mCapsuleCollider.center = mBaseCenter;
            mCapsuleCollider.height = mBaseHeight;
            mCapsuleCollider.radius = mBaseRadius;
        }
        public void SetHeight(float height)
        {
            mCapsuleCollider.height = height;
        }
        public void SetRadius(float radius)
        {
            mCapsuleCollider.radius = radius;
        }
        public void SetCenter(Vector3 center)
        {
            mCapsuleCollider.center = center;
        }

        public void SetRagdollModeCollider()
        {
            mCapsuleCollider.radius = 0.1f;
            mCapsuleCollider.height = 0.1f;
        }
    }
}
