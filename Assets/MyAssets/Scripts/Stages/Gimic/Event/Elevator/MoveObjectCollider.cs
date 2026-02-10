using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //オブジェクトが動く際に、その上に乗っているオブジェクトも一緒に動かすためのコンポーネント
    public class MoveObjectCollider : MonoBehaviour
    {
        //必須コンポーネント
        private BoxCollider     mBoxCollider;
        //メッシュコライダーはオプション
        private MeshCollider    mMeshCollider;
        //乗っているオブジェクトのRigidbodyリスト
        private List<Rigidbody> mAttachedRigidbodies = new List<Rigidbody>();
        //前回の位置
        private Vector3 mLastPosition;
        private void Awake()
        {
            mBoxCollider = GetComponent<BoxCollider>();
            if (mBoxCollider == null)
            {
                Debug.LogError("MoveObjectCollider: BoxCollider component is missing.");
            }
            //メッシュはあってもなくても良い
            //その場合はnullになる
            mMeshCollider = GetComponent<MeshCollider>();
            mLastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            // 1. 移動量（delta）を計算
            Vector3 deltaMovement = transform.position - mLastPosition;

            for (int i = 0; i < mAttachedRigidbodies.Count; i++)
            {
                Rigidbody rb = mAttachedRigidbodies[i];
                if (rb != null)
                {
                    // 2. 乗っているオブジェクトを同じ分だけ動かす
                    rb.MovePosition(rb.position + deltaMovement);
                }
            }
            // 3. 現在の位置を保存
            mLastPosition = transform.position;
        }

        public void EnableColliders()
        {
            if (mBoxCollider != null)
            {
                mBoxCollider.enabled = true;
            }
            if (mMeshCollider != null)
            {
                mMeshCollider.enabled = false;
            }
        }

        public void DisableColliders()
        {
            if (mBoxCollider != null)
            {
                mBoxCollider.enabled = false;
            }
            if (mMeshCollider != null)
            {
                mMeshCollider.enabled = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Rigidbody rb = collision.collider.attachedRigidbody;
            if (rb != null && !mAttachedRigidbodies.Contains(rb))
            {
                mAttachedRigidbodies.Add(rb);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            Rigidbody rb = collision.collider.attachedRigidbody;
            if (rb != null && mAttachedRigidbodies.Contains(rb))
            {
                mAttachedRigidbodies.Remove(rb);
            }
        }
    }

}
