using UnityEngine;

namespace MyAssets
{
    public class SwordStick : MonoBehaviour
    {
        // 剣の先端など、刺す位置の目印となるTransform
        [SerializeField]
        private Transform mStickPoint; 
        // 現在刺さっているオブジェクト
        private GameObject mStuckObject;
        public bool IsHasStuckObject => mStuckObject != null;

        private Vector3 mStickPointOffset  = new Vector3(0, 0.7f, 0);

        private void Awake()
        {
            mStickPoint = transform;
        }

        private void StickObject(GameObject obj)
        {
            obj.transform.SetParent(mStickPoint);
            // 刺す位置にオブジェクトを移動させ、親子関係を設定
            obj.transform.position = mStickPoint.position;
            obj.transform.localPosition = mStickPointOffset;
            obj.transform.rotation = mStickPoint.rotation;
            mStuckObject = obj;

            Collider collider = obj.GetComponent<Collider>();
            if (collider != null)
            {
                // 刺さったオブジェクトのコライダーを無効にする（必要に応じて）
                collider.isTrigger = true;
            }
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                // 刺さったオブジェクトの物理挙動を停止する（必要に応じて）
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }
        }

        public void RemoveStuckObject()
        {
            if (mStuckObject != null)
            {
                Collider collider = mStuckObject.GetComponent<Collider>();
                if (collider != null)
                {
                    // コライダーを元に戻す
                    collider.isTrigger = false;
                }
                Rigidbody rigidbody = mStuckObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    // 物理挙動を元に戻す
                    rigidbody.isKinematic = false;
                    rigidbody.useGravity = true;
                }
                // 刺さっているオブジェクトを剣から外す
                mStuckObject.transform.SetParent(null);
                mStuckObject = null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ChemistryObject chemistryObject = other.GetComponent<ChemistryObject>();
            ObjectSizeType objectSizeType = other.GetComponent<ObjectSizeType>();
            if (chemistryObject != null && objectSizeType != null && mStuckObject == null)
            {
                if(objectSizeType.Size == ObjectSizeType.SizeType.Medium)
                {
                    // 刺す処理
                    StickObject(chemistryObject.gameObject);
                }
            }
        }
    }
}
