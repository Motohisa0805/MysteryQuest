using UnityEngine;

namespace MyAssets
{
    public class HandTransform : MonoBehaviour
    {

        public float VectorSize() { return transform.position.magnitude; }
    }
}
