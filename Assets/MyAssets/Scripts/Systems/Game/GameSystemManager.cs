using UnityEngine;

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

            InputManager.Initialize();
        }

        private void Start()
        {
            InputManager.SetLockedMouseMode();
        }
    }

}
