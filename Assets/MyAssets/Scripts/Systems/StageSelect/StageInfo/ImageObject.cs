using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class ImageObject : MonoBehaviour
    {
        private Image mImage;
        public Image Image => mImage;

        private void Awake()
        {
            mImage = GetComponent<Image>();
        }
    }
}
