using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class MovementCompensator
    {
        [SerializeField]
        private float mStepSmooth;
        public float StepSmooth => mStepSmooth;
        [Header("Step Settings")]
        [SerializeField,Tooltip("乗り越えられる最大の段差の高さ")]
        private float mMaxStepHeight;
        public float MaxStepHeight => mMaxStepHeight;
        [SerializeField, Tooltip("段差検知を開始する最低の高さ(地面の凸凹を無視するため)")]
        private float mMinStepHeight;
        [SerializeField,Tooltip("前方への検知距離（キャラクターの半径より少し大きく）")]
        private float mCheckDistance;
        [SerializeField, Tooltip("乗り越え可能なスロープの角度制限（これより急なら壁とみなして登る）")]
        private float mMaxSlopeAngle;
        [SerializeField]
        private LayerMask mGroundMask;

        private Transform mThisTransform;
        private PlayableChracterController mController;
        
        [SerializeField]
        private Vector3 mStepGoalPosition;
        public Vector3 StepGoalPosition => mStepGoalPosition;
        [SerializeField]
        private Vector3 mStepStartPosition;
        public Vector3 StepStartPosition => mStepStartPosition;

        [SerializeField]
        private bool mIsClimbJumping;
        public bool IsClimbJumping => mIsClimbJumping;

        public void SetIsClimbJumping(bool v)
        {
            mIsClimbJumping = v;
        }

        public void Setup(Transform thisTransform)
        {
            mThisTransform = thisTransform;
            mController = mThisTransform.GetComponent<PlayableChracterController>();
            mIsClimbJumping = false;
        }

        public void ClearStepGoalPosition()
        {
            mStepGoalPosition = Vector3.zero;
            mStepStartPosition = Vector3.zero;
        }

        public void HandleStepClimbin()
        {
            //初期化
            mStepGoalPosition = Vector3.zero;
            mStepStartPosition = Vector3.zero;

            Vector3 forward = mThisTransform.forward;
            Vector3 position = mThisTransform.position;

            //下部レイキャスト
            Vector3 lowerRayOrigin = position + Vector3.up * mMinStepHeight;
            Ray lowerRay = new Ray(lowerRayOrigin, forward);

            RaycastHit hitLower;
            //距離判定
            if (Physics.Raycast(lowerRay, out hitLower, mCheckDistance, mGroundMask))
            {
                //ヒットオブジェクトがChemistryObjectか確認
                ChemistryObject obj = hitLower.collider.GetComponent<ChemistryObject>();
                //角度判定
                //壁の法線と上方向の角度を計算
                float angle = Vector3.Angle(hitLower.normal, Vector3.up);
                //もし角度がスロープ制限以下なら、それは「歩ける坂」
                if(angle < mMaxSlopeAngle || obj != null)
                {
                    return;
                }

                //上部レイキャスト
                Vector3 upperRayOrigin = position + Vector3.up * mMaxStepHeight;
                Ray upperRay = new Ray(upperRayOrigin, forward);

                //上部が「何にも当たらない」なら
                if(!Physics.Raycast(upperRay, mCheckDistance, mGroundMask))
                {
                    // 下方向レイキャスト（着地点探索）：段差の上に床はあるか？
                    // 「前方に進んだ位置」の「高い場所」から「真下」へレイを撃ちます
                    Vector3 downRayOrigin = position + (forward * mCheckDistance) + (Vector3.up * mMaxStepHeight);
                    Ray downRay = new Ray(downRayOrigin, Vector3.down);

                    RaycastHit hitDown;
                    //maxStepHeight分の距離を調べて、地面に当たるか確認
                    if(Physics.Raycast(downRay,out hitDown,mMaxStepHeight,mGroundMask))
                    {
                        //ヒットした位置が、現在の足元の高さより高いか確認（低いなら穴かもしれない）
                        if (hitDown.point.y > position.y + mMinStepHeight)
                        {
                            mStepGoalPosition = hitDown.point;
                            if(!mController.Grounded && mController.FallTimer.IsEnd())
                            {
                                mIsClimbJumping = true;
                                mStepStartPosition = position;
                            }
                        }
                    }
                    // デバッグ用描画（着地点確認）
                    Debug.DrawRay(downRay.origin, Vector3.down * mMaxStepHeight, Color.green);
                }
                // デバッグ用描画
                Debug.DrawRay(upperRay.origin, upperRay.direction * mCheckDistance, Color.red);
            }
            // デバッグ用描画
            Debug.DrawRay(lowerRay.origin, lowerRay.direction * mCheckDistance, Color.blue);
        }
    }
}
