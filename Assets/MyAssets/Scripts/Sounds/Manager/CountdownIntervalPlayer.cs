using System.Collections;
using UnityEngine;

namespace MyAssets
{
    public class CountdownIntervalPlayer : MonoBehaviour
    {
        private static CountdownIntervalPlayer instance;
        public static CountdownIntervalPlayer Instance => instance;

        [SerializeField]
        private Transform mPlayerTransform;

        [Header("時間設定")]
        // 合計時間
        [SerializeField] 
        private float mTotalDuration = 10f;   
        // 開始時の間隔（秒）
        [SerializeField] 
        private float mStartInterval = 0.4f;  
        // 終了直前の間隔（秒）
        [SerializeField] 
        private float mEndInterval = 0.1f;

        [SerializeField]
        private int mMaxRhythmCount = 3; // 最大リズムカウント

        private int mCurrentRhythmCount = 0; // 現在のリズムカウント

        [Header("演出設定")]
        [SerializeField]
        private float mAccelCurve = 2.0f; // 加速度カーブ

        private bool mIsRunning = false;
        private float mElapsedTime = 0f;

        float mCurrentPitch = 1.0f;
        float mPitchStep = 0.1f; // 1回ごとに0.1ずつ下げる

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //カウントダウン開始
        public void StartCountdown(float maxCount)
        {
            // 既に実行中なら無視
            if (mIsRunning) return;
            mTotalDuration = maxCount;
            mElapsedTime = 0f;
            mIsRunning = true;
            StartCoroutine(CountdownRoutine());
        }

        private IEnumerator CountdownRoutine()
        {
            while(mElapsedTime < mTotalDuration)
            {
                SoundManager.Instance.PlayOneShot2D("CountDown", false,-1, mCurrentPitch);
                mCurrentPitch -= mPitchStep;

                // 最低ピッチを下回らないように制限
                mCurrentPitch = Mathf.Max(mCurrentPitch, 0.5f);

                // 経過時間に基づいて次の間隔を計算
                float progress = mElapsedTime / mTotalDuration;

                // 間隔を補間（加速度的に）
                float curvedProgress = Mathf.Pow(progress, mAccelCurve);

                // 次の間隔を計算
                float currentInterval = Mathf.Lerp(mStartInterval, mEndInterval, curvedProgress);

                mCurrentRhythmCount++;
                if (mCurrentRhythmCount >= mMaxRhythmCount)
                {
                    // リセット時にピッチを元に戻す
                    mCurrentPitch = 1.0f; 
                    // リズムカウントが最大に達したらリセット
                    mCurrentRhythmCount = 0;
                    // ここで特別なリズム効果を追加することも可能
                    currentInterval += Mathf.Lerp(mStartInterval, mEndInterval, curvedProgress); 
                }

                // 次の再生まで待機
                mElapsedTime += currentInterval;
                yield return new WaitForSeconds(currentInterval);
            }

            mIsRunning = false;
        }

        public void StopCountdown()
        {
            StopCoroutine(CountdownRoutine());
            mIsRunning = false;
        }

    }
}
