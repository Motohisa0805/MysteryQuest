using System;
using UnityEngine;

namespace MyAssets
{
    [RequireComponent(typeof(Rigidbody))]
    //疑似化学エンジンのオブジェクトスクリプトファイル
    public class ChemistryObject : MonoBehaviour
    {
        [Serializable]
        public struct MaterialObjectInfo
        {
            public bool     mIsDestructible;
            
            public float    mDestroyDelay;

            public bool     mIsExtinguishing;

            public float    mScanScale; // 周囲スキャンの範囲を拡大する倍率（例: 1.5fなら1.5倍の範囲でスキャン）

            [Header("Wood、Organismの設定")]
            // 例: 草=0.1, 木箱=3.0, 大木=10.0
            [Header("燃えにくさ(熱容量)")]
            [Tooltip("この値まで熱が溜まると発火する(秒数換算)")]
            public float    mIgnitionResistance; 
            // 例: 草=0.1, 木箱=3.0, 大木=10.0
            [Header("火が消えるまでの消火量")]
            [Tooltip("この値が消火されるまでの時間(秒数換算)")]
            public float    mFireResistance; 

            //[Header("Iceの設定")]
        }

        [Serializable]
        public struct PhysicsMaterialInfo
        {
            public bool    mIsShock;
            [SerializeField]
            public bool    mIsDestructible;
            [SerializeField]
            private float  mCollisionForce;
            public float   CollisionForce { get { return mCollisionForce; } set { mCollisionForce = value; } }
        }

        [Header("この物体の素材(変わらない)")]
        [SerializeField]
        private MaterialType        mMaterial;
        public MaterialType         Material => mMaterial;

        [Header("現在帯びている属性（変化する）")]
        [SerializeField]
        private ElementType         mCurrentElements;
        public ElementType          CurrentElements => mCurrentElements;
        //マテリアルの詳細設定
        [SerializeField]
        private MaterialObjectInfo  mMaterialObjectInfo;

        [SerializeField]
        private PhysicsMaterialInfo mPhysicsMaterialInfo;
        public PhysicsMaterialInfo  PhysicsMaterial => mPhysicsMaterialInfo;

        [SerializeField]
        private LayerMask           mTargetObjectLayer = (1 << 3) | (1 << 6);

        //反応判定用のテーブル
        private ChemistryTable      mReactionTable;

        // 現在溜まっているエレメント
        private float               mCurrentHeatAccumulated = 0f; 
        //オブジェクト破壊までのタイマー
        private Timer               mDestroyTimer = new Timer();

        // 周囲の接触属性を管理する変数（フラグの組み合わせで複数の属性を同時に管理）
        private ElementType         mTotalContactElement;

        // 周囲20個まで検出
        private Collider[]          mScanResults = new Collider[20]; 
        // 0.2秒ごとに判定（負荷対策）
        private float               mScanInterval = 0.2f; 
        
        private float               mScanTimer = 0f;


        // 反応待ちの状態を管理する変数
        private MaterialToElementReactionResult      mPendingReaction;

        // 反応によって新たに付与された属性を管理するための変数（例: 火がついている状態を管理）
        private ChemistryElement    mGrantElement; 

        // 反応準備中のエフェクト（例: 煙が出ている状態など）を管理する変数
        private EffectReturner      mPreparationElementEffect;

        private Rigidbody           mRigidbody;
        public Rigidbody            Rigidbody => mRigidbody;

        private Vector3             mLastPosition;

        private Vector3             mCurrentVelocity;
        public Vector3              CurrentVelocity => mCurrentVelocity;

        [Header("オブジェクトの体力数値")]
        [SerializeField]
        private float               mHitPoint = 300.0f;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
        }
        //スタート時にイベント登録
        private void Start()
        {
            mTargetObjectLayer = 1 << 3 | 1 << 6;

            if(mPhysicsMaterialInfo.CollisionForce <= 0)
            {
                mPhysicsMaterialInfo.CollisionForce = 1.0f;
            }

            //化学反応テーブル取得(シングルトンからのアクセス頻度を減らすため)
            mReactionTable = GameSystemManager.Instance.ChemistryTable;
            //オブジェクト破壊タイマーにイベント登録
            if (mMaterialObjectInfo.mIsDestructible)
            {
                mDestroyTimer.OnEnd += ProcessDestruction;
            }

            // 最初の位置を初期化
            mLastPosition = transform.position;
        }
        // Updateは毎フレーム呼ばれる関数で、ここで周囲のスキャンや反応の更新、タイマーの管理などを行う
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

            // タイマー更新処理
            if (mMaterialObjectInfo.mIsDestructible) mDestroyTimer.Update(Time.deltaTime);
        }
        // FixedUpdateで物理的な移動ベクトルを算出して保存する関数
        private void FixedUpdate()
        {
            //現在の座標と前フレームの座標から移動ベクトルを算出
            Vector3 displacement = transform.position - mLastPosition;
            //移動ベクトルを経過時間（0.02秒など）で割り、1秒あたりの速度に変換
            mCurrentVelocity = displacement / Time.deltaTime;
            //次のフレームのために現在の座標を保存
            mLastPosition = transform.position;
        }
        // 周囲をスキャンして、接触している属性を更新する関数
        // これがこのクラスのコアな部分で、オブジェクトのコライダー形状に応じた判定方法で周囲を調べ、
        // 接触している属性を更新し、新しい属性があれば反応を開始する
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
                Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, transform.lossyScale) * mMaterialObjectInfo.mScanScale; 

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
                float radius = sphere.radius * maxScale * mMaterialObjectInfo.mScanScale;  // 1.1倍

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
            else
            {
                // MeshColliderなどの場合：
                float range = mRigidbody != null ?
                    Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) * mMaterialObjectInfo.mScanScale : 1.0f;

                hitCount = Physics.OverlapSphereNonAlloc(transform.position, range, mScanResults, mTargetObjectLayer);
            }

            mTotalContactElement = ElementType.None;

            for(int i = 0; i < hitCount; i++)
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
            ElementType newElements = mTotalContactElement & ~mCurrentElements;

            if (newElements != ElementType.None)
            {
                foreach (ElementType checkType in Enum.GetValues(typeof(ElementType)))
                {
                    if (checkType == ElementType.None) continue;
                    if ((newElements & checkType) == checkType)
                    {
                        if (mReactionTable.TryGetReaction(mMaterial, checkType, out MaterialToElementReactionResult result))
                        {
                            StartReactionProcess(result);
                            break;
                        }
                    }
                }
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
        // 反応結果を受け取って反応待ち状態を開始する関数
        private void StartReactionProcess(MaterialToElementReactionResult result)
        {
            // 反応待ち状態を開始
            mPendingReaction = result;
        }
        // 反応待ちがある場合、接触状況に応じて蓄熱・冷却を行う関数
        private void UpdatePendingReaction(bool isTouching)
        {
            //現状火だけの想定なので、火に触れているかで判断する
            //ここを全ての属性に対応させる場合は、引数でどの属性に触れているかも渡して、それぞれの属性ごとに蓄積量を管理する必要がある
            //木、草などの燃える素材は、火に触れているときは蓄熱、触れていないときは冷却する処理を行う
            if (mMaterial == MaterialType.Wood || mMaterial == MaterialType.Organism)
            {
                //火に触れている場合と触れていない場合で、蓄熱と冷却の処理を行う
                if (isTouching)
                {
                    // 火に触れている：蓄熱（秒間1.0溜まる）
                    float heatPower = 1.0f;
                    mCurrentHeatAccumulated += heatPower * Time.deltaTime;
                    if (mPreparationElementEffect == null)
                    {
                        mPreparationElementEffect = EffectManager.Instance.PlayEffect<ParticleSystem>("Smoke", transform.position, Quaternion.identity, Vector3.one, transform).GetComponent<EffectReturner>();
                    }
                }
                else
                {
                    // 火から離れた：冷却（秒間2.0で急速に冷める、あるいは 0f 代入で即リセット）
                    float coolingPower = 2.0f;
                    mCurrentHeatAccumulated -= coolingPower * Time.deltaTime;
                    mCurrentHeatAccumulated = Mathf.Max(0, mCurrentHeatAccumulated); // 0以下にはならない
                }

                // 発火判定
                if (mCurrentHeatAccumulated >= mMaterialObjectInfo.mIgnitionResistance)
                {
                    ApplyReaction(mPendingReaction);
                    mPendingReaction = default;
                    mCurrentHeatAccumulated = 0f;
                    if (mPreparationElementEffect != null)
                    {
                        mPreparationElementEffect.StopAndReturn();
                        mPreparationElementEffect = null;
                    }
                }

                // もし完全に冷めきったら、反応予約自体を消去
                if (!isTouching && mCurrentHeatAccumulated <= 0)
                {
                    mPendingReaction = default;
                    if (mPreparationElementEffect != null)
                    {
                        mPreparationElementEffect.StopAndReturn();
                        mPreparationElementEffect = null;
                    }
                }
            }
            //氷は火に触れているときは徐々に溶ける、触れていないときは何もしない処理を行う（例: 氷が火に触れると溶ける）
            else if (mMaterial == MaterialType.Ice)
            {
                if (isTouching)
                {
                    // 火力
                    float heatPower = 1.0f;

                    // 溶ける判定
                    //徐々にオブジェクトのサイズを小さくしていく
                    Vector3 scale = transform.localScale;
                    scale *= 1.0f - (heatPower * Time.deltaTime * 0.5f); // 徐々に小さくする
                    transform.localScale = scale;
                    mRigidbody.mass *= 1.0f - (heatPower * Time.deltaTime * 0.5f); // 質量も同様に減らす 
                }
                else
                {
                    // 火から離れた：冷却（秒間2.0で急速に冷める、あるいは 0f 代入で即リセット）
                    float coolingPower = 2.0f;
                    mCurrentHeatAccumulated -= coolingPower * Time.deltaTime;
                    mCurrentHeatAccumulated = Mathf.Max(0, mCurrentHeatAccumulated); // 0以下にはならない
                }

                // もし完全に溶ける(scale = 1)なら
                // 反応予約自体を消去
                //
                if (transform.localScale.magnitude <= 1)
                {
                    ApplyReaction(mPendingReaction);
                    mPendingReaction = default;
                    mCurrentHeatAccumulated = 0f;
                    if (mPreparationElementEffect != null)
                    {
                        mPreparationElementEffect.StopAndReturn();
                        mPreparationElementEffect = null;
                    }
                }
            }
        }

        // パーティクルシステムの形状をこのオブジェクトのコライダー形状に合わせる
        private void SetElement(ParticleSystem particle)
        {
            //Shapeモジュールを変数として取得
            var shapeModule = particle.shape;

            if (TryGetComponent<Collider>(out var col))
            {
                switch (col)
                {
                    case BoxCollider box:
                        shapeModule.shapeType = ParticleSystemShapeType.Box;
                        shapeModule.scale = Vector3.one; 
                        shapeModule.rotation = new Vector3(90.0f, 0, 0);

                        BoxCollider boxCollider = particle.gameObject.GetComponent<BoxCollider>();
                        if (boxCollider == null)
                        {
                            boxCollider = particle.gameObject.AddComponent<BoxCollider>();
                        }
                        else
                        {
                            boxCollider.enabled = true;
                        }
                        boxCollider.center = Vector3.zero;
                        boxCollider.size = box.size * 1.25f;
                        boxCollider.isTrigger = true;
                        break;
                    case SphereCollider sphere:
                        shapeModule.shapeType = ParticleSystemShapeType.Sphere;
                        shapeModule.scale = Vector3.one;
                        shapeModule.radius = sphere.radius;
                        shapeModule.rotation = new Vector3(90.0f, 0, 0);

                        SphereCollider sphereCollider = particle.gameObject.GetComponent<SphereCollider>();
                        if(sphereCollider == null)
                        {
                            sphereCollider = particle.gameObject.AddComponent<SphereCollider>();
                        }
                        else
                        {
                            sphereCollider.enabled = true;
                        }
                        sphereCollider.center = Vector3.zero;
                        sphereCollider.radius = sphere.radius * 1.1f;
                        sphereCollider.isTrigger = true;
                        break;
                    case CapsuleCollider capsule:
                        shapeModule.shapeType = ParticleSystemShapeType.Box;
                        shapeModule.scale = new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2);
                        shapeModule.rotation = new Vector3(90.0f,0,0);

                        CapsuleCollider capsuleCollider = particle.gameObject.GetComponent<CapsuleCollider>();
                        if(capsuleCollider == null)
                        {
                            capsuleCollider = particle.gameObject.AddComponent<CapsuleCollider>();
                        }
                        else
                        {
                            capsuleCollider.enabled = true;
                        }
                        capsuleCollider.center = Vector3.zero;
                        capsuleCollider.radius = capsule.radius;
                        capsuleCollider.height = capsule.height;
                        capsuleCollider.isTrigger = true;
                        capsuleCollider.direction = capsule.direction;
                        break;
                }
            }
        }

        //反応結果を適用する
        // ここで属性の追加・削除を行い、必要に応じてエフェクトを出し、燃え尽きる処理の開始なども行う
        private void ApplyReaction(MaterialToElementReactionResult result)
        {
            // 属性の追加・削除
            mCurrentElements |= result.gElementToAdd;
            mCurrentElements &= ~result.gElementToRemove;
            // 燃え尽きる処理の開始（もし木で、火がついたなら）
            if ((mMaterial == MaterialType.Wood || mMaterial == MaterialType.Organism) &&
                (result.gElementToAdd & ElementType.Fire) != 0)
            {
                // 反応によって新たに付与された属性を管理するための変数（例: 火がついている状態を管理）
                if (mGrantElement == null)
                {
                    Transform t = (mMaterial == MaterialType.Organism) ? GetComponentInChildren<FreeCameraTargetPoint>().transform : this.transform;
                    // 1. エフェクトを生成
                    EffectReturner effect = EffectManager.Instance.PlayEffect<ParticleSystem>(result.mEffectLabel, t.position, Quaternion.identity, Vector3.one, t).GetComponent<EffectReturner>();
                    // 2. エフェクトのGameObjectからChemistryElementを取得（なければ追加）
                    mGrantElement = effect.GetComponent<ChemistryElement>();
                    if (mGrantElement == null)
                    {
                        mGrantElement = effect.gameObject.AddComponent<ChemistryElement>();
                    }
                    // 3. 値をセット
                    mGrantElement.MyElementEffect = effect;
                    mGrantElement.ParentMaterial = this;

                    // 4. コライダー等のセットアップ
                    SetElement(effect.ParticleSystem);
                    //燃え上がるSE
                    SoundManager.Instance.PlayOneShot3D("Object_InFire", transform.position, transform);
                }

                if (mMaterialObjectInfo.mIsDestructible)
                {
                    mDestroyTimer.Start(mMaterialObjectInfo.mDestroyDelay);
                }
                else if(mMaterialObjectInfo.mIsExtinguishing)
                {
                    mGrantElement.ElementEraseTimer.Start(mMaterialObjectInfo.mFireResistance);
                }
            }
            else if (mMaterial == MaterialType.Ice && (result.gElementToAdd & ElementType.Fire) != 0)
            {
                //氷が火に触れたときの処理（例: 徐々に溶ける）
                //特にここでタイマーを開始する必要はないかもしれない（UpdatePendingReaction内でスケールを見て処理するため）
                if (mMaterialObjectInfo.mIsDestructible)
                {
                    mDestroyTimer.Start(0.5f);
                }
            }
        }

        //マテリアルが削除される時に呼び出すもの
        private void ProcessDestruction()
        {
            // ここで改めて今もエレメントがあるかチェック
            // （途中で水か何かで消火されていたら破壊しないため）
            if (mMaterial == MaterialType.Wood && (mCurrentElements & ElementType.Fire) != 0)
            {
                if (mGrantElement?.MyElementEffect != null)
                {
                    // オブジェクトが消えるので、エフェクトは切り離してその場に残す (引数なし = true)
                    mGrantElement.ProcessDestruction();
                    mGrantElement = null;
                }
                SoundManager.Instance.PlayOneShot3D("Object_OutFire", transform.position);
                Destroy(gameObject);
            }
            else if (mMaterial == MaterialType.Ice && (mCurrentElements & ElementType.Fire) != 0)
            {
                //氷が完全に溶けたときの処理
                if (mGrantElement?.MyElementEffect != null)
                {
                    mGrantElement.ProcessDestruction();
                    mGrantElement = null;
                }
                Destroy(gameObject);
            }
        }

        //エレメントだけ消す
        public void ProcessEraseElement()
        {
            if (mPreparationElementEffect != null)
            {
                // 予備エフェクトも同様
                mPreparationElementEffect.StopAndReturn(false);
                mPreparationElementEffect = null;
            }
            //現在の付与エレメントを初期化
            mCurrentElements = ElementType.None;
            //蓄積値も初期化
            mCurrentHeatAccumulated = 0;
            //付与エレメントもnull
            mGrantElement = null;
            mDestroyTimer.Reset();
            SoundManager.Instance.PlayOneShot3D("Object_OutFire", transform.position);
            AudioSource source = GetComponentInChildren<AudioSource>();
            if (source != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.ReturnAudioSource(source);
            }
        }

        //音を特定の条件下で再生するか調べる
        private void CheckPlaySound_Sword(Vector3 hitPoint)
        {
            //音の再生
            if (mMaterial == MaterialType.Wood)
            {
                SoundManager.Instance.PlayOneShot3D("Wood_Hit_Sword", transform.position);
            }
            else if (mMaterial == MaterialType.Stone)
            {
                SoundManager.Instance.PlayOneShot3D("Stone_Hit", transform.position);
            }
            else if (mMaterial == MaterialType.Iron)
            {
                SoundManager.Instance.PlayOneShot3D("Iron_Hit", transform.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //剣が当たったか調べる
            SwordStick swordStick = other.GetComponent<SwordStick>();
            if (swordStick != null)
            {
                if (transform.parent != swordStick.transform)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    ChemistryObject sword = swordStick.GetComponent<ChemistryObject>();
                    if (sword != null)
                    {
                        Vector3 force = sword.CurrentVelocity * mPhysicsMaterialInfo.CollisionForce;
                        AddBreakPower(force, hitPoint);
                        CheckPlaySound_Sword(hitPoint);
                    }
                }
            }
        }

        //破壊出来るか調べる
        public void AddBreakPower(Vector3 power, Vector3 hitPoint)
        {
            mRigidbody.AddForce(power, ForceMode.Impulse);
            EffectManager.Instance.PlayEffect<Transform>("Impact", hitPoint, Quaternion.identity, Vector3.one);
            if (mPhysicsMaterialInfo.mIsDestructible)
            {
                mHitPoint -= power.magnitude;
                if(mHitPoint <= 0)
                {
                    mHitPoint = 0;
                    Destroy(gameObject);
                }
            }
        }
        // オブジェクトが完全に破壊されるときのクリーンアップ処理
        private void OnDestroy()
        {
            if (mGrantElement?.MyElementEffect != null)
            {
                // オブジェクトが消えるので、エフェクトは切り離してその場に残す (引数なし = true)
                mGrantElement.StopElement();
            }
            if (mPreparationElementEffect != null)
            {
                mPreparationElementEffect.StopAndReturn(true);
                mPreparationElementEffect = null;
            }
            AudioSource[] source = GetComponentsInChildren<AudioSource>();
            for(int i = 0; i < source.Length; i++)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.ReturnAudioSource(source[i]);
                }
            }
        }
#if UNITY_EDITOR
        // Unityエディタ上でのみ実行される表示処理
        private void OnDrawGizmosSelected()
        {
            // 判定範囲を可視化するために色を設定（半透明の赤など）
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

            Collider myCollider = GetComponent<Collider>();
            if (myCollider == null) return;

            // PerformSurroundScan と同じ計算ロジックで形を描画
            if (myCollider is BoxCollider box)
            {
                // Boxの描画（回転を考慮）
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;
                // box.sizeを1.1倍にしたものを表示
                Gizmos.DrawCube(box.center, box.size * mMaterialObjectInfo.mScanScale);
                Gizmos.matrix = oldMatrix;
            }
            else if (myCollider is SphereCollider sphere)
            {
                // Sphereの描画
                Vector3 worldCenter = transform.TransformPoint(sphere.center);
                float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
                Gizmos.DrawSphere(worldCenter, sphere.radius * maxScale * mMaterialObjectInfo.mScanScale);
            }
            else if (myCollider is CapsuleCollider capsule)
            {
                // Capsuleの描画（簡易的に両端の球体と線で表示）
                GetCapsulePoints(capsule, out Vector3 p0, out Vector3 p1, out float radius);

                Gizmos.DrawWireSphere(p0, radius);
                Gizmos.DrawWireSphere(p1, radius);
                Gizmos.DrawLine(p0, p1);
                // 表面を覆うように表示したい場合はさらに工夫が必要ですが、これで十分位置はわかります
            }
        }
#endif
    }
}
