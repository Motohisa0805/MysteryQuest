using UnityEngine;

namespace MyAssets
{
    //疑似化学エンジンのオブジェクトスクリプトファイル
    public class ChemistryObject : MonoBehaviour
    {
        [Header("この物体の素材(変わらない)")]
        public MaterialType mMaterial;

        [Header("現在帯びている属性（変化する）")]
        public ElementType mCurrentElements;

        //外部から属性を与える
        public void HitByElement(ElementType incomingElement)
        {
            if(GameSystemManager.Instance.ChemistryTable.TryGetReaction(mMaterial,incomingElement,out ReactionResult result))
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

            MaterialInfluence();
        }

        private void MaterialInfluence()
        {
            if(mCurrentElements == ElementType.Fire)
            {
                Destroy(gameObject);
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
    }
}
