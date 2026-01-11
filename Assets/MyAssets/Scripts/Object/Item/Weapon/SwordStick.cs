using UnityEngine;

namespace MyAssets
{
    public class SwordStick : MonoBehaviour
    {
        // 剣の先端など、刺す位置の目印となるTransform
        [SerializeField]
        private Transform mStickPoint; 
        // 現在刺さっているオブジェクト
        private StickObject mStuckObject;
        public bool IsHasStuckObject => mStuckObject != null;

        private Vector3 mStickPointOffset  = new Vector3(0, 1.0f, 0);

        private void Awake()
        {
            mStickPoint = transform;
        }

        private void StickObject(StickObject obj)
        {
            obj.gameObject.transform.SetParent(mStickPoint);
            // 刺す位置にオブジェクトを移動させ、親子関係を設定
            obj.gameObject.transform.position = mStickPoint.position;
            obj.gameObject.transform.localPosition = mStickPointOffset;
            obj.gameObject.transform.rotation = mStickPoint.rotation;
            mStuckObject = obj;
            mStuckObject.SetReductionSize();


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
                mStuckObject.BaseSizeRaito();
                // 刺さっているオブジェクトを剣から外す
                mStuckObject.transform.SetParent(null);
                mStuckObject = null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            StickObject stickObject = other.GetComponent<StickObject>();
            if (stickObject != null && mStuckObject == null)
            {
                // 刺す処理
                StickObject(stickObject);
            }
        }
    }
}
