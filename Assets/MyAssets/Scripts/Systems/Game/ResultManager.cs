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

        private static bool mIsResulting = false;

        public static bool IsResulting
        {
            get
            {
                return mIsResulting;
            }
            set
            {
                mIsResulting = value;
            }
        }

        private static bool mIsPlayerDeath = false;

        public static bool IsPlayerDeath
        {
            get
            {
                return mIsPlayerDeath;
            }
            set
            {
                mIsPlayerDeath = value;
            }
        }

        public void Initilaize()
        {
            mDamageTaken = 0;
            mIsResulting = false;
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
