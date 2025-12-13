using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class StageCountController : MonoBehaviour
    {
        public enum TextType
        {
            eCurrent,
            eSlash,
            eMax
        }

        private Text[] mText = new Text[0];

        private void Awake()
        {

            Text[] list = GetComponentsInChildren<Text>();
            mText = list;
        }
        //現在のページと最大のページをテキストに代入
        public void DrawText(StageSelectController stageSelect)
        {
            mText[(int)TextType.eCurrent].text = stageSelect.CurrentStageInformation.gIndex.ToString();
            mText[(int)TextType.eMax].text = stageSelect.StageInformationList.StageInformations.Count.ToString();
        }
    }
}
