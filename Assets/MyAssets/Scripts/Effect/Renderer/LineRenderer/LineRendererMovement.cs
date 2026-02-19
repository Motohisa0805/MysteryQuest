using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public struct SpiralInfo
    {
        [SerializeField]
        private float   mOffset;
        public float    Offset => mOffset;
        [SerializeField]
        private float   mRadius;
        public float    Radius => mRadius;
        [SerializeField]
        private int     mPointsCount;
        public int      PointsCount => mPointsCount;
    }

    public class LineRendererMovement : MonoBehaviour
    {
        public enum LineType
        {
            Straight,
            Spiral,
        }

        private LineRenderer    mLineRenderer;

        [SerializeField]
        private LineType        mLineType;

        [SerializeField]
        private Transform       mStartTransform;
        public Transform        StartTransform { get { return mStartTransform; } set { mStartTransform = value; } }
        [SerializeField]
        private Vector3         mStartPosOffset;
        [SerializeField]
        private Transform       mEndTransform;
        public Transform        EndTransform { get { return mEndTransform; } set { mEndTransform = value; } }
        [SerializeField]
        private Vector3         mEndPosOffset;

        [SerializeField]
        private SpiralInfo      mSpiralLineInfo;

        private void Awake()
        {
            mLineRenderer = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            if(mLineType == LineType.Straight)
            {
                UpdateStraight();
            }
            else if(mLineType == LineType.Spiral)
            {
                UpdateSpiral();
            }
        }

        private void UpdateStraight()
        {
            if (!mStartTransform || !mEndTransform) { return; }
            int pointsCount = 2;
            pointsCount = mLineRenderer.positionCount;
            mLineRenderer.SetPosition(0, mStartTransform.position + mStartPosOffset);
            mLineRenderer.SetPosition(1, mEndTransform.position + mEndPosOffset);
        }

        private void UpdateSpiral()
        {
            if (!mStartTransform || !mEndTransform) { return; }
            mLineRenderer.positionCount = mSpiralLineInfo.PointsCount;

            for(int i = 0; i < mSpiralLineInfo.PointsCount; i++)
            {
                float t = i / (float)(mSpiralLineInfo.PointsCount - 1);
                // 始点と終点の間を線形補間
                Vector3 pos = Vector3.Lerp(mStartTransform.position + mStartPosOffset, mEndTransform.position + mEndPosOffset, t);

                // 進行方向に対して垂直な円の動きを加える
                float angle = t * Mathf.PI * 4f + Time.time * 5f + mSpiralLineInfo.Offset;

                pos += mStartTransform.up * Mathf.Sin(angle) * mSpiralLineInfo.Radius;
                pos += mStartTransform.right * Mathf.Cos(angle) * mSpiralLineInfo.Radius;

                mLineRenderer.SetPosition(i, pos);
            }
        }
    }
}
