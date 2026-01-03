using UnityEngine;

namespace MyAssets
{
    public class DoorController : MonoBehaviour,IGimmickReceiver
    {
        [SerializeField] 
        private float           mSpeed = 2f;

        [SerializeField]
        private float           mOpenHeight = 4f;

        [SerializeField]
        private Vector3         mClosedPos;
        [SerializeField]
        private Vector3         mOpenPos;

        private Vector3         mTargetPos;

        [SerializeField]
        private bool            mIsOpening = false;

        private void Start()
        {
            mClosedPos = transform.position;
            mOpenPos = mClosedPos + Vector3.up * mOpenHeight;
            mTargetPos = mClosedPos;
        }

        // Update is called once per frame
        private void Update()
        {
            if(!mIsOpening) return;
            // 目標地点との距離を計算
            float distance = Vector3.Distance(transform.position, mTargetPos);

            if (distance > 0.01f)
            {
                // 移動処理
                transform.position = Vector3.Lerp(transform.position, mTargetPos, Time.deltaTime * mSpeed);
            }
            else
            {
                // 完全に目標値に合わせる（微差を埋める）
                transform.position = mTargetPos;
                // このスクリプトのUpdateを止める
                enabled = false;
                mIsOpening = false;
            }
        }

        public void OnActivate()
        {
            if (mIsOpening) return;
            mIsOpening = true;

            mTargetPos = mOpenPos;
            enabled = true;
        }

        public void OnDeactivate()
        {
            if (mIsOpening) return;
            mIsOpening = true;

            mTargetPos = mClosedPos;
            enabled = true;
        }
    }
}
