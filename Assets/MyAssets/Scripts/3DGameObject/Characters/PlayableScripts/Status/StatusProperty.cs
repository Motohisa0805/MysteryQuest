using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public struct PlayerStatusPropaty
    {
        [Header("体力関連")]
        [SerializeField]
        private float mHP; //体力
        public float HP => mHP;
        [SerializeField]
        private float mMaxHP; //最大体力
        public float MaxHP => mMaxHP;
        [Header("スタミナ関連")]
        [SerializeField]
        private float mStamina; //スタミナ
        public float Stamina => mStamina;
        [SerializeField]
        private float mMaxStamina; //最大スタミナ
        public float MaxStamina => mMaxStamina;
        [Header("移動関連")]
        [SerializeField]
        private float mMaxSpeed; //最高速度
        public float MaxSpeed => mMaxSpeed;
        [Header("加速関連")]
        [SerializeField]
        private float mAcceleration; //加速度
        public float Acceleration => mAcceleration;
        [Header("ダッシュ関連")]
        [SerializeField]
        private float mDushMaxSpeed; //最高速度
        public float DushMaxSpeed => mDushMaxSpeed;
        [Header("しゃがみ関連")]
        [SerializeField]
        private float mCrouchMaxSpeed; //しゃがみ時の最高速度
        public float CrouchMaxSpeed => mCrouchMaxSpeed;
        [Header("ジャンプ関連")]
        [SerializeField]
        private float mIdleJumpPower; //ジャンプ力
        public float IdleJumpPower => mIdleJumpPower;
        [SerializeField]
        private float mMoveJumpPower; //ジャンプ力
        public float MoveJumpPower => mMoveJumpPower;
        [Header("回転関連")]
        [SerializeField]
        private float mRotationSpeed; //回転速度
        public float RotationSpeed => mRotationSpeed;
        [Header("ショルダービュー関連")]
        [SerializeField]
        private float mShoulderViewRotationSpeed; //ショルダービュー時の回転速度
        public float ShoulderViewRotationSpeed => mShoulderViewRotationSpeed;
    }
}
