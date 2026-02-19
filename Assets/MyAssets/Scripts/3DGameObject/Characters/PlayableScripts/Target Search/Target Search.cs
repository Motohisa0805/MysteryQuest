using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyAssets
{
    //ターゲットを探すスクリプト
    //PlayableChracterControllerが必要
    //ターゲットを視界内で探し出す
    [RequireComponent(typeof(PlayableChracterController))]
    public class TargetSearch : MonoBehaviour
    {
        //検索するタイマー
        private Timer mCurrentSearchinTimer = new Timer();
        //発見したオブジェクトを取得
        [SerializeField]
        private Transform mTargetObject;
        public Transform TargetObject => mTargetObject;
        //nullじゃないのならtrue、nullならfalse
        public bool FindTarget => mTargetObject != null;
        //ターゲットが最後にいた座標
        [SerializeField]
        private Vector3 mTargetLastPoint;
        public Vector3 TargetLastPoint => mTargetLastPoint;

        //新しく調べるまでのカウント
        [SerializeField]
        private float mRefreshTime = 0.1f;
        //視界の距離
        [SerializeField]
        private float mRange = 10.0f;
        //視界の広さ
        [SerializeField]
        private float mViewAngle = 45.0f;
        //探すオブジェクトをレイヤーで取得
        [SerializeField]
        LayerMask mTargetObjectLayer = Physics.AllLayers;
        //広さか360度かを決めるフラグ
        [SerializeField]
        private bool mAllSearchFlag = false;

        //探しているオブジェクトを発見したか
        [SerializeField]
        private bool mFindFlag = false;
        public bool FindFlag => mFindFlag;

        // 視界範囲内のオブジェクトリスト
        List<Transform> mInsideObjects = new List<Transform>();
        //発見したオブジェクトとの距離
        public Vector3 GetSubDistance => mTargetLastPoint - transform.position;

        public void SetAllSearch(bool a) { mAllSearchFlag = a; }
        public bool IsInside(Transform obj) => mInsideObjects.Contains(obj);

        private void Start()
        {
            StartCoroutine(UpdateRoutine());
        }

        public bool TryGetFirstObject(out Transform outTransform)
        {
            outTransform = null;
            float bestScore = float.MaxValue;

            foreach (var obj in mInsideObjects)
            {
                Vector3 toTarget = obj.position - transform.position;
                float distSqr = toTarget.sqrMagnitude; // 二乗で計算（高速）
                float angle = Vector3.Angle(transform.forward, toTarget.normalized);
                //距離と角度でスコアが一番小さい物を選択
                float score = distSqr + (angle * 3.0f);

                if (score < bestScore)
                {
                    bestScore = score;
                    outTransform = obj;
                }
            }
            return outTransform != null;
        }

        private void Update()
        {
            mCurrentSearchinTimer.Update(Time.deltaTime);

            //今追っかけてるオブジェクトが見える
            if (IsInside(mTargetObject))
            {
                if (mTargetObject != null)
                {
                    mTargetLastPoint = mTargetObject.transform.position;
                    mCurrentSearchinTimer.End();
                }
                return;
            }
            else
            {
                mTargetObject = null;
            }

            //見えないなら

            //新しいオブジェクトが見えたらそちらを追いかけるように切り替える
            if (TryGetFirstObject(out var obj))
            {
                mTargetObject = obj;
                mTargetLastPoint = mTargetObject.transform.position;
                mCurrentSearchinTimer.End();
                return;
            }

            //新しいオブジェクトもいないなら,一定時間で終了するためタイマースタート
            if (mCurrentSearchinTimer.IsEnd())
            {
                mCurrentSearchinTimer.Start(1.0f);
            }
        }

        public void AllSearchStart()
        {
            if (mAllSearchFlag) { return; }
            mAllSearchFlag = true;
            StartCoroutine(EndAllSearch());
        }
        private System.Collections.IEnumerator EndAllSearch()
        {
            yield return new WaitForSecondsRealtime(1f); // 1フレーム待つ
            mAllSearchFlag = false;
        }

        IEnumerator UpdateRoutine()
        {
            var refreshWait = new WaitForSeconds(mRefreshTime);
            Collider[] colliders = new Collider[10]; // 予め一定数のコライダ用配列を用意
            while (true)
            {
                yield return refreshWait;

                // 範囲内のコライダを取得し、重複を避けるために一時リストに格納
                int hitCount = Physics.OverlapSphereNonAlloc(transform.position, mRange, colliders, mTargetObjectLayer);

                mInsideObjects.Clear(); // 前の結果をクリア
                for (int i = 0; i < hitCount; i++)
                {
                    //OverlapSphereNonAllocで取得した対象オブジェクトを1つ調べる
                    Transform tra = colliders[i].transform;
                    TargetFilter filter = colliders[i].GetComponent<TargetFilter>();

                    // 視野角の範囲内かを確認
                    //差分を正規化
                    Vector3 directionToObject = (tra.transform.position - transform.position).normalized;
                    //
                    float angle = Vector3.Angle(transform.forward, directionToObject);
                    //視野範囲かどうか
                    if (angle <= mViewAngle || filter != null && filter.TypeFilter == TargetFilter.Filter.Enemy)
                    {
                        // Raycastで壁越しを除去
                        if (Physics.Raycast(transform.position, directionToObject, out RaycastHit hit, mRange, mTargetObjectLayer))
                        {
                            //
                            if (hit.transform == tra || hit.collider.gameObject.layer == mTargetObjectLayer)
                            {
                                mFindFlag = true;
                                mInsideObjects.Add(tra); // オブジェクトを視界内リストに追加
                            }
                            else
                            {
                                mFindFlag = false;
                            }
                        }
                    }
                }
                if (hitCount == 0)
                {
                    mFindFlag = false;
                }
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            UnityEditor.Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                Quaternion.Euler(0, -mViewAngle, 0) * transform.forward,
                mViewAngle * 2.0f,
                mRange);
        }
#endif
    }
}
