using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class EventManager : MonoBehaviour
    {
        private static EventManager         mInstance;
        public static EventManager          Instance => mInstance;

        //プレイヤーに関係するイベント変数

        private PlayableChracterController  mPlayableChracterController;

        private TPSCamera                   mTPSCamera;

        private EventPoint                  mEventMoveTargetPosition;
        public EventPoint                   EventMoveTargetPosition => mEventMoveTargetPosition;

        private List<EventPoint>            mEventMovePointList = new List<EventPoint>();
        public List<EventPoint>             EventMovePointList => mEventMovePointList;

        private EffectReturner              mConfettiParticleSystem;

        private Action                      mStartEventAction;
        private void Awake()
        {
            mInstance = this;
        }

        public void InitializeEvent()
        {
            mEventMovePointList.Clear();
            EventPoint[] points = FindObjectsByType<EventPoint>(FindObjectsSortMode.None);
            mEventMovePointList = new List<EventPoint>(points);
            //クイックソート
            if (mEventMovePointList.Count > 1)
            {
                QuickSort(0, mEventMovePointList.Count - 1);
            }
        }
        private void QuickSort(int left, int right)
        {
            if(left >= right)
            {
                return;
            }
            int v = Partition(left, right);
            QuickSort(left, v - 1);
            QuickSort(v + 1, right);
        }
        private int Partition(int left, int right)
        {
            int i = left;
            int j = right - 1;
            int pivot = mEventMovePointList[right].PointNo;
            while (true)
            {
                // 左からピボット以上の値を探す
                while (i < right && mEventMovePointList[i].PointNo < pivot) i++;
                // 右からピボット以下の値を探す
                while (j >= i && mEventMovePointList[j].PointNo > pivot) j--;

                if (i >= j) break;

                // 要素の入れ替え
                Swap(i, j);
            }
            // 最後にピボットを適切な位置（i）に移動
            Swap(i, right);
            return i;
        }
        // 入れ替え処理を共通化するとスッキリします
        private void Swap(int indexA, int indexB)
        {
            EventPoint tmp = mEventMovePointList[indexA];
            mEventMovePointList[indexA] = mEventMovePointList[indexB];
            mEventMovePointList[indexB] = tmp;
        }

        private void Start()
        {
            mPlayableChracterController = FindAnyObjectByType<PlayableChracterController>();
            if(mPlayableChracterController == null)
            {
                Debug.LogWarning("PlayableChracterControllerが見つかりません。");
            }

            mTPSCamera = FindAnyObjectByType<TPSCamera>();
            if (mTPSCamera == null)
            {
                Debug.LogWarning("TPSCameraが見つかりません。");
            }

            InitializeEvent();
            StartCoroutine(StartEvent());

            ResultManager.IsStopGameUIInput = false;
            ResultManager.IsPlayerDeath = false;
        }

        public IEnumerator StartEvent()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            PlayOpeningCutscene().Forget();
        }

        public void SetEventMoveTargetPoint(int index)
        {
            if (mEventMovePointList.Count > 0)
            {
                mEventMoveTargetPosition = mEventMovePointList[index];
            }
        }

        public void ClearEventMoveTargetPoint(int index)
        {
            if (mEventMovePointList.Count > 0)
            {
                Destroy(mEventMovePointList[index].gameObject);
                mEventMovePointList.RemoveAt(index);
            }
        }

        //1回のイベントの流れ
        public async UniTaskVoid PlayOpeningCutscene()
        {
            if(mEventMovePointList.Count <= 0) { return; }
            //カーソルを固定
            InputManager.SetLockedMouseMode();
            mPlayableChracterController.transform.position = new Vector3(mEventMovePointList[0].transform.position.x, mEventMovePointList[0].transform.position.y + 0.75f, mEventMovePointList[0].transform.position.z);
            await mPlayableChracterController.EventIdleToAsync();
            //await mPlayableChracterController.PlayIdleToAsync();
            SetEventMoveTargetPoint(0);
            await mEventMovePointList[0].SetConfig();

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            SetEventMoveTargetPoint(1);
            await mPlayableChracterController.MoveToAsync();

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            await mPlayableChracterController.PlayIdleToAsync();

            SoundManager.Instance.PlayBGM("StageBGM01");
        }

        public void OnPlayEndingCutscene()
        {
            PlayEndingCutscene().Forget();
        }

        public async UniTaskVoid PlayEndingCutscene()
        {
            if (mEventMovePointList.Count <= 0) { return; }
            SoundManager.Instance.PlayBGM("GameClearBGM", false);
            GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.Tutorial);
            ResultManager.IsStopGameUIInput = true;
            //3番目のポイントに移動
            SetEventMoveTargetPoint(2);
            await mPlayableChracterController.MoveToAsync();
            //少し待機
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            //プレイヤーを180度カメラ方向に回転

            //カメラの追従を解除
            if(mTPSCamera != null)
            {
                mTPSCamera.enabled = false;
            }

            //エレベーターを動かす
            SetEventMoveTargetPoint(3);
            await mEventMovePointList[3].SetConfig();

            await UniTask.Delay(TimeSpan.FromSeconds(3));
            //カーソル固定を解除
            InputManager.SetNoneMouseMode();
            GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.HUD);
            GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.Result);

            TPSCamera camera = FindAnyObjectByType<TPSCamera>();
            if (camera != null)
            {
                mConfettiParticleSystem = EffectManager.Instance.PlayEffect<ParticleSystem>("Confetti", Vector3.zero, Quaternion.identity, Vector3.one, camera.transform).GetComponent<EffectReturner>();
                mConfettiParticleSystem.transform.localPosition = new Vector3(0, 2.5f, 5);

                SoundManager.Instance.PlayOneShot2D("Applause and cheers");
            }
        }

        public async UniTaskVoid DeathEvent()
        {
            ResultManager.IsStopGameUIInput = true;
            ResultManager.IsPlayerDeath = true;
            GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.Tutorial);
            GameUserInterfaceManager.Instance.SetActiveHUD(false, GameHUDType.GameUIPanelType.HUD);
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            //カーソル固定を解除
            InputManager.SetNoneMouseMode();
            GameUserInterfaceManager.Instance.SetActiveHUD(true, GameHUDType.GameUIPanelType.Result);
            SoundManager.Instance.PlayBGM("GameOverBGM", false);
        }

        private void OnDestroy()
        {
            if (mConfettiParticleSystem != null)
            {
                // オブジェクトは残るので、くっついたままフェードアウトさせる (false)
                mConfettiParticleSystem.StopAndReturn(false);
                mConfettiParticleSystem = null;
            }
            mInstance = null;
        }
    }
}
