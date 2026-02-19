using UnityEngine;

namespace MyAssets
{
    //ステージギミックのドアコントローラー
    // ドアの開閉を制御する
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

        public void OnActivate(float count = 0)
        {
            if (mIsOpening) return;
            mIsOpening = true;
            SoundManager.Instance.PauseBGM();
            CountdownIntervalPlayer.Instance.StartCountdown(count);
            mTargetPos = mOpenPos;
            enabled = true;
        }

        public void OnDeactivate()
        {
            if (mIsOpening) return;
            mIsOpening = true;
            SoundManager.Instance.UnPauseStart();
            mTargetPos = mClosedPos;
            enabled = true;
        }

        private void PushPlayer(Transform playerTransform)
        {
            CapsuleCollider collider = playerTransform.GetComponent<CapsuleCollider>();
            float pushDistance = 0.25f;
            //コライダーを取得出来たら
            if (collider)
            {
                //半径を代入
                pushDistance = collider.radius;
            }

            Vector3 doorToPlayer = playerTransform.position - transform.position;

            float dotProduct = Vector3.Dot(transform.forward, doorToPlayer);

            Vector3 pushDirection;
            if (dotProduct >= 0)
            {
                pushDirection = transform.forward;
            }
            else
            {
                pushDirection = -transform.forward;
            }

            // 現在位置に加算して強制移動
            playerTransform.position += pushDirection * pushDistance;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (!mIsOpening) { return; }
            PlayableChracterController playableChracter = collision.gameObject.GetComponent<PlayableChracterController>();
            if (playableChracter != null)
            {
                PushPlayer(collision.transform);
            }
        }
    }
}
