using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    //ゲーム開始時に呼ばれるゲームの初期化を行うクラス
    public class BootstrapManager : MonoBehaviour
    {
        [SerializeField]
        private GameSystemManager           mGameSystemManager;
        [SerializeField]
        private GameUserInterfaceManager    mUserInterfaceManager;
        [SerializeField]
        private ItemManager                 mItemManager;
        [SerializeField]
        private PlayerUIManager             mPlayerUIManager;
        [SerializeField]
        private SoundManager                mSoundManager;
        [SerializeField]
        private EffectManager               mEffectManager;

        private void Start()
        {
            if(mGameSystemManager != null)
            {
                Instantiate(mGameSystemManager);
            }
            if(mUserInterfaceManager != null)
            {
                Instantiate(mUserInterfaceManager);
            }
            if(mItemManager != null)
            {
                Instantiate(mItemManager);
            }
            if(mPlayerUIManager != null)
            {
                Instantiate(mPlayerUIManager);
            }
            if(mSoundManager != null)
            {
                Instantiate(mSoundManager);
            }
            if(mEffectManager != null)
            {
                Instantiate(mEffectManager);
            }
        }

        private void LateUpdate()
        {
            //タイトルに遷移
            SceneManager.LoadScene((int)SceneList.Title);
        }
    }
}

