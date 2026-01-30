using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [CreateAssetMenu(menuName = "Chemistry/Reaction Table")]
    public class ChemistryTable : ScriptableObject
    {
        [Header("化学反応リスト")]
        public List<ReactionRule> gRules = new List<ReactionRule>();

        //検索を高速化するための辞書(実行時に構築)
        private Dictionary<(MaterialType, ElementType), ReactionResult> g_lookupCache;

        public void Initialize()
        {
            g_lookupCache = new Dictionary<(MaterialType, ElementType), ReactionResult>();

            foreach(var rule in gRules)
            {
                var key = (rule.gTargetMaterial, rule.gStimulusElement);
                if(!g_lookupCache.ContainsKey(key))
                {
                    g_lookupCache.Add(key, rule.gResult);
                }
            }
        }

        //検索機能
        public bool TryGetReaction(MaterialType mat,ElementType elem,out ReactionResult result)
        {
            if (g_lookupCache == null) Initialize();

            return g_lookupCache.TryGetValue((mat,elem),out result);
        }
    }
}
