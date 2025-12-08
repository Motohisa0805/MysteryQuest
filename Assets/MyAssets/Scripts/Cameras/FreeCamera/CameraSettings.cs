using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public struct CameraSettingsTPS
    {
        [SerializeField]
        private Vector3 mOffset; //注視対象からのオフセット
        public Vector3 Offset => mOffset;
        [SerializeField]
        private float mDistance; //注視対象からの距離
        public float Distance => mDistance;
        [SerializeField]
        private float mSensitivity; //感度
        public float Sensitivity => mSensitivity;
        [SerializeField]
        private float mMinAngle; //最小仰角
        public float MinAngle => mMinAngle;
        [SerializeField]
        private float mMaxAngle; //最大仰角
        public float MaxAngle => mMaxAngle;
        [SerializeField]
        private float mSmoothTime; //カメラの追従を滑らかにする時間
        public float SmoothTime => mSmoothTime;
        [SerializeField]
        private LayerMask mLayerMask; //カメラの当たり判定に使用するレイヤーマスク
        public LayerMask LayerMask => mLayerMask;
    }
}