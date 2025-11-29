using UnityEngine;

namespace MyAssets
{
    public class ObjectSizeType : MonoBehaviour
    {
        public enum SizeType
        {
            None = -1,
            Small,
            Medium,
            Large,
        }

        [SerializeField]
        private SizeType mSize;
        public SizeType Size => mSize;
    }
}
