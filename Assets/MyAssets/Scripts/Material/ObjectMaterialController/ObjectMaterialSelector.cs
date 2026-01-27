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
            for(int i = 0; i < target.transform.childCount; i++)
            {
                if(target.transform.GetChild(i).gameObject.layer == originalLayer)
                {
                    target.transform.GetChild(i).gameObject.layer = effectLayer;
                }
            }
        }

        public void DeactivateEffect(GameObject target)
        {
            for (int i = 0; i < target.transform.childCount; i++)
            {
                if(target.layer == target.transform.GetChild(i).gameObject.layer)
                {
                    target.transform.GetChild(i).gameObject.layer = originalLayer;
                }
            }
            // 元のレイヤーに戻し、通常の描画
            target.layer = originalLayer;
        }
    }
}
