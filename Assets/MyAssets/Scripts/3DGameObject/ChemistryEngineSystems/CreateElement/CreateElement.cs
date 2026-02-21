using UnityEngine;

namespace MyAssets
{
    //ステージギミックで使用する一回だけエレメントを生成する処理
    public class CreateElement : MonoBehaviour
    {
        [Header("生成するエレメントの名前")]
        [SerializeField]
        private string mElementName;

        [SerializeField]
        private Vector3 mLocalOffsetPos;

        private void Create()
        {
            if (EffectManager.Instance)
            {
                // 1. エフェクトを生成
                EffectReturner effect = EffectManager.Instance.PlayEffect<EffectReturner>(mElementName, transform.position, Quaternion.identity, Vector3.one, transform);
                if (effect)
                {
                    effect.transform.localPosition = mLocalOffsetPos;
                    Destroy(this);
                }
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            Create();
        }

        private void Update()
        {
            Create();
        }
    }
}
