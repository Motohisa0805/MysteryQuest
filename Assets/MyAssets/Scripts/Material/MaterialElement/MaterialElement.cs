using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class MaterialElement
    {
        [Tooltip("エフェクト番号")]
        [SerializeField]
        private int         mID;
        public int          ID => mID;
        [Tooltip("エフェクト本体")]
        [SerializeField]
        private Material[]    mMaterials;
        public Material[]     Materials => mMaterials;
    }
}
