using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class ObjectMaterialSelector
    {
        private int originalLayer;
        private int effectLayer;

        public void Setup()
        {
            effectLayer = LayerMask.NameToLayer("Interactable");
        }

        public void ActivateEffect(GameObject target)
        {
            // レイヤーを変えて、Renderer Featureが反応してマテリアルを上書き
            originalLayer = target.layer;
            target.layer = effectLayer;
        }

        public void DeactivateEffect(GameObject target)
        {
            // 元のレイヤーに戻し、通常の描画
            target.layer = originalLayer;
        }
    }
}
