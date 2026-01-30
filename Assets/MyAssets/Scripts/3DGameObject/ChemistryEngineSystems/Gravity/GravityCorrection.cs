using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class GravityCorrection
    {

        private float mGravityMultiply = 2;

        private float maxFallSpeed = 20f;

        public void Correction(Rigidbody rigidbody)
        {
            rigidbody.linearVelocity += Physics.gravity * mGravityMultiply * Time.deltaTime;
            if (rigidbody.linearVelocity.y < -maxFallSpeed)
            {
                rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, -maxFallSpeed, rigidbody.linearVelocity.z);
            }
        }
    }
}
