using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    // ゲームシステムマネージャークラス
    // ゲーム全体のシステムを管理するシングルトンクラス
    public class GameSystemManager : MonoBehaviour
    {
        private static GameSystemManager    instance;

        public static GameSystemManager     Instance => instance;

        // シングルトン等で管理されているテーブルを参照
        [SerializeField]
        private ChemistryTable              mChemistryTable;

        public ChemistryTable               ChemistryTable => mChemistryTable;

        private AudioSource                 mGameSoundEffect;


        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            // シーンが読み込まれたときに実行する関数を登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            // メモリリーク防止のため、登録を解除
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // シーン読み込み時に呼ばれる関数
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneList sceneIndex = (SceneList)scene.buildIndex;
            
            if(mGameSoundEffect != null)
            {
                SoundManager.Instance.StopLoopSE(mGameSoundEffect);
            }

            switch(sceneIndex)
            {
                case SceneList.Title:
                    SoundManager.Instance.PlayBGM("TitleBGM");
                    mGameSoundEffect = SoundManager.Instance.PlayLoopSE("Wind", transform.position, transform);
                    break;
                case SceneList.Select:
                    SoundManager.Instance.PlayBGM("SelectBGM");
                    break;
                default:
                    SoundManager.Instance.StopBGM();
                    mGameSoundEffect = SoundManager.Instance.PlayLoopSE("Wind", transform.position, transform);
                    break;
            }
        }
    }

}
