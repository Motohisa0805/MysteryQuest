using Unity.VisualScripting;
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

        ReactionResult mReactionResult;

        Timer mReadyElementTimer = new Timer();

        Timer mDestroyTimer = new Timer();

        [SerializeField]
        private float mFireDestoryCount = 5f;

        private void Start()
        {
            mReadyElementTimer.OnEnd += ApplyReaction;
            mDestroyTimer.OnEnd += MaterialDebuff;
        }

        private void Update()
        {
            mDestroyTimer.Update(Time.deltaTime);
            mReadyElementTimer.Update(Time.deltaTime);
        }

        //外部から属性を与える
        private void HitByElement(ElementType incomingElement)
        {
            //同じエレメントなら
            if (mCurrentElements == incomingElement)
            {
                return;
            }
            if (GameSystemManager.Instance.ChemistryTable.TryGetReaction(mMaterial,incomingElement,out ReactionResult result))
            {
                ReadyApplyReaction(result);
            }
            else
            {
                mCurrentElements |= incomingElement;
            }
        }

        private void ApplyReaction()
        {
            if(mReactionResult.gElementToAdd == ElementType.None)
            {
                return;
            }
            //属性の追加
            if (mReactionResult.gElementToAdd != ElementType.None)
            {
                mCurrentElements |= mReactionResult.gElementToAdd;
            }

            //属性の削除
            if (mReactionResult.gElementToRemove != ElementType.None)
            {
                mCurrentElements &= ~mReactionResult.gElementToAdd;
            }

            //エフェクト再生など
            if (!string.IsNullOrEmpty(mReactionResult.mVfxName))
            {

            }

            if (GameSystemManager.Instance.EffectTable.TryGetReaction(mReactionResult.mEffectType, out ParticleSystem effect))
            {
                ParticleSystem obj = Instantiate(effect, transform.position, Quaternion.identity);
                obj.transform.SetParent(transform, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                mDestroyTimer.Start(mFireDestoryCount);
                SoundManager.Instance.PlayOneShot3D(SoundList.SEType.Fire, transform,true, false, mFireDestoryCount);
            }
            mReactionResult = new ReactionResult{ };
        }
        //エレメントを反映させるまでの時間
        //例：火なら燃え上がるまでの時間
        private void ReadyApplyReaction(ReactionResult result)
        {
            //結果を一時保存
            mReactionResult = result;
            //反映するまでの時間が始まっていない＆現在のエレメントが与えるエレメントと違うのなら
            if(mReadyElementTimer.IsEnd()&&mCurrentElements != result.gElementToAdd)
            {
                mReadyElementTimer.Start(result.mDuration);
            }
        }
        //オブジェクトの物理的な処理
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

        private void OnTriggerStay(Collider other)
        {
            ChemistryElement element = other.GetComponentInChildren<ChemistryElement>();
            if (element != null)
            {
                HitByElement(element.Type);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            ChemistryElement element = other.GetComponentInChildren<ChemistryElement>();
            if (element != null)
            {
                if(element.Type == mReactionResult.gElementToAdd)
                {
                    mReadyElementTimer.Reset();
                    mReactionResult = new ReactionResult { };
                }
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
    }
}
