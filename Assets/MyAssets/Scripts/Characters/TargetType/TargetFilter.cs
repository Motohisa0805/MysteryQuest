using UnityEngine;

namespace MyAssets
{
    //注目対象のオブジェクトにアタッチするクラス
    public class TargetFilter : MonoBehaviour
    {
        public enum Filter
        {
            Friend,
            Enemy,
        }
        [SerializeField]
        private Filter mFilter;

        public Filter TypeFilter => mFilter;
    }
}
