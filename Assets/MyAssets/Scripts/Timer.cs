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
        private float current = 0;

        public float Current => current;
        //カウント方式のフラグ
        private bool up = false;
        //カウント方式の設定
        public void SetCountUp(bool u)
        {
            up = u;
        }
        //カウントのスタート
        public void Start(float time)
        {
            current = time;
        }

        public void Start_NoReStart(float time)
        {
            if (current > 0) { return; }
            current = time;
        }
        //カウントの更新
        public void Update(float time)
        {
            if (up)
            {
                current += time;
            }
            else
            {
                if (current <= 0) { return; }
                current -= time;
                if (current <= 0)
                {
                    current = 0;
                    End();
                }
            }
        }
        //カウントの終了
        public void End()
        {
            current = 0;
            OnEnd?.Invoke();
            OnceEnd?.Invoke();
            OnceEnd = null;
        }
        //カウントが終わっているか
        public bool IsEnd() { return current <= 0; }
        //カウントを分に変換
        public int GetMinutes()
        {
            return (int)current / 60;
        }
        //カウントを秒に変換
        public int GetSecond()
        {
            return (int)current % 60;
        }
    }
}
