using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

namespace MyAssets
{
    // イベント用ギミック：エレベーター制御
    //最初と最後に使うエレベーター
    [RequireComponent(typeof(Rigidbody))]
    public class EventElevatorController : EventPoint
    {

        private bool                mIsMoving = false;

        [SerializeField]
        private Vector3             mGoalPos = Vector3.zero;
        // 移動開始時の座標
        [SerializeField]
        private Vector3             mStartPos;   
        // 何秒かけて移動するか
        [SerializeField] 
        private float               mDuration = 3.0f; 
        // 移動開始からの時間
        private float               mTimer = 0.0f; 

        private MoveObjectCollider  mElevator;

        private Rigidbody           mRigidbody;

        //True:ステージより下に降下、fale：ステージに降下
        [SerializeField]
        private bool mIsFallOrDown = false;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            if (mRigidbody == null)
            {
                Debug.LogError("EventElevatorController: Rigidbody component is missing.");
            }
            mElevator = GetComponent<MoveObjectCollider>();
            if (mElevator == null)
            {
                Debug.LogError("EventElevatorController: Elevator Transform is not assigned.");
            }
            else
            {
                mStartPos = transform.position;
                if(mIsFallOrDown)
                {
                    mGoalPos = mStartPos;
                    mGoalPos.y = mStartPos.y - 3f;
                }
                else
                {
                    mGoalPos = transform.parent.position;
                    mGoalPos.y = mGoalPos.y + 0.75f;
                }
                mTimer = 0.0f;

            }
        }

        public override async Task SetConfig()
        {
            var utcs = new UniTaskCompletionSource();
            mOnComplete = () =>
            {
                utcs.TrySetResult();
            };
            mTimer = 0.0f;
            mIsMoving = true;
            enabled = true;
            mElevator.EnableColliders();
            await utcs.Task;
        }

        private void FixedUpdate()
        {
            if (!mIsMoving) return;

            // 1. 時間を進める
            mTimer += Time.deltaTime;

            // 2. 移動の進捗率（0.0 〜 1.0）を計算
            float t = Mathf.Clamp01(mTimer / mDuration);

            // 3. SmoothStepで「動き出し」と「終わり」を滑らかにする
            // 第3引数に 0~1 の進捗を入れると、滑らかな 0~1 が返ってきます
            float smoothedT = Mathf.SmoothStep(0.0f, 1.0f, t);

            // 4. 【重要】「現在の位置」ではなく「開始位置」から計算する
            // これにより、毎フレームの移動量が安定し、プレイヤーが吹っ飛ぶのを防ぎます
            Vector3 nextPos = Vector3.Lerp(mStartPos, mGoalPos, smoothedT);
            mRigidbody.MovePosition(nextPos);

            // 5. 終了判定（進捗が 1.0 になったら終了）
            if (t >= 1.0f)
            {
                mRigidbody.MovePosition(mGoalPos); // 最後にピタッと合わせる
                mIsMoving = false;
                mElevator.DisableColliders();
                mOnComplete?.Invoke();
                enabled = false;
            }
        }
    }
}
