using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyAssets
{
    public class BlackoutController : MonoBehaviour
    {
        // インスペクターから設定できるようにImageコンポーネントを公開
        [SerializeField]
        private Image mPanel;

        public Image Panel 
        {
            get 
            { 
                if (mPanel == null)
                {
                    mPanel = GetComponent<Image>();
                }
                return mPanel; 
            } 
        }

        // ブラックアウトにかける時間（秒）
        [SerializeField]
        public float mFadeDuration = 0.5f;


        private void Awake()
        {
            if (mPanel == null)
            {
                mPanel = GetComponent<Image>(); 
            }
        }
        /// <summary>
        /// 画面を完全にブラックアウトさせる処理を開始します。（フェードアウト）
        /// </summary>
        public void StartBlackout(int sceneIndex = -1)
        {
            // コルーチンをスタート
            StartCoroutine(FadeAlpha(0f, 1f, mFadeDuration, sceneIndex));
        }

        /// <summary>
        /// ブラックアウト状態から画面を元に戻す処理を開始します。（フェードイン）
        /// </summary>
        public void StartFadeIn(int sceneIndex = -1)
        {
            // コルーチンをスタート
            StartCoroutine(FadeAlpha(1f, 0f, mFadeDuration, sceneIndex));
        }

        /// <summary>
        /// 実際のアルファ値（透明度）を変更するコルーチン
        /// </summary>
        /// <param name="startAlpha">開始時のアルファ値 (0f: 透明, 1f: 不透明)</param>
        /// <param name="endAlpha">終了時のアルファ値</param>
        /// <param name="duration">処理にかける時間（秒）</param>
        private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration,int sceneIndex = -1)
        {
            // 経過時間を記録する変数
            float timeElapsed = 0f;

            // BlackoutPanelの色を取得
            Color panelColor = mPanel.color;

            // コルーチン本体：timeElapsedがdurationに達するまでループ
            while (timeElapsed < duration)
            {
                // 経過時間を加算（Time.deltaTimeは前フレームからの経過時間）
                timeElapsed += Time.unscaledDeltaTime;

                // 経過時間に基づき、現在のアルファ値を計算（Lerp関数を使用）
                // timeElapsed / duration で 0.0 から 1.0 までの割合を求める
                float alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);

                // Panelの色に新しいアルファ値を適用
                panelColor.a = alpha;
                mPanel.color = panelColor;

                // 次のフレームまで待機
                yield return null;
            }

            // 処理が完了したら、最後に目標のアルファ値を確実に設定
            panelColor.a = endAlpha;
            mPanel.color = panelColor;

            if(sceneIndex != -1)
            {
                SceneManager.LoadScene(sceneIndex);
                if(Time.timeScale < 1.0f)
                {
                    Time.timeScale = 1.0f;
                }
            }
        }
    }
}
