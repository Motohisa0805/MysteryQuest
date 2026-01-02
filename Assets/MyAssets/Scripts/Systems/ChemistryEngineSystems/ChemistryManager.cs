using UnityEngine;

namespace MyAssets
{
    //疑似化学エンジンのマネージャースクリプトファイル
    public class ChemistryManager : MonoBehaviour
    {
        private static ChemistryManager instance;
        public static ChemistryManager Instance => instance;


        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }


    }
}
