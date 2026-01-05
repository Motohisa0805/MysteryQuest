using UnityEngine;

namespace MyAssets
{
    //剣についているエフェクトを管理するクラス
    //主にTrailRendererのON/OFFを管理する
    public class SwingEffectHandler : MonoBehaviour
    {
        private TrailRenderer keepSlachEffect;
        private void Start()
        {
            keepSlachEffect = GetComponentInChildren<TrailRenderer>();

            ActivateSlachEffect(false);
        }

        public void ActivateSlachEffect(bool activate)
        {
            if (keepSlachEffect == null) { return; }
            if (keepSlachEffect.enabled == activate) { return; }
            keepSlachEffect.enabled = activate;
        }
    }
}
