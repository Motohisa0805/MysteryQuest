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
            //オブジェクトとプレイヤーの距離
            public float        mDistance;

            //オブジェクトの最終回転変数
            public Quaternion   mRotation;
            //オブジェクトの最初の回転変数
            public Quaternion   mStartRotation;
            //オブジェクトの回転にかかる時間
            public float        mRotationDuration; 
            //オブジェクトの回転にかかる時間を計測する変数
            public float        mRotationElapsedTime;
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

        private float mFollowSpeed = 20f;
        private float mMaxVelocity = 15f;

        public void Setup(Transform transform)
        {
            //自分のTransformの設定
            mBaseTransform = transform;
            //入力処理取得
            mPlayableInput = mBaseTransform.parent.GetComponent<PlayableInput>();
            //演出クラス取得
            mTakeObjectLineVFXController = transform.GetComponentInChildren<TakeObjectLineVFXController>();
            mTakeObjectLineVFXController.SetOriginTransform(transform);
            mTakeObjectLineVFXController.SetEndTransform(transform);
            mTakeObjectLineVFXController.gameObject.SetActive(false);
            mIsTaked = false;
            //変数の初期化
            mRayDistance = 15;
        }

        //取得したオブジェクトを操作する処理
        public void TakeObjectInput()
        {
            if(mChemistryObject == null)return;
            //一度に回転させる角度
            float oneRotate = 45.0f;
            Quaternion deltaRotation = Quaternion.identity;
            bool pressed = false;

            if (InputManager.GetKeyDown(KeyCode.eRightSelect))
            {
                // プレイヤーのUp軸を中心にマイナス回転（時計回り）
                deltaRotation = Quaternion.AngleAxis(oneRotate, mBaseTransform.up);
                pressed = true;
            }
            else if (InputManager.GetKeyDown(KeyCode.eLeftSelect))
            {
                deltaRotation = Quaternion.AngleAxis(-oneRotate, mBaseTransform.up);
                pressed = true;
            }

            if (InputManager.GetKeyDown(KeyCode.eUpSelect))
            {
                // プレイヤーのRight軸を中心に回転
                deltaRotation = Quaternion.AngleAxis(oneRotate, mBaseTransform.right);
                pressed = true;
            }
            else if (InputManager.GetKeyDown(KeyCode.eDownSelect))
            {
                deltaRotation = Quaternion.AngleAxis(-oneRotate, mBaseTransform.right);
                pressed = true;
            }

            if (pressed)
            {
                // 現在の「目標回転」の前に差分を掛けて、プレイヤー基準の回転
                mTakeObjectInfo.mStartRotation = mChemistryObject.transform.rotation;
                mTakeObjectInfo.mRotation = deltaRotation * mTakeObjectInfo.mRotation;

                mTakeObjectInfo.mRotationElapsedTime = 0f;
                SoundManager.Instance.PlayOneShot2D("TakeObjectRotate");
            }
        }
        public void ObjectCheck()
        {
            if (mIsTaked)
            {
                return;
            }
            Vector3 origin = Camera.main.transform.position;
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
                    else
                    {
                        ClearFocus();
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
            SoundManager.Instance.PlayOneShot2D("Select_TakeObject");
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
            mTakeObjectInfo.mRotationElapsedTime = 0f;
            mTakeObjectInfo.mRotation = mChemistryObject.transform.rotation;
            mTakeObjectInfo.mStartRotation = mChemistryObject.transform.rotation;

            //取得した時の距離を取得
            mTakeObjectInfo.mDistance = Math.Abs((mBaseTransform.position - mChemistryObject.transform.position).magnitude);
            //取得フラグを有効に
            mIsTaked = true;

            // 掴んだので、フォーカス変数側は空にしておく（掴んでいる間はObjectCheckが走らないため）
            // ただしレイヤーは戻さない（掴んでいる間も光らせたい場合）
            mFocusedObject = null;

            mTakeObjectLineVFXController.SetEndTransform(mChemistryObject.transform);
            mTakeObjectLineVFXController.gameObject.SetActive(true);

            SoundManager.Instance.PlayOneShot2D("Take_Object");
            mTakingObjectSoundSource = SoundManager.Instance.PlayLoopSE("Taking_Object", mBaseTransform.position, mBaseTransform);
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
                //回転処理
                //指定した間隔処理する
                if(mTakeObjectInfo.mRotationElapsedTime <= mTakeObjectInfo.mRotationDuration)
                {
                    mTakeObjectInfo.mRotationElapsedTime += Time.deltaTime;

                    float duration = Mathf.Max(mTakeObjectInfo.mRotationDuration,0.01f);
                    float t = Mathf.Clamp01(mTakeObjectInfo.mRotationElapsedTime / duration);

                    Quaternion nextRot = Quaternion.Slerp(mTakeObjectInfo.mStartRotation, mTakeObjectInfo.mRotation, t);
                    mTargetRb.MoveRotation(nextRot);
                    if(mTakeObjectInfo.mRotationElapsedTime > mTakeObjectInfo.mRotationDuration)
                    {
                        mTakeObjectInfo.mRotationElapsedTime = mTakeObjectInfo.mRotationDuration;
                    }
                }

                //移動処理
                Vector3 targetWorldPos = mBaseTransform.position + (mBaseTransform.forward * mTakeObjectInfo.mDistance);

                Vector3 diff = targetWorldPos - mTargetRb.position;


                mTargetRb.linearVelocity = Vector3.ClampMagnitude(diff * mFollowSpeed, mMaxVelocity);

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
