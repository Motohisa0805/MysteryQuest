using System;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class CloseDoorTimer
    {
        [Header("ƒhƒA‚ª•Â‚Ü‚é‚Ü‚Å‚ÌŽžŠÔ")]
        [SerializeField]
        private float mCloseTime = 3f;
        public float CloseTime => mCloseTime;

        private Timer mTimer = new Timer();

        public Timer Timer => mTimer;
        public void Update(float deltaTime)
        {
            mTimer.Update(deltaTime);
        }

        public void Start()
        {
            mTimer.Start(mCloseTime);
        }
    }
}
