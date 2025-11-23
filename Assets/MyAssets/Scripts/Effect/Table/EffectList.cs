using System;
using UnityEngine;

namespace MyAssets
{
    [Flags]
    public enum EffectType
    {
        None = 0,
        BoxFire = 1 << 0,
        Dummy = 1 << 1,
    }
    [Serializable]
    public class EffectList
    {
        [Tooltip("エフェクト番号")]
        public EffectType gEffectType;
        [Tooltip("エフェクト本体")]
        public ParticleSystem gParticleSystem;
    }
}
