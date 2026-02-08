using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class MoveFootStageDevice : MonoBehaviour, IGimmickReceiver
    {
        [Header("足場となるオブジェクト")]
        [SerializeField]
        private MoveObjectCollider[]    mFootStages = new MoveObjectCollider[0];

        [Header("足場を指定した地点を行き来させるか")]
        [SerializeField]
        private bool                    mReturn = false;


        [Header("移動速度 (m/s)")]
        [SerializeField]
        private float                   mSpeed = 2.0f;

        [SerializeField]
        private float                   mGap = 3.0f;

        [SerializeField]
        private bool                    mMoveStop = false;
        public bool                     MoveStop
        {
            get { return mMoveStop; }
            set { mMoveStop = value; }
        }

        [Header("移動ルートの座標 (ローカル座標)")]
        [SerializeField]
        private List<Vector3>           mWaypoints = new List<Vector3>();

        // 外部から座標を取得するためのプロパティ
        public List<Vector3>            Waypoints => mWaypoints;

        // 内部計算用：ルートの総距離
        private float                   mTotalDistance = 0f;
        // 内部計算用：各ウェイポイントまでの積算距離
        private float[]                 mDistances;
        // 現在の進行距離タイマー
        private float                   mCurrentDist = 0f;

        private void Start()
        {
            InitializePath();
        }

        //ルートの総距離や区間ごとの距離を事前計算する
        private void InitializePath()
        {
            if (mWaypoints.Count < 2) return;
            int segmentCount = mReturn ? mWaypoints.Count : mWaypoints.Count - 1;

            mDistances = new float[mWaypoints.Count + 1];// 累積距離キャッシュ
            mTotalDistance = 0f;

            for(int i = 0; i < mWaypoints.Count; i++)
            {
                mDistances[i] = mTotalDistance;

                if(i < mWaypoints.Count - 1)
                {
                    mTotalDistance += Vector3.Distance(mWaypoints[i], mWaypoints[i + 1]);
                }
                else if(!mReturn)
                {
                    mTotalDistance += Vector3.Distance(mWaypoints[i], mWaypoints[0]);
                }
            }
        }

        private void FixedUpdate()
        {
            if(mWaypoints.Count < 2)return;
            if(mMoveStop) return;
            // 距離を進める
            mCurrentDist += mSpeed * Time.deltaTime;

            // 足場を動かす
            UpdatePlatforms();
        }
        // 全ての足場の位置を更新
        private void UpdatePlatforms()
        {
            if (mFootStages == null || mFootStages.Length == 0) return;
            if (mTotalDistance <= 0) return;

            for(int i = 0; i < mFootStages.Length; i++)
            {
                // 先頭からの遅れ（オフセット）を計算
                float offset = i * mGap;

                //この足場の目標距離
                float targetDist = mCurrentDist - offset;

                // 距離に応じた座標を取得して反映
                Vector3 pos = GetPositionAtDistance(targetDist);

                //親の座標系に合わせる（mWaypointsはローカルなのでTransformPointする
                mFootStages[i].transform.position = transform.TransformPoint(pos);
            }
        }

        /// 距離(m)から座標(Vector3)を求める関数
        private Vector3 GetPositionAtDistance(float dist)
        {
            // 1. 距離を 0 ～ 総距離 の範囲に正規化する
            float calculatedDist = dist;
            if(mReturn)
            {
                // 往復 (PingPong)
                // 距離 time に対して、0 -> Total -> 0 となるように変換
                calculatedDist = Mathf.PingPong(dist, mTotalDistance);
            }
            else
            {
                // 周回 (Loop)
                // 負の値になっても正の余剰になるように計算 (例: -3m -> 後ろから3m)
                calculatedDist = Mathf.Repeat(dist, mTotalDistance);
            }

            // 2. どの区間(セグメント)にいるか探す
            // シンプルな線形探索
            for(int i = 0; i < mWaypoints.Count; i++)
            {
                //区間の長さ
                float segmentLen = 0f;
                Vector3 p1 = mWaypoints[i];
                Vector3 p2 = Vector3.zero;

                //次の点
                if(i < mWaypoints.Count - 1)
                {
                    p2 = mWaypoints[i + 1];
                    segmentLen = Vector3.Distance(p1, p2);
                }
                else if(!mReturn)
                {
                    p2 = mWaypoints[0];
                    segmentLen = Vector3.Distance(p1, p2);
                }
                else
                {
                    // 往復の端の点
                    return mWaypoints[i];
                }

                //現在の距離がこの区間内なら、補間して座標を返す
                if(calculatedDist <= segmentLen)
                {
                    //区間内の進行度(0.0 ~ 1.0)
                    float t = calculatedDist / segmentLen;
                    return Vector3.Lerp(p1, p2, t);
                }

                // 次の区間へ
                calculatedDist -= segmentLen;
            }
            return mWaypoints[0]; // フェイルセーフ
        }


        public void OnActivate(float count = 0)
        {
            mMoveStop = true;
        }
        public void OnDeactivate()
        {
            mMoveStop = false;
        }
        /// <summary>
        /// 選択されている時のみギズモ（可視化ライン）を表示
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // ポイントがなければ何もしない
            if (mWaypoints == null || mWaypoints.Count == 0) return;

            Gizmos.color = Color.cyan; // 線の色

            // 現在地（親オブジェクト）を基準としたワールド座標に変換して描画
            Vector3 parentPos = transform.position;

            for (int i = 0; i < mWaypoints.Count; i++)
            {
                // ローカル座標をワールド座標に変換（親が動いても追従するように）
                Vector3 currentPoint = transform.TransformPoint(mWaypoints[i]);

                // 1. ポイントに球を描画
                Gizmos.DrawSphere(currentPoint, 0.3f);

                // 2. 次のポイントへ線を引く
                if (i < mWaypoints.Count - 1)
                {
                    Vector3 nextPoint = transform.TransformPoint(mWaypoints[i + 1]);
                    Gizmos.DrawLine(currentPoint, nextPoint);
                }
                // 3. mReturnがtrueなら、最後から最初へ線を引く
                else if (mReturn && mWaypoints.Count > 1)
                {
                    Vector3 firstPoint = transform.TransformPoint(mWaypoints[0]);
                    Gizmos.DrawLine(currentPoint, firstPoint);
                }
            }
        }
    }
}

