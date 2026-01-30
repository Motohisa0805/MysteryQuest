using System;
using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    [System.Serializable]
    public struct StageSelectData
    {
        [SerializeField]
        private Text mStageTitle;
        public Text StageTitle => mStageTitle;

        [SerializeField]
        private Text mStageExplanation;
        public Text StageExplanation => mStageExplanation;

        [SerializeField]
        private Image mStageImage;
        public Image StageImage => mStageImage;
        public void SetData(Text stageTitle, Text stageExplanation, Image stageImage)
        {
            mStageTitle = stageTitle;
            mStageExplanation = stageExplanation;
            mStageImage = stageImage;
        }
    }

    //ステージの選択を行うUIの管理を行うクラス
    public class StageSelectController : MonoBehaviour
    {
        //ステージ情報を全て持ったスクリプタブルオブジェクト
        [SerializeField]
        private StageInformationList    mStageInformationList;
        public StageInformationList     StageInformationList => mStageInformationList;
        //現在選択中のステージ情報
        private StageInformation        mCurrentStageInformation;
        public StageInformation         CurrentStageInformation => mCurrentStageInformation;
        //現在のステージ番号
        private int                     mCurrentStageIndex;

        //ステージ情報を代入する先をまとめて宣言した構造体
        [SerializeField]
        private StageSelectData         mStageDataObject;


        private StageCountController    mStageCounter;

        //ステージをロードするボタンを宣言
        [SerializeField]
        private LoadSceneButton         mLoadSceneButton;

        public void ChangeStageInformationPage(int index)
        {
            mCurrentStageIndex += index;
            //配列の数に収める
            mCurrentStageIndex = Math.Clamp(mCurrentStageIndex, 0, StageInformationList.StageInformations.Count - 1);
            mCurrentStageInformation = mStageInformationList.StageInformations[mCurrentStageIndex];
            SetData(CurrentStageInformation);
            mStageCounter.DrawText(this);
        }


        private void Awake()
        {
            mStageCounter = GetComponentInChildren<StageCountController>();

            mStageDataObject.SetData(
                GetComponentInChildren<TitleTextObject>().GetComponent<Text>(),
                GetComponentInChildren<ExplanationStringObject>().GetComponent<Text>(),
                GetComponentInChildren<ImageObject>().GetComponent<Image>()
                );

            mCurrentStageIndex = 0;

            //0番目のデータを参照(後々ゲームの進捗度で変更する)
            mCurrentStageInformation = mStageInformationList.StageInformations[mCurrentStageIndex];
        }
        //ステージのデータを代入
        private void SetData(StageInformation data)
        {
            mStageDataObject.StageTitle.text = data.gStageName;
            mStageDataObject.StageExplanation.text = data.gStageExplanation;
            mStageDataObject.StageImage.sprite = data.gStageSprite;
            if(mLoadSceneButton)
            {
                mLoadSceneButton.SetTag(data.gSceneList);
            }
        }

        private void Start()
        {
            SetData(CurrentStageInformation);
            mStageCounter.DrawText(this);
        }
    }
}
