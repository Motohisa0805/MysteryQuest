using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public struct CameraSettingsTPS
    {
        [SerializeField]
        private Vector3     mOffset; //’Ž‹‘ÎÛ‚©‚ç‚ÌƒIƒtƒZƒbƒg
        public Vector3      Offset => mOffset;
        [SerializeField]
        private float       mDistance; //’Ž‹‘ÎÛ‚©‚ç‚Ì‹——£
        public float        Distance => mDistance;
        [SerializeField]
        private float       mMinAngle; //Å¬‹ÂŠp
        public float        MinAngle => mMinAngle;
        [SerializeField]
        private float       mMaxAngle; //Å‘å‹ÂŠp
        public float        MaxAngle => mMaxAngle;
        [SerializeField]
        private float       mSmoothTime; //ƒJƒƒ‰‚Ì’Ç]‚ðŠŠ‚ç‚©‚É‚·‚éŽžŠÔ
        public float        SmoothTime => mSmoothTime;
    }
}