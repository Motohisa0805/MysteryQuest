using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [CreateAssetMenu(menuName = "Chemistry/Reaction Table")]
    public class ChemistryTable : ScriptableObject
    {
        [Header("化学反応リスト")]
        public List<ReactionRule> gRules = new List<ReactionRule>();

        [Header("エレメント同士の反応リスト")]
        public List<ElementToElementReactionRule> gElementToElementRules = new List<ElementToElementReactionRule>();

        //検索を高速化するための辞書(実行時に構築)
        private Dictionary<(MaterialType, ElementType), MaterialToElementReactionResult> g_lookupCache;

        private Dictionary<(ElementType, ElementType), ElementToElementReactionResult> g_elementToElementLookupCache;

        public void Initialize()
        {
            //辞書の構築
            //同じキーが複数存在する可能性があるため、最初のルールのみを追加する
            //マテリアルとエレメントの組み合わせで反応結果を検索するための辞書
            g_lookupCache = new Dictionary<(MaterialType, ElementType), MaterialToElementReactionResult>();

            foreach(var rule in gRules)
            {
                var key = (rule.gTargetMaterial, rule.gStimulusElement);
                if(!g_lookupCache.ContainsKey(key))
                {
                    g_lookupCache.Add(key, rule.gResult);
                }
            }
            //エレメント同士の組み合わせで反応結果を検索するための辞書
            g_elementToElementLookupCache = new Dictionary<(ElementType, ElementType), ElementToElementReactionResult>();
            foreach(var rule in gElementToElementRules)
            {
                var key = (rule.gTargetElement, rule.gStimulusElement);
                if(!g_elementToElementLookupCache.ContainsKey(key))
                {
                    g_elementToElementLookupCache.Add(key, rule.gResult);
                }
            }
        }

        //検索機能
        public bool TryGetReaction(MaterialType mat,ElementType elem,out MaterialToElementReactionResult result)
        {
            if (g_lookupCache == null) Initialize();

            return g_lookupCache.TryGetValue((mat,elem),out result);
        }

        public bool TryGetElementToElementReaction(ElementType target, ElementType stimulus, out ElementToElementReactionResult result)
        {
            if (g_elementToElementLookupCache == null) Initialize();
            return g_elementToElementLookupCache.TryGetValue((target, stimulus), out result);
        }
    }
}
