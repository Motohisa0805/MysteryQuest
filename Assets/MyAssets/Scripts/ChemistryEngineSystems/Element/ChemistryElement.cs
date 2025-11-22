using UnityEngine;

namespace MyAssets
{
    //疑似化学エンジンのエレメントクラス
    //当たったオブジェクトにエレメントタイプを渡す
    public class ChemistryElement : MonoBehaviour
    {
        [SerializeField]
        private ElementType mType;
        public ElementType Type => mType;
    }
}
