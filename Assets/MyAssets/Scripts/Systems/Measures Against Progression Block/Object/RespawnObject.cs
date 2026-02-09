using UnityEngine;

namespace MyAssets
{
    public class RespawnObject : MonoBehaviour
    {
        private ObjectRespawner mObjectRespawner;
        //RigidbodyéQè∆ópïœêî
        //Ç†Ç¡ÇƒÇ‡Ç»Ç≠ÇƒÇ‡ó«Ç¢ÇÊÇ§Ç…ê›åv
        private Rigidbody       mRigidbody;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
        }
        public void Setup(ObjectRespawner objectRespawner)
        {
            mObjectRespawner = objectRespawner;
        }

        private void OnTriggerEnter(Collider other)
        {
            AbyssArea abyssArea = other.GetComponent<AbyssArea>();
            if (abyssArea != null)
            {
                if(mRigidbody)
                {
                    mRigidbody.linearVelocity = Vector3.zero;
                }
                mObjectRespawner.Respawn(this, transform.rotation);
            }
        }
    }
}