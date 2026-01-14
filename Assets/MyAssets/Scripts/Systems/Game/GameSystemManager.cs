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

        [SerializeField]
        private EffectTable                 mEffectTable;
        public EffectTable                  EffectTable => mEffectTable;

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
            
            switch(sceneIndex)
            {
                case SceneList.Title:
                    SoundManager.Instance.PlayBGM(1);
                    break;
                case SceneList.Select:
                    SoundManager.Instance.PlayBGM(2);
                    break;
                default:
                    SoundManager.Instance.StopBGM();
                    break;
            }
        }
    }

}
