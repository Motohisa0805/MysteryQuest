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

        private Transform                   mBaseTransform;

        private PlayableChracterController  mPlayerController;

        private PlayableInput               mPlayableInput;


        [SerializeField]
        private float                       mRayDistance = 5f;

        [SerializeField]
        private LayerMask                   mHitMask;

        private RaycastHit                  mInteractableHit;

        //取得したオブジェクトを格納する変数
        [SerializeField]
        private ObjectSizeType              mObjectSizeType;
        public ObjectSizeType               ObjectSizeType => mObjectSizeType;
        // 今見ている（選択候補の）オブジェクト
        private ObjectSizeType              mFocusedObject;      
        // Rigidbodyを保持
        private Rigidbody                   mTargetRb;
        private Collider                    mTargetCollider;
        [Header("取得しているか")]
        [SerializeField]
        private bool                        mIsTaked;

        [SerializeField]
        private TakeObjectInfo              mTakeObjectInfo;


        private AudioSource                 mTakingObjectSoundSource;

        private TakeObjectLineVFXController mTakeObjectLineVFXController;

        private float                       mFollowSpeed = 20f;
        private float                       mMaxVelocity = 15f;

        public void Setup(Transform transform)
        {
            //自分のTransformの設定
            mBaseTransform = transform;
            mPlayerController = mBaseTransform.parent.GetComponent<PlayableChracterController>();
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
            if(mObjectSizeType == null)return;
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
                mTakeObjectInfo.mStartRotation = mObjectSizeType.transform.rotation;
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
                    if (hitObj.TryGetComponent<ObjectSizeType>(out ObjectSizeType target))
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
        private void SetFocus(ObjectSizeType target)
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
            SoundManager.Instance.PlayOneShot2D("Take_Object");
            if(mPlayerController.LandingChemistryObject != null)
            {
                if (mPlayerController.LandingChemistryObject.gameObject == mFocusedObject.gameObject)
                {
                    return;
                }
            }
            mObjectSizeType = mFocusedObject;

            if (mObjectSizeType.TryGetComponent<Rigidbody>(out mTargetRb))
            {
                mTargetRb.useGravity = false;
                mTargetRb.isKinematic = false;
            }
            mTargetCollider = mObjectSizeType.GetComponent<Collider>();

            //取得した時の回転を取得
            mTakeObjectInfo.mRotationElapsedTime = 0f;
            mTakeObjectInfo.mRotation = mObjectSizeType.transform.rotation;
            mTakeObjectInfo.mStartRotation = mObjectSizeType.transform.rotation;

            //取得した時の距離を取得
            mTakeObjectInfo.mDistance = Math.Abs((mBaseTransform.position - mObjectSizeType.transform.position).magnitude);
            //取得フラグを有効に
            mIsTaked = true;

            // 掴んだので、フォーカス変数側は空にしておく（掴んでいる間はObjectCheckが走らないため）
            // ただしレイヤーは戻さない（掴んでいる間も光らせたい場合）
            mFocusedObject = null;

            mTakeObjectLineVFXController.SetEndTransform(mObjectSizeType.transform);
            mTakeObjectLineVFXController.gameObject.SetActive(true);

            mTakingObjectSoundSource = SoundManager.Instance.PlayLoopSE("Taking_Object", mBaseTransform.position, mBaseTransform);
        }
        public void UpdateTakeObject()
        {
            //オブジェクトが取得途中でなくなった時の処理
            if(mObjectSizeType == null)
            {
                mTakeObjectLineVFXController.SetEndTransform(mBaseTransform);
                mTakeObjectLineVFXController.gameObject.SetActive(false);
                mIsTaked = false;
                mObjectSizeType = null;
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
                //地面めり込み対策
                if(mTargetCollider != null)
                {
                    float objectHalfHeight = mTargetCollider.bounds.extents.y;

                    // ターゲット位置の少し上から真下にレイを飛ばして地面を探す
                    // Rayの開始位置を少し高く設定（坂道などに対応するため）
                    Vector3 rayOrigin = targetWorldPos;
                    rayOrigin.y += 2.0f;

                    if(Physics.Raycast(rayOrigin,Vector3.down,out RaycastHit hit,10f,mHitMask))
                    {
                        if(hit.collider.gameObject != mTargetCollider.gameObject)
                        {
                            // 「地面のY座標」+「オブジェクトの底までの高さ」
                            float minHeight = hit.point.y + objectHalfHeight;
                            // もしターゲット位置が、最低高さよりも低ければ持ち上げる
                            if (targetWorldPos.y < minHeight)
                            {
                                targetWorldPos.y = minHeight;
                            }
                        }
                    }
                }

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
            if (mObjectSizeType == null) return;

            if (mTargetRb != null)
            {
                mTargetRb.useGravity = true;
                mTargetRb.linearVelocity = Vector3.zero;
                mTargetRb = null;
            }
            SoundManager.Instance.StopLoopSE(mTakingObjectSoundSource);
            EffectManager.Instance.ObjectMaterialSelector.DeactivateEffect(mObjectSizeType.gameObject);
            mTakeObjectLineVFXController.SetEndTransform(mBaseTransform);
            mTakeObjectLineVFXController.gameObject.SetActive(false);
            mIsTaked = false;
            mObjectSizeType = null;
        }
    }
}
