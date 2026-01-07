using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    public class ResultManager : MonoBehaviour
    {
        private static float mGameTime = 3600.0f;
        
        private static UpTimer mGameTimer = new UpTimer();
        public static UpTimer GameTimer => mGameTimer;

        private static int mDamageTaken = 0;
        private static int mMaxDamageTaken = 999;
        public static int DamageTaken 
        {
            get 
            { 
                return mDamageTaken; 
            }
            set 
            {
                if(mDamageTaken < mMaxDamageTaken)
                {
                    mDamageTaken = value; 
                }
                else
                {
                    mDamageTaken = mMaxDamageTaken;
                }
            } 
        }

        public void Initilaize()
        {
            mDamageTaken = 0;
            if (SceneManager.GetActiveScene().buildIndex > 1 && mGameTimer != null)
            {
                mGameTimer.Start(mGameTime);
            }
        }

        private void Start()
        {
            Initilaize();
        }

        private void Update()
        {
            if(mGameTimer != null)
            {
                mGameTimer.Update(Time.deltaTime);
            }
        }
    }
}
