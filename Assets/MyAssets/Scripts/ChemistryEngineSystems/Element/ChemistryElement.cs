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

        private void Start()
        {
            switch(mType)
            {
                case ElementType.Fire:
                    SoundManager.Instance.PlayOneShot3D(1009, transform.position, transform, true,true);
                    break;
            }
        }
    }
}
