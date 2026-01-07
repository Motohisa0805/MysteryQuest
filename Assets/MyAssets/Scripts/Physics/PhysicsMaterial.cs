using UnityEngine;

namespace MyAssets
{
    public class PhysicsMaterial : MonoBehaviour
    {
        [SerializeField]
        private GravityCorrection mGravityCorrection;

        private Rigidbody mRigidbody;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            if(mRigidbody == null)
            {
                Debug.LogError("Rigidbody component is missing on " + gameObject.name);
            }
        }

        private void Update()
        {
            mGravityCorrection.Correction(mRigidbody);
        }
    }
}
