using UnityEngine;

namespace MyAssets
{
    public class SetItemTransform : MonoBehaviour
    {
        public enum TransformType
        {
            None,
            Right,
            Left,
            Weapon,
            Shield
        }

        private TransformType mHandType = TransformType.None;
        public TransformType Type 
        {
            get 
            {
                if(mHandType == TransformType.None)
                {
                    string name = transform.name;
                    mHandType = TransformType.Right;
                    if (name.Contains("Left"))
                    {
                        mHandType = TransformType.Left;
                    }
                    else if (name.Contains("Weapon"))
                    {
                        mHandType = TransformType.Weapon;
                    }
                    else if (name.Contains("Shield"))
                    {
                        mHandType = TransformType.Shield;
                    }
                }
                return mHandType; 
            } 
        }

        [SerializeField]
        private int mHaveItemID = -1;
        public int HaveItemID => mHaveItemID;
        public void SetItemID(int itemID) { mHaveItemID = itemID; }
        private GameObject mHaveObject;
        public GameObject HaveObject { get { return mHaveObject; }set { mHaveObject = value; } }

        public Collider GetCollider()
        {
            if (mHaveObject == null) return null;
            return mHaveObject.GetComponent<Collider>();
        }

        public float VectorSize() { return transform.position.magnitude; }

        private void Start()
        {
            string name = transform.name;
            mHandType = TransformType.Right;
            if (name.Contains("Left"))
            {
                mHandType = TransformType.Left;
            }
            else if (name.Contains("Weapon"))
            {
                mHandType = TransformType.Weapon;
            }
            else if (name.Contains("Shield"))
            {
                mHandType = TransformType.Shield;
            }
        }
    }
}
