using System;
using System.Collections.Generic;
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
            public bool mIsDestructible;
            public float mDestroyDelay;

            public bool mIsExtinguishing;
            [Header("燃えにくさ(熱容量)")]
            [Tooltip("この値まで熱が溜まると発火する(秒数換算)")]
            public float mIgnitionResistance; // 例: 草=0.1, 木箱=3.0, 大木=10.0
            [Header("火が消えるまでの消火量")]
            [Tooltip("この値が消火されるまでの時間(秒数換算)")]
            public float mFireResistance; // 例: 草=0.1, 木箱=3.0, 大木=10.0

        }

        // クラス内変数
        private float mCurrentHeatAccumulated = 0f; // 現在溜まっている熱
        private Timer mExtinguishingTimer = new Timer();

        [Header("この物体の素材(変わらない)")]
        [SerializeField]
        private MaterialType mMaterial;

        [Header("現在帯びている属性（変化する）")]
        [SerializeField]
        private ElementType mCurrentElements;
        public ElementType CurrentElements => mCurrentElements;

        private ChemistryTable mReactionTable;

        private Dictionary<ElementType,int> _contactCounters = new Dictionary<ElementType, int>();

        [SerializeField]
        private MaterialObjectInfo mMaterialObjectInfo;

        private ReactionResult mPendingReaction;

        //オブジェクト破壊までのタイマー
        private Timer mDestroyTimer = new Timer();


        private Rigidbody mRigidbody;

        private ParticleSystem mElementEffect;

        //スタート時にイベント登録
        private void Start()
        {
            mRigidbody = GetComponent<Rigidbody>();

            //化学反応テーブル取得(シングルトンからのアクセス頻度を減らすため)
            mReactionTable = GameSystemManager.Instance.ChemistryTable;
            //オブジェクト破壊タイマーにイベント登録
            if (mMaterialObjectInfo.mIsDestructible)
            {
                mDestroyTimer.OnEnd += ProcessDestruction;
            }
            if(mMaterialObjectInfo.mIsExtinguishing)
            {
                mExtinguishingTimer.OnEnd += ProcessDestructionEffect;
            }
        }

        private bool ExtinguishingCheck()
        {
            // 何らかの属性に触れているかチェック
            if (_contactCounters.Count == 0) return true;

            // 新しい反応を探す
            foreach (var kvp in _contactCounters)
            {
                ElementType element = kvp.Key;
                if(element == ElementType.Fire)
                {
                    return false;
                }
            }
            return true;
        }

        private void Update()
        {
            MonitorContactDuration();
            if (mMaterialObjectInfo.mIsDestructible)
            {
                mDestroyTimer.Update(Time.deltaTime);
            }
            if(mMaterialObjectInfo.mIsExtinguishing && ExtinguishingCheck())
            {
                mExtinguishingTimer.Update(Time.deltaTime);
            }
        }

        // 毎フレーム「接触中のエレメント」に対して反応を進める
        private void MonitorContactDuration()
        {
            // 進行中の反応があれば、タイマーを進める処理のみを行う（重要：ループ外で1回だけ実行）
            if (mPendingReaction.gElementToAdd != ElementType.None)
            {
                UpdatePendingReaction();
                return; // 反応中は他の新しい反応を探さない（ロック）
            }

            // 何らかの属性に触れているかチェック
            if (_contactCounters.Count == 0) return;

            // 新しい反応を探す
            foreach (var kvp in _contactCounters)
            {
                ElementType element = kvp.Key;

                // 属性を持っていないか
                if ((mCurrentElements & element) == 0)
                {
                    // 持っていない場合のみ
                    // 反応テーブル検索
                    if (mReactionTable.TryGetReaction(mMaterial, element, out ReactionResult result))
                    {
                        // 新しい反応を開始
                        StartReactionProcess(result);
                        break; // 1フレームに1つの反応だけを受け付ける
                    }
                }
            }
        }
        private void StartReactionProcess(ReactionResult result)
        {
            mPendingReaction = result;
        }

        private void UpdatePendingReaction()
        {
            //テストで1秒で1溜まる
            float heatPower = 1.0f;

            mCurrentHeatAccumulated += heatPower * Time.deltaTime;
            
            // 3. 発火判定
            // オブジェクトごとの「燃えにくさ」を超えたら発火
            if (mCurrentHeatAccumulated >= mMaterialObjectInfo.mIgnitionResistance)
            {
                ApplyReaction(mPendingReaction);

                // リセット
                mPendingReaction = default;
                mCurrentHeatAccumulated = 0f;
            }
        }


        private void SetParticleShapeToCollider(ParticleSystem particle)
        {
            // 1. Shapeモジュールを変数として取得
            var shapeModule = particle.shape;

            if (TryGetComponent<Collider>(out var col))
            {
                switch (col)
                {
                    case BoxCollider box:
                        shapeModule.shapeType = ParticleSystemShapeType.Box;
                        shapeModule.scale = box.size; // サイズも合わせるとより正確です
                        shapeModule.rotation = new Vector3(90.0f, 0, 0);
                        break;
                    case SphereCollider sphere:
                        shapeModule.shapeType = ParticleSystemShapeType.Sphere;
                        shapeModule.radius = sphere.radius;
                        shapeModule.rotation = new Vector3(90.0f, 0, 0);
                        break;
                    case CapsuleCollider capsule:
                        shapeModule.shapeType = ParticleSystemShapeType.Box;
                        shapeModule.scale = new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2);
                        shapeModule.rotation = new Vector3(90.0f,0,0);
                        break;
                }
            }
        }


        private void ApplyReaction(ReactionResult result)
        {
            mCurrentElements |= result.gElementToAdd;
            mCurrentElements &= ~result.gElementToRemove;

            if (GameSystemManager.Instance.EffectTable.TryGetReaction(mPendingReaction.mEffectType, out ParticleSystem effect))
            {
                Transform transform = this.transform;
                if(mMaterial == MaterialType.Organism)
                {
                    transform = GetComponentInChildren<FreeCameraTargetPoint>().transform;
                }
                mElementEffect = Instantiate(effect, transform.position, Quaternion.identity);
                mElementEffect.transform.SetParent(transform, false);
                mElementEffect.transform.localScale = Vector3.one;
                mElementEffect.transform.localPosition = Vector3.zero;
                mElementEffect.transform.localRotation = Quaternion.identity;
                SetParticleShapeToCollider(mElementEffect);
                SoundManager.Instance.PlayOneShot3D(3, transform,true, true,true, mMaterialObjectInfo.mDestroyDelay);
                // 燃え尽きる処理の開始（もし木で、火がついたなら）
                if ((mMaterial == MaterialType.Wood || mMaterial == MaterialType.Organism) &&
                    (result.gElementToAdd & ElementType.Fire) != 0)
                {
                    if (mMaterialObjectInfo.mIsDestructible)
                    {
                        mDestroyTimer.Start(mMaterialObjectInfo.mDestroyDelay);
                    }
                    else if(mMaterialObjectInfo.mIsExtinguishing)
                    {
                        mExtinguishingTimer.Start(mMaterialObjectInfo.mFireResistance);
                    }
                }
            }
        }

        //オブジェクトの物理的な処理
        private void ProcessDestruction()
        {
            // ここで改めて今もエレメントがあるかチェック
            // （途中で水か何かで消火されていたら破壊しないため）
            if (mMaterial == MaterialType.Wood && (mCurrentElements & ElementType.Fire) != 0)
            {
                Destroy(gameObject);
                // 必要なら燃え尽きエフェクトなどを生成
            }
        }

        private void ProcessDestructionEffect()
        {
            if(mMaterialObjectInfo.mIsExtinguishing)
            {
                Destroy(mElementEffect.gameObject);
                RemoveContact(CurrentElements);
                mCurrentElements = 0;
                mCurrentHeatAccumulated = 0;
                AudioSource source = GetComponentInChildren<AudioSource>();
                if (source != null && SoundManager.Instance != null)
                {
                    SoundManager.Instance.ReturnAudioSource(source);
                }
            }
        }

        private void OnDestroy()
        {
            AudioSource source = GetComponentInChildren<AudioSource>();
            if (source != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.ReturnAudioSource(source);
            }
        }

        //物理イベント：カウンターの増減のみを行う
        private void OnTriggerEnter(Collider other)
        {
            var elementComp = other.GetComponentInChildren<ChemistryElement>();
            if (elementComp != null)
            {
                AddContact(elementComp.Type);
            }
            var material = other.GetComponentInChildren<ChemistryObject>();
            if(material != null)
            {
                AddContact(material.CurrentElements);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var elementComp = other.GetComponentInChildren<ChemistryElement>();
            if (elementComp != null)
            {
                RemoveContact(elementComp.Type);
            }
            var material = other.GetComponentInChildren<ChemistryObject>();
            if (material != null)
            {
                RemoveContact(material.CurrentElements);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var elementComp = collision.collider.GetComponentInChildren<ChemistryElement>();
            if (elementComp != null)
            {
                AddContact(elementComp.Type);
            }
            var material = collision.collider.GetComponentInChildren<ChemistryObject>();
            if (material != null)
            {
                AddContact(material.CurrentElements);
            }
            //音の再生
            CheckPlaySound(collision);
        }
        //音を特定の条件下で再生するか調べる
        private void CheckPlaySound(Collision collision)
        {
            if(collision.impulse.magnitude <= 2.5f) { return; }
            //音の再生
            if (mMaterial == MaterialType.Wood)
            {
                SoundManager.Instance.PlayOneShot3D(6, transform);
            }
            else if (mMaterial == MaterialType.Stone)
            {
                SoundManager.Instance.PlayOneShot3D(5, transform);
            }
            else if (mMaterial == MaterialType.Iron)
            {

            }
        }

        // カウンター管理用メソッド
        private void AddContact(ElementType type)
        {
            if (!_contactCounters.ContainsKey(type))
            {
                _contactCounters[type] = 0;
            }
            _contactCounters[type]++;
        }

        private void RemoveContact(ElementType type)
        {
            if (_contactCounters.ContainsKey(type))
            {
                _contactCounters[type]--;

                // 0になったら接触終了
                if (_contactCounters[type] <= 0)
                {
                    _contactCounters.Remove(type);

                    // もし「着火待ち」だった属性から離れたなら、タイマーリセット
                    // （ここを工夫すると「火から離れても少し熱が残る」なども表現可能）
                    if (mPendingReaction.gElementToAdd != ElementType.None)
                    {
                        // 本当は「今の反応のトリガーになった属性か」を厳密に見るべきですが
                        // 簡易的にリセットします
                        mPendingReaction = default;
                        mCurrentHeatAccumulated = 0;
                    }
                }
            }
        }
    }
}
