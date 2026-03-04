using System.Collections.Generic;
using UnityEngine;


namespace MyAssets
{
    //3Dオブジェクトの破壊時にモデルを分割したものをエフェクトとして表示、処理するクラス
    //このスクリプト自体は破壊可能オブジェクト(現在は氷)の子オブジェクトにアタッチする形だけで使用するので
    //現状はEffectManagerで管理するかは今後変更予定
    public class ModelBreakEffect : MonoBehaviour
    {

        private List<Rigidbody> mModelParts = new List<Rigidbody>();


        [SerializeField]
        private float           mDestroyCount = 1.0f;
        [SerializeField]
        private float           mExplosionForce = 300.0f;
        [SerializeField]
        private float           mExplosionRadius = 5.0f;
        private void Awake()
        {
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            mModelParts.Clear();
            mModelParts = new List<Rigidbody>(rigidbodies);
        }

        private void Start()
        {
            foreach(Rigidbody rigidbody in mModelParts)
            {
                if(rigidbody.gameObject.activeSelf)
                {
                    rigidbody.gameObject.SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            foreach (Rigidbody rigidbody in mModelParts)
            {
                if (rigidbody.gameObject.activeSelf)
                {
                    rigidbody.gameObject.SetActive(false);
                }
            }
        }


        public void RunEffect()
        {
            Vector3 explosionPos = transform.parent.position + (Vector3.down * 0.5f) + (Vector3.forward * 0.1f);

            foreach (Rigidbody rigidbody in mModelParts)
            {
                if (!rigidbody.gameObject.activeSelf)
                {
                    rigidbody.gameObject.SetActive(true);
                    rigidbody.isKinematic = false;
                    // 力を加える
                    rigidbody.AddExplosionForce(mExplosionForce, explosionPos, mExplosionRadius);
                }
            }
            transform.parent = null;
            Destroy(gameObject, mDestroyCount);
        }

    }
}
