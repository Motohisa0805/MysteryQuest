using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class EffectElement
    {
        [Tooltip("エフェクト番号")]
        [SerializeField]
        private int mID;
        public int ID => mID;
        [Tooltip("エフェクト本体")]
        [SerializeField]
        private ParticleSystem mParticleSystem;
        public ParticleSystem ParticleSystem => mParticleSystem;
        [SerializeField]
        private float mSizeScale;
        public float SizeScale => mSizeScale;

        [SerializeField]
        private int mInitObjectCount;
        public int InitObjectCount => mInitObjectCount;

        [SerializeField]
        private int mMaxSize;
        public int MaxSize => mMaxSize;

        [SerializeField]
        private int mDefaultCapacity;
        public int DefaultCapacity => mDefaultCapacity;
    }
}
