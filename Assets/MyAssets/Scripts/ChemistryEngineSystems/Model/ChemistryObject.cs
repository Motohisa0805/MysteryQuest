using UnityEngine;

namespace MyAssets
{
    //疑似化学エンジンのオブジェクトスクリプトファイル
    public class ChemistryObject : MonoBehaviour
    {
        [Header("この物体の素材(変わらない)")]
        [SerializeField]
        private MaterialType mMaterial;

        public MaterialType Material => mMaterial;

        [Header("現在帯びている属性（変化する）")]
        [SerializeField]
        private ElementType mCurrentElements;

        public ElementType CurrentElements => mCurrentElements;

        [SerializeField]
        private float mFireDestoryCount = 5f;

        //外部から属性を与える
        public void HitByElement(ElementType incomingElement)
        {
            //同じエレメントなら
            if (mCurrentElements == incomingElement)
            {
                return;
            }
            if (GameSystemManager.Instance.ChemistryTable.TryGetReaction(mMaterial,incomingElement,out ReactionResult result))
            {
                ApplyReaction(result);
            }
            else
            {
                mCurrentElements |= incomingElement;
            }
        }

        private void ApplyReaction(ReactionResult result)
        {
            //属性の追加
            if(result.gElementToAdd != ElementType.None)
            {
                mCurrentElements |= result.gElementToAdd;
            }

            //属性の削除
            if (result.gElementToRemove != ElementType.None)
            {
                mCurrentElements &= ~result.gElementToAdd;
            }

            //エフェクト再生など
            if(!string.IsNullOrEmpty(result.mVfxName))
            {

            }

            if (GameSystemManager.Instance.EffectTable.TryGetReaction(result.mEffectType, out ParticleSystem effect))
            {
                ParticleSystem obj = Instantiate(effect, transform.position, Quaternion.identity);
                obj.transform.SetParent(transform, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
            }
            //オブジェクトの物理的な処理
            MaterialDebuff();
        }

        private void MaterialDebuff()
        {
            if(mCurrentElements == ElementType.Fire)
            {
                Destroy(gameObject, mFireDestoryCount);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            ChemistryElement element = other.GetComponentInChildren<ChemistryElement>();
            if (element != null)
            {
                HitByElement(element.Type);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            ChemistryObject obejct = collision.gameObject.GetComponent<ChemistryObject>();
            if (obejct != null&&obejct.CurrentElements != ElementType.None)
            {
                HitByElement(obejct.CurrentElements);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            ChemistryObject obejct = collision.gameObject.GetComponent<ChemistryObject>();
            if (obejct != null && obejct.CurrentElements != ElementType.None)
            {
                HitByElement(obejct.CurrentElements);
            }
        }
        /*
         */
    }
}
