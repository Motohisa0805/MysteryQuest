using UnityEngine;

namespace MyAssets
{
    public class TakedObjectChecker : MonoBehaviour
    {
        [SerializeField]
        private ChemistryObject mTakedObject;

        public ChemistryObject TakedObject => mTakedObject;

        private bool mHasTakedObject;

        public void SetReleaseTakedObject() 
        {
            mHasTakedObject = false;
            Rigidbody rigidbody = mTakedObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.useGravity = true;
            }
            Collider collider = mTakedObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
        }

        [SerializeField]
        private HandTransform[] mHandTransforms = new HandTransform[2];

        public Vector3 GetObjectsYouHavePoint()
        {
            if (mHandTransforms[0] == null || mHandTransforms[1] == null)
            {
                return transform.position;
            }
            Vector3 vec1 = mHandTransforms[0].gameObject.transform.position;
            Vector3 vec2 = mHandTransforms[1].gameObject.transform.position;
            return new Vector3((vec1.x + vec2.x) / 2, (vec1.y + vec2.y) / 2, (vec1.z + vec2.z) / 2);
        }

        private void Awake()
        {
            mHandTransforms = transform.GetComponentsInChildren<HandTransform>();
        }

        public void CheckTheDistanceHandsAndObject()
        {
            if (mTakedObject == null||mHasTakedObject) return;

            float minDis = mTakedObject.transform.position.y - GetObjectsYouHavePoint().y;
            if (Mathf.Abs(minDis) < 0.01f)
            {
                mHasTakedObject = true;
                Rigidbody rigidbody = mTakedObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.useGravity = false;
                }
                Collider collider = mTakedObject.GetComponent<Collider>();
                if(collider != null)
                {
                    collider.isTrigger = true;
                }
            }
        }

        public void UpdateTakedObjectPosition()
        {
            if(!mHasTakedObject) return;
            mTakedObject.transform.position = Vector3.Lerp(mTakedObject.transform.position, GetObjectsYouHavePoint(), 1.0f);
            mTakedObject.transform.rotation = transform.rotation;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!mHasTakedObject)
            {
                ChemistryObject obj = other.GetComponent<ChemistryObject>();
                if (obj != null)
                {
                    mTakedObject = obj;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(!mHasTakedObject)
            {
                ChemistryObject obj = other.GetComponent<ChemistryObject>();
                if (obj == mTakedObject)
                {
                    mTakedObject = null;
                }
            }
        }
    }
}
