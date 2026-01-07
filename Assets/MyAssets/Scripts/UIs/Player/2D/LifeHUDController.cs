using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{

    // ライフHUDコントローラークラス
    // プレイヤーのライフHUDの管理を行うクラス
    public class LifeHUDController : MonoBehaviour
    {
        [SerializeField]
        private GameObject mLifeIconPrefab;

        [SerializeField]
        private List<Transform> mNoLifeIcons = new List<Transform>();
        private List<Transform> mLifeIcons = new List<Transform>();

        [SerializeField]
        private Sprite mLife_FULL;
        [SerializeField]
        private Sprite mLife_THREE_QUARTERS;
        [SerializeField]
        private Sprite mLife_HALF;
        [SerializeField]
        private Sprite mLife_QUARTER;

        [SerializeField]
        private Sprite mNoLife;

        private int mLifeCount;

        private float mDisplayedHP;
        private Coroutine mUpdateCoroutine;

        //ライフHUD初期化処理
        private void InitilaizeAddLife()
        {
            Transform heart = null;
            Image heartimage = null;
            int count = (int)PlayerStatusManager.Instance.PlayerStatusData.MaxHP / 120;
            mLifeCount = count;
            for (int i = 0; i < count; i++)
            {
                //ハートオブジェクトを生成
                heart = Instantiate(mLifeIconPrefab, transform).transform;
                mNoLifeIcons.Add(heart);
                //オブジェクトからImageクラスをアタッチ
                heartimage = heart.AddComponent<Image>();
                //Imageクラスがあれば
                if (heartimage != null)
                {
                    heartimage.sprite = mNoLife;
                }
                RectTransform rect = heart.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(50, 50);
                rect.anchoredPosition = new Vector2(50 * i, 0);

                //ハートオブジェクトを生成
                heart = Instantiate(mLifeIconPrefab, transform).transform;
                mLifeIcons.Add(heart);
                //オブジェクトからImageクラスをアタッチ
                heartimage = heart.AddComponent<Image>();
                //Imageクラスがあれば
                if (heartimage != null)
                {
                    heartimage.sprite = mLife_FULL;
                }
                rect = heart.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(50, 50);
                rect.anchoredPosition = new Vector2(50 * i, 0);
            }
        }

        private void Start()
        {
            PlayerUIManager.Instance.LifeHUDController = this;
            // 初期化時に現在のHPを同期
            mDisplayedHP = PlayerStatusManager.Instance.PlayerStatusData.CurrentHP;
            InitilaizeAddLife();
        }

        //ライフHUD更新処理
        public void UpdateLifeHUD()
        {
            if(mUpdateCoroutine == null) { return; }
            // 既に動いていたら止めて、最新の目標値へ向けて再スタート
            if (mUpdateCoroutine != null) StopCoroutine(mUpdateCoroutine);
            mUpdateCoroutine = StartCoroutine(AnimateLifeUpdate());
        }

        private IEnumerator AnimateLifeUpdate()
        {
            float targetHP = PlayerStatusManager.Instance.PlayerStatusData.CurrentHP;
            float animationSpeed = 240; // 1秒間に変化するHP量

            // 表示HPが目標HPに十分近づくまでループ
            while (!Mathf.Approximately(mDisplayedHP, targetHP))
            {
                // 目標値に向かって一定速度で変化させる
                mDisplayedHP = Mathf.MoveTowards(mDisplayedHP, targetHP, animationSpeed * Time.deltaTime);

                // 変化した表示HPに基づいてスプライトを更新
                RefreshIcons(mDisplayedHP);

                yield return null; // 1フレーム待機
            }

            mUpdateCoroutine = null;
        }
        // スプライトを実際に切り替える処理（中身は元のUpdateLifeHUDを汎用化したもの）
        private void RefreshIcons(float hpToDisplay)
        {
            float tempHP = hpToDisplay;
            for (int i = 0; i < mLifeCount; i++)
            {
                Image heartimage = mLifeIcons[i].GetComponent<Image>();
                if (tempHP >= 120)
                {
                    heartimage.sprite = mLife_FULL;
                    tempHP -= 120;
                }
                else if (tempHP >= 90) 
                {
                    heartimage.sprite = mLife_THREE_QUARTERS;
                    // 0にするのはその後のハートを空にするため
                    tempHP = 0; 
                }
                else if (tempHP >= 60) 
                {
                    heartimage.sprite = mLife_HALF; 
                    tempHP = 0; 
                }
                else if (tempHP >= 30) 
                {
                    heartimage.sprite = mLife_QUARTER;
                    tempHP = 0; 
                }
                else 
                {
                    heartimage.sprite = mNoLife;
                    tempHP = 0; 
                }
            }
        }
    }
}
