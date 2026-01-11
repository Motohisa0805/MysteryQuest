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
        public float UseSP;
        public float RecoverySP;
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

        private bool mIsStaminaCoolDown;
        public bool IsStaminaCoolDown => mIsStaminaCoolDown;
        public bool IsNoSprit => mPlayerStatus.CurrentSP <= 0 && mIsStaminaCoolDown;
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

        public void ChangeSP(bool input)
        {
            if(!mIsStaminaCoolDown)
            {
                if (input)
                {
                    if (mPlayerStatus.CurrentSP > 0)
                    {
                        mPlayerStatus.CurrentSP -= mPlayerStatus.UseSP * Time.deltaTime;
                    }
                }
                else
                {
                    if (mPlayerStatus.CurrentSP < mPlayerStatus.MaxSP)
                    {
                        mPlayerStatus.CurrentSP += mPlayerStatus.RecoverySP * Time.deltaTime;
                    }
                }
                if(mPlayerStatus.CurrentSP <= 0)
                {
                    mPlayerStatus.CurrentSP = 0;
                    mIsStaminaCoolDown = true;
                }
            }
            else
            {
                if (mPlayerStatus.CurrentSP < mPlayerStatus.MaxSP)
                {
                    mPlayerStatus.CurrentSP += mPlayerStatus.RecoverySP * Time.deltaTime;
                }
                if(mPlayerStatus.CurrentSP >= mPlayerStatus.MaxSP)
                {
                    mPlayerStatus.CurrentSP = mPlayerStatus.MaxSP;
                    mIsStaminaCoolDown = false;
                }
            }

        }

        public void RecoverySP(bool input)
        {
            if (!mIsStaminaCoolDown)
            {
                if (!input)
                {
                    if (mPlayerStatus.CurrentSP < mPlayerStatus.MaxSP)
                    {
                        mPlayerStatus.CurrentSP += mPlayerStatus.RecoverySP * Time.deltaTime;
                    }
                }
                if (mPlayerStatus.CurrentSP <= 0)
                {
                    mPlayerStatus.CurrentSP = 0;
                    mIsStaminaCoolDown = true;
                }
            }
            else
            {
                if (mPlayerStatus.CurrentSP < mPlayerStatus.MaxSP)
                {
                    mPlayerStatus.CurrentSP += mPlayerStatus.RecoverySP * Time.deltaTime;
                }
                if (mPlayerStatus.CurrentSP >= mPlayerStatus.MaxSP)
                {
                    mPlayerStatus.CurrentSP = mPlayerStatus.MaxSP;
                    mIsStaminaCoolDown = false;
                }
            }
        }
    }

}
