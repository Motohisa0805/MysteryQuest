using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class SwitchControler : MonoBehaviour
    {
        [SerializeField]
        private List<MonoBehaviour> mReceivers = new List<MonoBehaviour>();

        private bool mIsOn = false;

        private MaterialColorChange mMaterialColorChange;

        [Header("スイッチに関係する設定群")]
        [Header("スイッチが時間でオフにする")]
        [SerializeField]
        private bool mTimerDisable = false;
        [SerializeField]
        private CloseDoorTimer mCloseDoorTimer;

        private void Awake()
        {
            mMaterialColorChange = GetComponentInChildren<MaterialColorChange>();
        }

        private void Start()
        {
            if (mTimerDisable)
            {
                mCloseDoorTimer.Timer.OnEnd += () =>
                {
                    if (mIsOn)
                    {
                        mIsOn = false;
                        SendSignal(mIsOn);
                    }
                };
            }
        }

        private void Update()
        {
            if (mTimerDisable)
            {
                mCloseDoorTimer.Update(Time.deltaTime);
            }
        }

        private void SendSignal(bool isOn)
        {
            foreach (var obj in mReceivers)
            {
                if (obj is IGimmickReceiver receiver)
                {
                    if (isOn)
                    {
                        if(mMaterialColorChange != null)
                        {
                            mMaterialColorChange.ChangeEmissionColor();
                        }
                        if(mTimerDisable)
                        {
                            SoundManager.Instance.PauseBGM();
                            mCloseDoorTimer.Start();
                            CountdownIntervalPlayer.Instance.StartCountdown(mCloseDoorTimer.CloseTime);
                        }
                        receiver.OnActivate();
                    }
                    else
                    {
                        SoundManager.Instance.UnPauseStart();
                        if (mMaterialColorChange != null)
                        {
                            mMaterialColorChange.DisableEmissionColor();
                        }
                        receiver.OnDeactivate();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var material = other.gameObject.GetComponent<ChemistryObject>();
            if (material != null)
            {
                if (mTimerDisable && !mCloseDoorTimer.Timer.IsEnd() || material.Material == MaterialType.Organism) { return; }
                mIsOn = !mIsOn;
                SendSignal(mIsOn);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var material = collision.gameObject.GetComponent<ChemistryObject>();
            if (material != null)
            {
                if (mTimerDisable && !mCloseDoorTimer.Timer.IsEnd() || material.Material == MaterialType.Organism) { return; }
                mIsOn = !mIsOn;
                SendSignal(mIsOn);
            }
        }
    }
}
