using System;

namespace MyAssets
{
    [System.Serializable]
    public class Timer
    {
        //一度きりのアクション
        public event Action OnceEnd;
        //何度でも使えるアクション
        public event Action OnEnd;
        //現在のカウント
        private float mCurrent = 0;
        public float Current => mCurrent;
        //カウント時の最大値
        private float mMax = 0;
        public float Max => mMax;

        //カウントのスタート
        public void Start(float time)
        {
            mCurrent = time;
            mMax = time;
        }
        //カウントの更新
        public void Update(float time)
        {
            if (mCurrent <= 0) { return; }
            mCurrent -= time;
            if (mCurrent <= 0)
            {
                mCurrent = 0;
                End();
            }
        }
        //カウントの終了
        public void End()
        {
            mCurrent = 0;
            OnEnd?.Invoke();
            OnceEnd?.Invoke();
            OnceEnd = null;
        }
        //カウントが終わっているか
        public bool IsEnd() { return mCurrent <= 0; }
        //カウントを分に変換
        public int GetMinutes()
        {
            return (int)mCurrent / 60;
        }
        //カウントを秒に変換
        public int GetSecond()
        {
            return (int)mCurrent % 60;
        }
    }

    [System.Serializable]
    public class UpTimer
    {
        //一度きりのアクション
        public event Action OnceEnd;
        //何度でも使えるアクション
        public event Action OnEnd;
        //現在のカウント
        private float mCurrent = 0;
        public float Current => mCurrent;
        //カウント時の最大値
        private float mMax = 0;
        public float Max => mMax;

        public void Start(float time)
        {
            mCurrent = 0;
            mMax = time;
        }
        public float GetNormalize()
        {
            if (mMax == 0) { return 1.0f; }
            return mCurrent / mMax;
        }
        //カウントの更新
        public void Update(float time)
        {
            if(mCurrent >= mMax) { return; }
            mCurrent += time;
            if (mCurrent >= mMax)
            {
                mCurrent = 0;
                mMax = 0;
                End();
            }
        }
        //カウントの終了
        public void End()
        {
            mCurrent = 0;
            OnEnd?.Invoke();
            OnceEnd?.Invoke();
            OnceEnd = null;
        }
        //カウントが終わっているか
        public bool IsEnd() { return mCurrent >= mMax; }
        //カウントを分に変換
        public int GetMinutes()
        {
            return (int)mCurrent / 60;
        }
        //カウントを秒に変換
        public int GetSecond()
        {
            return (int)mCurrent % 60;
        }
    }
}
