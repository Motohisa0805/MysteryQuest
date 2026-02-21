using System;
using UnityEngine;
using static MyAssets.ChemistryObject;

namespace MyAssets
{
    //疑似化学エンジンのエレメントクラス
    //当たったオブジェクトにエレメントタイプを渡す
    public class ChemistryElement : MonoBehaviour
    {
        [SerializeField]
        private ElementType                     mElementType;
        public ElementType                      Type => mElementType;

        private ElementType                     mBaseElementType;

        // 周囲20個まで検出
        private Collider[]                      mScanResults = new Collider[20];
        // 0.2秒ごとに判定（負荷対策）
        private float                           mScanInterval = 0.2f;

        private float                           mScanTimer = 0f;

        [SerializeField]
        private LayerMask                       mTargetObjectLayer = (1 << 3) | (1 << 6);

        // 周囲の接触属性を管理する変数（フラグの組み合わせで複数の属性を同時に管理）
        private ElementType                     mTotalContactElement;

        //反応判定用のテーブル
        private ChemistryTable                  mReactionTable;


        // 反応待ちの状態を管理する変数
        private ElementToElementReactionResult  mPendingReaction;

        //エフェクト再生用のリファレンス
        private EffectReturner                  mMyElementEffect;
        public EffectReturner                   MyElementEffect { get => mMyElementEffect; set => mMyElementEffect = value; }

        private ChemistryObject                 mParentMaterial;
        public ChemistryObject                  ParentMaterial { get => mParentMaterial; set => mParentMaterial = value; }

        //エレメントを自然と消すまでのタイマー
        //エレメントはタイマーで消すようにする
        private Timer                           mElementEraseTimer = new Timer();
        public Timer                            ElementEraseTimer => mElementEraseTimer;

        private AudioSource                     mLoopAudio;

        private void Awake()
        {
            mMyElementEffect = GetComponent<EffectReturner>();
        }
        //ここは再度使う時に処理
        private void OnEnable()
        {
            if (!mMyElementEffect)
            {
                mMyElementEffect = GetComponent<EffectReturner>();
            }
            //再度使う時に設定
            if(mBaseElementType != ElementType.None)
            {
                mElementType = mBaseElementType;
            }
            mTotalContactElement = ElementType.None;
        }
        //初回起動時に設定
        private void Start()
        {
            mBaseElementType = mElementType;
            mTotalContactElement = ElementType.None;

            mTargetObjectLayer = 1 << 3 | 1 << 6;

            //化学反応テーブル取得(シングルトンからのアクセス頻度を減らすため)
            mReactionTable = GameSystemManager.Instance.ChemistryTable;

            mElementEraseTimer.OnEnd += ProcessDestruction;

            //エレメントによってSEを再生
            SetLoopSE();
        }

        private void SetLoopSE()
        {
            switch(mElementType)
            {
                case ElementType.Fire:
                    //燃えているSE
                    mLoopAudio = SoundManager.Instance.PlayLoopSE("Object_Fire", transform.position, transform);
                    break;
            }
        }

        // カプセルの始点・終点・半径を計算するヘルパー関数
        private void GetCapsulePoints(CapsuleCollider capsule, out Vector3 p0, out Vector3 p1, out float radius)
        {
            // カプセルの向き (0:X, 1:Y, 2:Z)
            Vector3 dir = Vector3.up;
            float height = capsule.height;
            float r = capsule.radius;

            // スケール適用
            Vector3 scale = transform.lossyScale;
            float scaleHeight = 1f;
            float scaleRadius = 1f;

            switch (capsule.direction)
            {
                case 0: // X-Axis
                    dir = Vector3.right;
                    scaleHeight = scale.x;
                    scaleRadius = Mathf.Max(scale.y, scale.z);
                    break;
                case 1: // Y-Axis
                    dir = Vector3.up;
                    scaleHeight = scale.y;
                    scaleRadius = Mathf.Max(scale.x, scale.z);
                    break;
                case 2: // Z-Axis
                    dir = Vector3.forward;
                    scaleHeight = scale.z;
                    scaleRadius = Mathf.Max(scale.x, scale.y);
                    break;
            }

            float worldHeight = height * scaleHeight;
            radius = r * scaleRadius;

            // 中心から上下（または左右前後）にオフセット
            float halfHeight = Mathf.Max(0, (worldHeight * 0.5f) - radius);
            Vector3 center = transform.TransformPoint(capsule.center);

            // 回転を考慮した方向ベクトル
            Vector3 axis = transform.rotation * dir;

            p0 = center - (axis * halfHeight);
            p1 = center + (axis * halfHeight);
        }

        private void PerformSurroundScan()
        {
            // 進行中の反応がある場合は、周囲を見ない
            //if (mPendingReaction.gElementToAdd != ElementType.None) return;

            int hitCount = 0;

            // 自分についているコライダーを取得
            Collider myCollider = GetComponent<Collider>();

            // コライダーの種類によって判定方法を変える
            if (myCollider is BoxCollider box)
            {
                // BoxColliderの場合：回転も考慮した箱型判定
                // Centerはローカル座標なので、ワールド座標に変換が必要
                Vector3 worldCenter = transform.TransformPoint(box.center);
                // Sizeの半分 * スケール = 半分の大きさ(HalfExtents)
                Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, transform.lossyScale);

                hitCount = Physics.OverlapBoxNonAlloc(
                    worldCenter,
                    halfExtents,
                    mScanResults,
                    transform.rotation, // 回転を考慮
                    mTargetObjectLayer,
                    QueryTriggerInteraction.Collide
                );
            }
            else if (myCollider is SphereCollider sphere)
            {
                // SphereColliderの場合：球形判定
                // 半径にスケールの最大値を掛けてワールドサイズにする
                float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
                Vector3 worldCenter = transform.TransformPoint(sphere.center);
                float radius = sphere.radius * maxScale;

                hitCount = Physics.OverlapSphereNonAlloc(
                    worldCenter,
                    radius,
                    mScanResults,
                    mTargetObjectLayer,
                    QueryTriggerInteraction.Collide
                );
            }
            else if (myCollider is CapsuleCollider capsule)
            {
                // CapsuleColliderの場合：カプセル型判定
                // カプセルの「始点」と「終点」を計算する必要がある（少し計算が複雑）
                GetCapsulePoints(capsule, out Vector3 point0, out Vector3 point1, out float radius);

                hitCount = Physics.OverlapCapsuleNonAlloc(
                    point0,
                    point1,
                    radius,
                    mScanResults,
                    mTargetObjectLayer,
                    QueryTriggerInteraction.Collide
                );
            }

            for (int i = 0; i < hitCount; i++)
            {
                Collider col = mScanResults[i];

                //自分自身は無視
                if (col.transform == transform) continue;

                //相手がエレメントか
                if (col.TryGetComponent(out ChemistryElement elemComp))
                {
                    mTotalContactElement |= elemComp.Type;
                }
            }

            if (mTotalContactElement == ElementType.None) return;
            // 新しい属性があるかチェック
            ElementType newElements = mTotalContactElement & ~mElementType;

            if (newElements != ElementType.None)
            {
                foreach (ElementType checkType in Enum.GetValues(typeof(ElementType)))
                {
                    if (checkType == ElementType.None) continue;
                    if ((newElements & checkType) == checkType)
                    {
                        Debug.Log("鎮火チェック");
                        if (mReactionTable.TryGetElementToElementReaction(mElementType, checkType, out ElementToElementReactionResult result))
                        {
                            Debug.Log("鎮火開始");
                            StartReactionProcess(result);
                            break;
                        }
                        else
                        {
                            mTotalContactElement = ElementType.None;
                        }
                    }
                }
            }
            else
            {
                mTotalContactElement = ElementType.None;
            }
        }

        // 反応結果を受け取って反応待ち状態を開始する関数
        private void StartReactionProcess(ElementToElementReactionResult result)
        {
            // 反応待ち状態を開始
            mPendingReaction = result;
        }

        // 反応待ち状態を更新する関数（接触状態に応じて熱量を増減させる）
        private void UpdatePendingReaction(bool isTouching)
        {
            // 追加する属性がなく、削除する属性もないなら何もしない
            if (mPendingReaction.gElementToAdd == ElementType.None && mPendingReaction.gElementToRemove == ElementType.None) return;
            // 接触していれば直ちに適用（あるいは短時間のタイマーを設けるか）
            if (mElementType == ElementType.Fire&&mPendingReaction.gElementToAdd == ElementType.Water)
            {
                
                ApplyReaction(mPendingReaction);
                mPendingReaction = default; // 適用後はリセット
            }
        }

        //反応結果を適用する
        // ここで属性の追加・削除を行い、必要に応じてエフェクトを出し、燃え尽きる処理の開始なども行う
        private void ApplyReaction(ElementToElementReactionResult result)
        {
            // 属性の追加・削除
            mElementType |= result.gElementToAdd;
            mElementType &= ~result.gElementToRemove;
            //もし火が水に変わったら、燃え尽きる処理を開始する
            if (mElementType == ElementType.Water)
            {
                //エレメントの停止処理
                //水に触れたら即鎮火
                mElementEraseTimer.End();
            }
        }

        private void Update()
        {
            // スキャンタイマーを進める
            mScanTimer += Time.deltaTime;

            // 一定時間ごとに周囲をスキャンして反応を更新
            if (mScanTimer >= mScanInterval)
            {
                mScanTimer = 0f;
                PerformSurroundScan(); // ここで判定
            }

            // 反応待ちがある場合、接触状況に応じて蓄熱・冷却を行う
            if (mPendingReaction.gElementToAdd != ElementType.None)
            {
                // 接触している属性の中に、反応を引き起こす属性が含まれているかチェック
                bool isTouchingTrigger = (mTotalContactElement & mPendingReaction.gElementToAdd) != 0;
                UpdatePendingReaction(isTouchingTrigger);
            }

            mElementEraseTimer.Update(Time.deltaTime);
        }
        
        public void StopElement()
        {
            if (transform.parent != null)
            {
                transform.parent = null;
            }
            if (mMyElementEffect != null)
            {
                mMyElementEffect.StopAndReturn(true);
            }
        }

        // エレメントの返還処理を行う関数
        public void ProcessDestruction()
        {
            // ここでは単純にエフェクトを出してオブジェクトを非アクティブにする例
            //自分のエレメントを止める
            StopElement();
            //マテリアルがあるなら
            if(mParentMaterial != null)
            {
                mParentMaterial.ProcessEraseElement();
            }
            mParentMaterial = null;
            //ループSEを返還
            SoundManager.Instance.StopLoopSE(mLoopAudio, 0.25f);
        }
    }
}
