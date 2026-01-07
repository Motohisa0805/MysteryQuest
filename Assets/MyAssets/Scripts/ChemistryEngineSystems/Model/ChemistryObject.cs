using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{

    //疑似化学エンジンのオブジェクトスクリプトファイル
    public class ChemistryObject : MonoBehaviour
    {
        [Serializable]
        public struct MaterialObjectInfo
        {
            public bool mIsDestructible;
            public float mDestroyDelay;
        }

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

        //反応確定までのタイマー
        private Timer mReadyElementTimer = new Timer();
        //オブジェクト破壊までのタイマー
        private Timer mDestroyTimer = new Timer();


        //スタート時にイベント登録
        private void Start()
        {
            //化学反応テーブル取得(シングルトンからのアクセス頻度を減らすため)
            mReactionTable = GameSystemManager.Instance.ChemistryTable;
            //オブジェクト破壊タイマーにイベント登録
            mDestroyTimer.OnEnd += ProcessDestruction;
        }

        private void Update()
        {
            MonitorContactDuration();
            mDestroyTimer.Update(Time.deltaTime);
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

                // 自分はまだその属性を持っていないか？
                if ((mCurrentElements & element) == 0) // 持っていない場合のみ
                {
                    // 反応テーブル検索
                    if (mReactionTable.TryGetReaction(mMaterial, element, out ReactionResult result))
                    {
                        // 新しい反応を開始
                        StartReactionProcess(result);
                        break; // 1フレームに1つの反応だけを受け付ける（優先度バグ防止）
                    }
                }
            }
        }
        private void StartReactionProcess(ReactionResult result)
        {
            mPendingReaction = result;
            mReadyElementTimer.Start(result.mDuration);
        }

        private void UpdatePendingReaction()
        {
            // 注意: ここで「トリガーとなった属性から離れたか？」のチェックが必要だが
            // 簡易的に RemoveContact でリセットされる前提とする

            mReadyElementTimer.Update(Time.deltaTime);

            if (mReadyElementTimer.IsEnd())
            {
                ApplyReaction(mPendingReaction);

                // 反応完了したのでリセット
                mPendingReaction = default;
                mReadyElementTimer.Reset();
            }
        }


        private void ApplyReaction(ReactionResult result)
        {
            mCurrentElements |= result.gElementToAdd;
            mCurrentElements &= ~result.gElementToRemove;

            if (GameSystemManager.Instance.EffectTable.TryGetReaction(mPendingReaction.mEffectType, out ParticleSystem effect))
            {
                ParticleSystem obj = Instantiate(effect, transform.position, Quaternion.identity);
                obj.transform.SetParent(transform, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                SoundManager.Instance.PlayOneShot3D(SoundList.SEType.Fire, transform,true, true,true, mMaterialObjectInfo.mDestroyDelay);
                // 燃え尽きる処理の開始（もし木で、火がついたなら）
                if (mMaterial == MaterialType.Wood && (result.gElementToAdd & ElementType.Fire) != 0)
                {
                    if (mMaterialObjectInfo.mIsDestructible)
                    {
                        mDestroyTimer.Start(mMaterialObjectInfo.mDestroyDelay);
                    }
                }
            }
        }

        //オブジェクトの物理的な処理
        private void ProcessDestruction()
        {
            // ここで改めて「今も燃えているか？」チェック
            // （途中で水がかかって消火されていたら破壊しないため）
            if (mMaterial == MaterialType.Wood && (mCurrentElements & ElementType.Fire) != 0)
            {
                Destroy(gameObject);
                // 必要なら燃え尽きエフェクトなどを生成
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
                        mReadyElementTimer.Reset();
                    }
                }
            }
        }
    }
}
