using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{

    public class PlayerStatusManager : MonoBehaviour
    {
        [Serializable]
        public struct PlayerStatus
        {
            public float MaxHP;
            public float CurrentHP;
            public float MaxStamina;
            public float CurrentStamina;
            public float UseStamina;
            public float RecoveryStamina;
            public float ThrowPower;
            //最高速度
            public float MaxSpeed;
            //加速度
            public float Acceleration;
            //最高速度
            public float DushMaxSpeed;
            //しゃがみ時の最高速度
            public float CrouchMaxSpeed;
            //待機ジャンプ力
            public float IdleJumpPower;
            //移動ジャンプ力
            public float MoveJumpPower;
            //回転速度
            public float RotationSpeed;
            //ショルダービュー時の回転速度
            public float ShoulderViewRotationSpeed;
        }
        private static PlayerStatusManager  instance;
        public static PlayerStatusManager   Instance => instance;

        [SerializeField]
        private PlayerStatus                mPlayerStatus;
        [SerializeField]
        private PlayerStatus                mKeepStatus;
        public PlayerStatus                 PlayerStatusData => mKeepStatus;

        private bool                        mIsStaminaCoolDown;
        public bool                         IsStaminaCoolDown => mIsStaminaCoolDown;
        public bool                         IsNoSprit => mPlayerStatus.CurrentStamina <= 0 && mIsStaminaCoolDown;

        private bool                        mUseStamina;
        public bool                         UseStamina => mUseStamina;
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
            mPlayerStatus.MaxHP = 360;
            mPlayerStatus.CurrentHP = 360;
            mPlayerStatus.MaxStamina = 70;
            mPlayerStatus.CurrentStamina = 70;
            mPlayerStatus.UseStamina = 10;
            mPlayerStatus.RecoveryStamina = 30;
            mPlayerStatus.ThrowPower = 10;
            mPlayerStatus.MaxSpeed = 5;
            mPlayerStatus.Acceleration = 100;
            mPlayerStatus.DushMaxSpeed = 7.5f;
            mPlayerStatus.CrouchMaxSpeed = 3;
            mPlayerStatus.IdleJumpPower = 2;
            mPlayerStatus.MoveJumpPower = 5;
            mPlayerStatus.RotationSpeed = 15;
            mPlayerStatus.ShoulderViewRotationSpeed = 1000;

            mKeepStatus.MaxHP = mPlayerStatus.MaxHP;
            mKeepStatus.CurrentHP = mPlayerStatus.CurrentHP;
            mKeepStatus.MaxStamina = mPlayerStatus.MaxStamina;
            mKeepStatus.CurrentStamina = mPlayerStatus.CurrentStamina;
            mKeepStatus.UseStamina = mPlayerStatus.UseStamina;
            mKeepStatus.RecoveryStamina = mPlayerStatus.RecoveryStamina;
            mKeepStatus.ThrowPower = mPlayerStatus.ThrowPower;
            mKeepStatus.MaxSpeed = mPlayerStatus.MaxSpeed;
            mKeepStatus.Acceleration = mPlayerStatus.Acceleration;
            mKeepStatus.DushMaxSpeed = mPlayerStatus.DushMaxSpeed;
            mKeepStatus.CrouchMaxSpeed = mPlayerStatus.CrouchMaxSpeed;
            mKeepStatus.IdleJumpPower = mPlayerStatus.IdleJumpPower;
            mKeepStatus.MoveJumpPower = mPlayerStatus.MoveJumpPower;
            mKeepStatus.RotationSpeed = mPlayerStatus.RotationSpeed;
            mKeepStatus.ShoulderViewRotationSpeed = mPlayerStatus.ShoulderViewRotationSpeed;
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
            mKeepStatus.MaxStamina = mPlayerStatus.MaxStamina;
            mKeepStatus.CurrentStamina = mPlayerStatus.CurrentStamina;
            mKeepStatus.UseStamina = mPlayerStatus.UseStamina;
            mKeepStatus.RecoveryStamina = mPlayerStatus.RecoveryStamina;
            mKeepStatus.ThrowPower = mPlayerStatus.ThrowPower;
            mKeepStatus.MaxSpeed = mPlayerStatus.MaxSpeed;
            mKeepStatus.Acceleration = mPlayerStatus.Acceleration;
            mKeepStatus.DushMaxSpeed = mPlayerStatus.DushMaxSpeed;
            mKeepStatus.CrouchMaxSpeed = mPlayerStatus.CrouchMaxSpeed;
            mKeepStatus.IdleJumpPower = mPlayerStatus.IdleJumpPower;
            mKeepStatus.MoveJumpPower = mPlayerStatus.MoveJumpPower;
            mKeepStatus.RotationSpeed = mPlayerStatus.RotationSpeed;
            mKeepStatus.ShoulderViewRotationSpeed = mPlayerStatus.ShoulderViewRotationSpeed;
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
                mUseStamina = input;
                if (input)
                {
                    if (mKeepStatus.CurrentStamina > 0)
                    {
                        mKeepStatus.CurrentStamina -= mKeepStatus.UseStamina * Time.deltaTime;
                    }
                }
                else
                {
                    if (mKeepStatus.CurrentStamina < mKeepStatus.MaxStamina)
                    {
                        mKeepStatus.CurrentStamina += mKeepStatus.RecoveryStamina * Time.deltaTime;
                    }
                }
                if(mKeepStatus.CurrentStamina <= 0)
                {
                    mKeepStatus.CurrentStamina = 0;
                    mIsStaminaCoolDown = true;
                }
            }
            else
            {
                mUseStamina = input;
                if (mKeepStatus.CurrentStamina < mKeepStatus.MaxStamina)
                {
                    mKeepStatus.CurrentStamina += mKeepStatus.RecoveryStamina * Time.deltaTime;
                }
                if(mKeepStatus.CurrentStamina >= mKeepStatus.MaxStamina)
                {
                    mKeepStatus.CurrentStamina = mKeepStatus.MaxStamina;
                    mIsStaminaCoolDown = false;
                }
            }

        }

        public void RecoverySP(bool flag)
        {
            mUseStamina = false;
            if (!mIsStaminaCoolDown)
            {
                if (flag)
                {
                    if (mKeepStatus.CurrentStamina < mKeepStatus.MaxStamina)
                    {
                        mKeepStatus.CurrentStamina += mKeepStatus.RecoveryStamina * Time.deltaTime;
                    }
                }
                if (mKeepStatus.CurrentStamina <= 0)
                {
                    mKeepStatus.CurrentStamina = 0;
                    mIsStaminaCoolDown = true;
                }
            }
            else
            {
                if (mKeepStatus.CurrentStamina < mKeepStatus.MaxStamina)
                {
                    mKeepStatus.CurrentStamina += mKeepStatus.RecoveryStamina * Time.deltaTime;
                }
                if (mKeepStatus.CurrentStamina >= mKeepStatus.MaxStamina)
                {
                    mKeepStatus.CurrentStamina = mKeepStatus.MaxStamina;
                    mIsStaminaCoolDown = false;
                }
            }
        }
    }

}
