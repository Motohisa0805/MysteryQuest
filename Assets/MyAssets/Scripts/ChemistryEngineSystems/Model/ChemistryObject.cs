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

        private HashSet<ChemistryObject> _touchingObjects = new HashSet<ChemistryObject>();
        private HashSet<ChemistryElement> _touchingElements = new HashSet<ChemistryElement>();

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



        private void Update()
        {
            MonitorContactDuration();
            if (mMaterialObjectInfo.mIsDestructible)
            {
                mDestroyTimer.Update(Time.deltaTime);
            }
            if(mMaterialObjectInfo.mIsExtinguishing)
            {
                mExtinguishingTimer.Update(Time.deltaTime);
                if (mCurrentElements != ElementType.None)
                {
                    if (mExtinguishingTimer.IsEnd())
                    {
                        ProcessDestructionEffect();
                    }
                }
            }
        }

        // 毎フレーム「接触中のエレメント」に対して反応を進める
        private void MonitorContactDuration()
        {
            // 進行中の反応があれば早期リターン
            if (mPendingReaction.gElementToAdd != ElementType.None)
            {
                UpdatePendingReaction();
                return;
            }

            //今触れているもの全ての属性を合成する
            ElementType totalContactElement = ElementType.None;

            // 化学オブジェクト（相手も変化するもの）
            // 死んだオブジェクトはRemove
            _touchingObjects.RemoveWhere(obj => obj == null); 

            foreach (var obj in _touchingObjects)
            {
                // 自分自身の属性は拾わないようにガード（念の為）
                if (obj == this) continue;
                totalContactElement |= obj.CurrentElements;
            }

            // 化学エレメント（固定のトリガー）
            _touchingObjects.RemoveWhere(obj =>
            {
                // オブジェクトが破棄されていたら消す
                if (obj == null) return true;

                // 追加：ルートが同じ（身内）になっていたらリストから消す
                // これを入れることで、親子関係になった瞬間に接触リストから外れます
                if (obj.transform.root == transform.root) return true;

                // 自分自身なら消す
                if (obj == this) return true;

                // それ以外ならリストに残す
                return false;
            });

            foreach (var elm in _touchingElements)
            {
                totalContactElement |= elm.Type;
            }

            // 2. 合計した属性に対して反応判定を行う
            // （何も触れていなければ totalContactElement は None になり、反応しない）
            if (totalContactElement == ElementType.None) return;

            // 自分が持っていない属性が含まれているかチェック
            // （ビット演算： (相手の属性 & ~自分の属性) が 0 でなければ、未知の属性がある）
            ElementType newElements = totalContactElement & ~mCurrentElements;

            if (newElements != ElementType.None)
            {
                // 優先順位などがあればここでビットを解析するが、
                // とりあえず見つかった順、あるいはテーブルにあるか順で判定

                // ここでは単純にビットが立っているものを1つずつ調べる例
                foreach (ElementType checkType in Enum.GetValues(typeof(ElementType)))
                {
                    if (checkType == ElementType.None) continue;

                    if ((newElements & checkType) == checkType)
                    {
                        if (mReactionTable.TryGetReaction(mMaterial, checkType, out ReactionResult result))
                        {
                            StartReactionProcess(result);
                            break; // 1フレーム1反応
                        }
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
            // 自分自身や子供を拾わないためのガード
            if (other.transform.root == transform.root) return;

            var elementComp = other.GetComponentInChildren<ChemistryElement>();
            if (elementComp != null)
            {
                _touchingElements.Add(elementComp);
            }

            // GetComponentsInChildren に変えて、複数取得＆自分除外を徹底する
            var materials = other.GetComponentsInChildren<ChemistryObject>();
            foreach (var mat in materials)
            {
                if (mat == this) continue;
                _touchingObjects.Add(mat);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // 退出時もルートチェック（入る時弾いたものは出る時も無視）
            if (other.transform.root == transform.root) return;

            var elementComp = other.GetComponentInChildren<ChemistryElement>();
            if (elementComp != null)
            {
                _touchingElements.Remove(elementComp);
                ResetReactionIfEmpty(); // 接触がゼロになったか確認
            }

            var materials = other.GetComponentsInChildren<ChemistryObject>();
            foreach (var mat in materials)
            {
                if (mat == this) continue;
                _touchingObjects.Remove(mat);
                ResetReactionIfEmpty();
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            
            // 自分自身や子供を拾わないためのガード
            if (collision.transform.root == transform.root) return;

            var elementComp = collision.collider.GetComponentInChildren<ChemistryElement>();
            if (elementComp != null)
            {
                _touchingElements.Add(elementComp);
            }

            // GetComponentsInChildren に変えて、複数取得＆自分除外を徹底する
            var materials = collision.collider.GetComponentsInChildren<ChemistryObject>();
            foreach (var mat in materials)
            {
                if (mat == this) continue;
                _touchingObjects.Add(mat);
            }
            //音の再生
            CheckPlaySound(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            // 退出時もルートチェック（入る時弾いたものは出る時も無視）
            if (collision.transform.root == transform.root) return;

            var elementComp = collision.collider.GetComponentInChildren<ChemistryElement>();
            if (elementComp != null)
            {
                _touchingElements.Remove(elementComp);
                ResetReactionIfEmpty(); // 接触がゼロになったか確認
            }

            var materials = collision.collider.GetComponentsInChildren<ChemistryObject>();
            foreach (var mat in materials)
            {
                if (mat == this) continue;
                _touchingObjects.Remove(mat);
                ResetReactionIfEmpty();
            }
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

        // 全て離れた時にタイマーをリセットする処理
        private void ResetReactionIfEmpty()
        {
            if (_touchingElements.Count == 0 && _touchingObjects.Count == 0)
            {
                // 何にも触れていないなら反応進行をリセット
                if (mPendingReaction.gElementToAdd != ElementType.None)
                {
                    mPendingReaction = default;
                    mCurrentHeatAccumulated = 0;
                }
            }
        }
    }
}
