using UnityEngine;

namespace MyAssets
{
    public class RagdollController : MonoBehaviour
    {
        private Transform[] mRagdolls;

        private Rigidbody[] mRagdollBody;

        private float mBaseMass;
        private void Awake()
        {
            CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint>();
            mRagdolls = new Transform[joints.Length];
            mRagdollBody = new Rigidbody[joints.Length];
            for(int i = 0; i < joints.Length; i++)
            {
                mRagdolls[i] = joints[i].transform;
                mRagdollBody[i] = joints[i].connectedBody;
            }
            mBaseMass = mRagdolls[0].GetComponent<Rigidbody>().mass;
        }

        private void Start()
        {
            SetEnabledRagdoll(false);
        }

        public void SetEnabledRagdoll(bool enabled)
        {
            for (int i = 0; i < mRagdolls.Length; i++)
            {
                Collider collider = mRagdolls[i].GetComponent<Collider>();
                Rigidbody rb = mRagdolls[i].GetComponent<Rigidbody>();
                CharacterJoint joint = mRagdolls[i].GetComponent<CharacterJoint>();
                //ラグドールが有効の時
                if(enabled)
                {
                    collider.isTrigger = false;
                    collider.enabled = true;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.mass = mBaseMass;
                    rb.detectCollisions = true;
                    joint.enablePreprocessing = true;
                    joint.connectedBody = mRagdollBody[i];
                }
                //ラグドールが無効の時
                else
                {
                    collider.isTrigger = true;
                    collider.enabled = false;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.mass = 0.0001f;
                    rb.Sleep();
                    rb.detectCollisions = false;
                    joint.enablePreprocessing = false;
                    joint.connectedBody = null;
                }
                //mRagdolls[i].gameObject.SetActive(enabled);
            }
        }
    }
}
