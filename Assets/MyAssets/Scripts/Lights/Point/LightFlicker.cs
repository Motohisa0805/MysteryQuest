using UnityEngine;

namespace MyAssets
{
    public class LightFlicker : MonoBehaviour
    {
        private Light fireLight;
        private void Start() => fireLight = GetComponent<Light>();

        private void Update()
        {
            // 0.8〜1.2の間でランダムに明るさを変える
            fireLight.intensity = Random.Range(0.8f, 1.2f);
        }
    }
}
