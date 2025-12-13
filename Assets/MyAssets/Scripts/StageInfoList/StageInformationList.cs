using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public struct StageInformation
    {
        public int          gIndex;
        public SceneList    gSceneList;
        public string       gStageName;
        [TextArea(3, 10)]
        public string       gStageExplanation;
        public int          gDifficultyLevel;
        public Sprite       gStageSprite;
    }

    [CreateAssetMenu(menuName = "Stage/StageInformationList")]
    public class StageInformationList : ScriptableObject
    {
        [SerializeField]
        private List<StageInformation> mStageInformations = new List<StageInformation>();
        public List<StageInformation> StageInformations => mStageInformations;
    }
}
