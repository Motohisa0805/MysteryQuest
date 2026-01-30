using UnityEngine;

namespace MyAssets
{
    public class StickObject : MonoBehaviour
    {
        private float mBaseSizeRaito;

        private float mReductionRaito = 0.5f;


        public void SetReductionSize()
        {
            Vector3 scale = transform.localScale;
            scale *= mReductionRaito;
            transform.localScale = scale;
        }

        public void BaseSizeRaito()
        {
            Vector3 baseSize = transform.localScale;
            baseSize = Vector3.one;
            transform.localScale = baseSize;
        }
    }
}
