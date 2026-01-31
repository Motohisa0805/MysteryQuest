using UnityEngine;

namespace MyAssets
{
    [CreateAssetMenu(menuName = "Chemistry/MaterialSoundData")]
    public class MaterialSoundData : ScriptableObject
    {
        public MaterialType gMaterialType;
        public AudioClip[] gHardImpacts;
        public AudioClip[] gSoftImpacts;
        public AudioClip frictionLoop;    // 引きずり音（ループ）
        public float frictionPitchMod;    // 摩擦時のピッチ補正
    }
}
