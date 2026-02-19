using UnityEngine;

namespace MyAssets
{
    //疑似化学エンジンのエレメントクラス
    //当たったオブジェクトにエレメントタイプを渡す
    public class ChemistryElement : MonoBehaviour
    {
        [SerializeField]
        private ElementType     mType;
        public ElementType      Type => mType;

        //エレメント用のループ再生サウンド
        private AudioSource mElementSoundSource;

        private void Start()
        {
            switch(mType)
            {
                case ElementType.Fire:
                    SoundManager.Instance.PlayOneShot3D("Object_Fire", transform.position, transform, true,true);
                    break;
            }
        }
        /*
        private void Update()
        {
            if(mElementSoundSource == null)
            {
                mElementSoundSource = SoundManager.Instance.PlayLoopSE("Object_Fire", transform.position, transform);
            }
        }

        private void OnDisable()
        {
            if(mElementSoundSource != null)
            {
                if(SoundManager.Instance != null)
                {
                    SoundManager.Instance.StopLoopSE(mElementSoundSource);
                }
            }
        }
         */
    }
}
