using System;
using UnityEngine;

namespace MyAssets
{
    //マテリアル
    public enum MaterialType
    {
        None,
        Organism,
        Wood,
        Iron,
        Stone,
    }

    //エレメント
    [Flags]
    public enum ElementType
    {
        None = 0,
        Fire = 1 << 0,
        Water = 1 << 1,
    }

    [Serializable]
    public struct ReactionResult
    {
        [Tooltip("相手に付与する属性")]
        public ElementType gElementToAdd;

        [Tooltip("相手から削除する属性")]
        public ElementType gElementToRemove;

        [Tooltip("発生させるエフェクトID")]
        public int mEffectID;
        [Tooltip("発生させるエフェクトLabel")]
        public string mEffectLabel;
    }
    [Serializable]
    public class ReactionRule
    {
        //デバッグ用
        public string           gRuleName;

        [Header("条件")]
        public MaterialType     gTargetMaterial;//ターゲットの素材
        public ElementType      gStimulusElement;//触れたエレメント

        [Header("結果")]
        public ReactionResult   gResult;
    }
}
