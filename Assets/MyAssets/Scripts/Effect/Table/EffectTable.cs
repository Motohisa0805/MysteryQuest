using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace MyAssets
{

    [CreateAssetMenu(menuName = "Effect/EffectTable")]
    public class EffectTable : ScriptableObject
    {
        [Header("エフェクトリスト")]
        public List<EffectList> gEffects = new List<EffectList>();

        //検索を高速化するための辞書(実行時に構築)
        private Dictionary<EffectType, ParticleSystem> g_lookupCache;

        public void Initialize()
        {
            g_lookupCache = new Dictionary<EffectType, ParticleSystem>();
            foreach (var effect in gEffects)
            {
                var key = effect.gEffectType;
                if (!g_lookupCache.ContainsKey(key))
                {
                    g_lookupCache.Add(key, effect.gParticleSystem);
                }
            }
        }

        //検索機能
        public bool TryGetReaction(EffectType effect, out ParticleSystem result)
        {
            if (g_lookupCache == null) Initialize();

            return g_lookupCache.TryGetValue(effect, out result);
        }
    }
}
