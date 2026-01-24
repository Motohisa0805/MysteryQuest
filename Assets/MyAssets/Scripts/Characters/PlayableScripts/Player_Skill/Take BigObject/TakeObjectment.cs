using System;
using UnityEngine;

namespace MyAssets
{
    //オブジェクトの取得、チェックを行うクラス
    [Serializable]
    public class TakeObjectment
    {
        //オブジェクトを取得した時のオフセット情報
        [Serializable]
        public struct TakeObjectInfo
        {
            public float mDistance;

            public Quaternion mRotation;
        }

        private Transform       mBaseTransform;

        private PlayableInput   mPlayableInput;


        [SerializeField]
        private float           mRayDistance = 5f;

        [SerializeField]
        private LayerMask       mHitMask;

        private RaycastHit      mInteractableHit;

        //取得したオブジェクトを格納する変数
        [SerializeField]
        private ChemistryObject mChemistryObject;
        public ChemistryObject ChemistryObject => mChemistryObject;
        // 今見ている（選択候補の）オブジェクト
        private ChemistryObject mFocusedObject;      
        // Rigidbodyを保持
        private Rigidbody       mTargetRb;
        [Header("取得しているか")]
        [SerializeField]
        private bool            mIsTaked;

        [SerializeField]
        private TakeObjectInfo  mTakeObjectInfo;


        private AudioSource     mTakingObjectSoundSource;

        private TakeObjectLineVFXController mTakeObjectLineVFXController;

        public void Setup(Transform transform)
        {
            mBaseTransform = transform;
            mPlayableInput = mBaseTransform.parent.GetComponent<PlayableInput>();
            mTakeObjectLineVFXController = transform.GetComponentInChildren<TakeObjectLineVFXController>();
            mTakeObjectLineVFXController.SetOriginTransform(transform);
            mTakeObjectLineVFXController.SetEndTransform(transform);
            mTakeObjectLineVFXController.gameObject.SetActive(false);
            mIsTaked = false;
        }

        public void ObjectCheck()
        {
            if (mIsTaked)
            {
                return;
            }
            Vector3 origin = mBaseTransform.position;
            Ray checkRay = new Ray(origin, mBaseTransform.forward);

            // レイキャスト判定
            if (Physics.Raycast(checkRay, out mInteractableHit, mRayDistance, mHitMask))
            {
                GameObject hitObj = mInteractableHit.collider.gameObject;

                // プレイヤー自身は無視
                if (hitObj != mPlayableInput.gameObject)
                {
                    // ChemistryObjectを持っているか確認
                    if (hitObj.TryGetComponent<ChemistryObject>(out ChemistryObject target))
                    {
                        // 新しいオブジェクトを見つけた瞬間だけ処理
                        if (mFocusedObject != target)
                        {
                            // 前のオブジェクトを解除
                            ClearFocus();
                            // 新しいオブジェクトをセット
                            SetFocus(target);
                        }
                    }
                }
            }
            else
            {
                //当たってないなら解除
                ClearFocus();
            }

            Debug.DrawRay(checkRay.origin, mBaseTransform.forward * mRayDistance, Color.green);

            // 掴む処理
            if (mFocusedObject != null && mPlayableInput.Interact)
            {
                GrabObject();
            }
        }

        private void SetFocus(ChemistryObject target)
        {
            mFocusedObject = target;
            EffectManager.Instance.ObjectMaterialSelector.ActivateEffect(mFocusedObject.gameObject);
            SoundManager.Instance.PlayOneShot2D(1014);
        }

        public void ClearFocus()
        {
            if (mFocusedObject == null) return;
            EffectManager.Instance.ObjectMaterialSelector.DeactivateEffect(mFocusedObject.gameObject);

            mFocusedObject = null;
        }

        // 掴む処理
        private void GrabObject()
        {
            mChemistryObject = mFocusedObject;

            if (mChemistryObject.TryGetComponent<Rigidbody>(out mTargetRb))
            {
                mTargetRb.useGravity = false;
                mTargetRb.isKinematic = false;
            }
            //取得した時の回転を取得
            mTakeObjectInfo.mRotation = mChemistryObject.transform.rotation;

            //取得した時の距離を取得
            mTakeObjectInfo.mDistance = Math.Abs((mBaseTransform.position - mChemistryObject.transform.position).magnitude);
            //取得フラグを有効に
            mIsTaked = true;

            // 掴んだので、フォーカス変数側は空にしておく（掴んでいる間はObjectCheckが走らないため）
            // ただしレイヤーは戻さない（掴んでいる間も光らせたい場合）
            mFocusedObject = null;

            mTakeObjectLineVFXController.SetEndTransform(mChemistryObject.transform);
            mTakeObjectLineVFXController.gameObject.SetActive(true);

            SoundManager.Instance.PlayOneShot2D(1015);
            mTakingObjectSoundSource = SoundManager.Instance.PlayLoopSE(1016, mBaseTransform.position, mBaseTransform);
        }

        public void UpdateTakeObject()
        {
            //オブジェクトが取得途中でなくなった時の処理
            if(mChemistryObject == null)
            {
                mTakeObjectLineVFXController.SetEndTransform(mBaseTransform);
                mTakeObjectLineVFXController.gameObject.SetActive(false);
                mIsTaked = false;
                mChemistryObject = null;
                return;
            }

            if (mIsTaked && mTargetRb != null)
            {
                mTargetRb.MoveRotation(mTakeObjectInfo.mRotation);
                Vector3 targetWorldPos = mBaseTransform.position + (mBaseTransform.forward * mTakeObjectInfo.mDistance);
                mTargetRb.MovePosition(targetWorldPos);
                mTargetRb.linearVelocity = Vector3.zero;
                mTargetRb.angularVelocity = Vector3.zero;
            }

        }

        public void InputTakeOffObject()
        {
            if (mPlayableInput.Sprit)
            {
                TakeOffObject();
            }
        }

        public void TakeOffObject()
        {
            if (mChemistryObject == null) return;

            if (mTargetRb != null)
            {
                mTargetRb.useGravity = true;
                mTargetRb.linearVelocity = Vector3.zero;
                mTargetRb = null;
            }
            SoundManager.Instance.StopLoopSE(mTakingObjectSoundSource);
            EffectManager.Instance.ObjectMaterialSelector.DeactivateEffect(mChemistryObject.gameObject);
            mTakeObjectLineVFXController.SetEndTransform(mBaseTransform);
            mTakeObjectLineVFXController.gameObject.SetActive(false);
            mIsTaked = false;
            mChemistryObject = null;
        }
    }
}
