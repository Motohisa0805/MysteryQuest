using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public float ThrowPower;
    }

    public class PlayerStatusManager : MonoBehaviour
    {
        private static PlayerStatusManager instance;
        public static PlayerStatusManager Instance => instance;

        [SerializeField]
        private PlayerStatus mPlayerStatus;
        [SerializeField]
        private PlayerStatus mKeepStatus;
        public PlayerStatus   PlayerStatusData => mKeepStatus;

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

        private void Start()
        {
            mKeepStatus.MaxHP = mPlayerStatus.MaxHP;
            mKeepStatus.CurrentHP = mPlayerStatus.CurrentHP;
            mKeepStatus.MaxSP = mPlayerStatus.MaxSP;
            mKeepStatus.CurrentSP = mPlayerStatus.CurrentSP;
            mKeepStatus.UseSP = mPlayerStatus.UseSP;
            mKeepStatus.RecoverySP = mPlayerStatus.RecoverySP;
            mKeepStatus.ThrowPower = mPlayerStatus.ThrowPower;
        }

        private void OnEnable()
        {
            // シーンがロードされた時のイベントに登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 破棄時にイベント解除（お作法）
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // シーンがロードされるたびに呼ばれる
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            mKeepStatus.MaxHP = mPlayerStatus.MaxHP;
            mKeepStatus.CurrentHP = mPlayerStatus.CurrentHP;
            mKeepStatus.MaxSP = mPlayerStatus.MaxSP;
            mKeepStatus.CurrentSP = mPlayerStatus.CurrentSP;
            mKeepStatus.UseSP = mPlayerStatus.UseSP;
            mKeepStatus.RecoverySP = mPlayerStatus.RecoverySP;
            mKeepStatus.ThrowPower = mPlayerStatus.ThrowPower;
        }

        public void ChangeHP(float amount)
        {
            mKeepStatus.CurrentHP += amount;
            if (mKeepStatus.CurrentHP > mKeepStatus.MaxHP)
            {
                mKeepStatus.CurrentHP = mKeepStatus.MaxHP;
            }
            else if (mKeepStatus.CurrentHP < 0)
            {
                mKeepStatus.CurrentHP = 0;
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
                    if (mKeepStatus.CurrentSP > 0)
                    {
                        mKeepStatus.CurrentSP -= mKeepStatus.UseSP * Time.deltaTime;
                    }
                }
                else
                {
                    if (mKeepStatus.CurrentSP < mKeepStatus.MaxSP)
                    {
                        mKeepStatus.CurrentSP += mKeepStatus.RecoverySP * Time.deltaTime;
                    }
                }
                if(mKeepStatus.CurrentSP <= 0)
                {
                    mKeepStatus.CurrentSP = 0;
                    mIsStaminaCoolDown = true;
                }
            }
            else
            {
                if (mKeepStatus.CurrentSP < mKeepStatus.MaxSP)
                {
                    mKeepStatus.CurrentSP += mKeepStatus.RecoverySP * Time.deltaTime;
                }
                if(mKeepStatus.CurrentSP >= mKeepStatus.MaxSP)
                {
                    mKeepStatus.CurrentSP = mKeepStatus.MaxSP;
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
                    if (mKeepStatus.CurrentSP < mKeepStatus.MaxSP)
                    {
                        mKeepStatus.CurrentSP += mKeepStatus.RecoverySP * Time.deltaTime;
                    }
                }
                if (mKeepStatus.CurrentSP <= 0)
                {
                    mKeepStatus.CurrentSP = 0;
                    mIsStaminaCoolDown = true;
                }
            }
            else
            {
                if (mKeepStatus.CurrentSP < mKeepStatus.MaxSP)
                {
                    mKeepStatus.CurrentSP += mKeepStatus.RecoverySP * Time.deltaTime;
                }
                if (mKeepStatus.CurrentSP >= mKeepStatus.MaxSP)
                {
                    mKeepStatus.CurrentSP = mKeepStatus.MaxSP;
                    mIsStaminaCoolDown = false;
                }
            }
        }
    }

}
