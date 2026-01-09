using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public struct PlayerStatus
    {
        public float MaxHP;
        public float CurrentHP;
        public float MaxSP;
        public float CurrentSP;
        public float Attack;
        public float Defense;
    }

    public class PlayerStatusManager : MonoBehaviour
    {
        private static PlayerStatusManager instance;
        public static PlayerStatusManager Instance => instance;

        [SerializeField]
        private PlayerStatus mPlayerStatus;
        public PlayerStatus   PlayerStatusData => mPlayerStatus;

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

        public void ChangeHP(float amount)
        {
            mPlayerStatus.CurrentHP += amount;
            if (mPlayerStatus.CurrentHP > mPlayerStatus.MaxHP)
            {
                mPlayerStatus.CurrentHP = mPlayerStatus.MaxHP;
            }
            else if (mPlayerStatus.CurrentHP < 0)
            {
                mPlayerStatus.CurrentHP = 0;
            }
            if(amount < 0)
            {
                ResultManager.DamageTaken += Mathf.Abs((int)amount);
            }
        }
    }

}
