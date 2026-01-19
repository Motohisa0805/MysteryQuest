using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    public class EffectManager : MonoBehaviour
    {
        private static EffectManager mInstance;

        public static EffectManager Instance => mInstance;

        [SerializeField]
        private EffectTable mEffectTable;
        public EffectTable EffectTable => mEffectTable;

        // エフェクト名ごとにプールを格納する辞書
        private Dictionary<int, IObjectPool<ParticleSystem>> mPools = new Dictionary<int, IObjectPool<ParticleSystem>>();

        private CullingGroup mCullingGroup;
        private BoundingSphere[] mSpheres;
        private int mActiveCount = 0;
        private const int MAX_CULLING_OBJECTS = 500;

        // 稼働中のパーティクルリスト (配列操作用)
        private List<ParticleSystem> mActiveParticles = new List<ParticleSystem>(MAX_CULLING_OBJECTS);

        // 「パーティクルのInstanceID」から「リストのインデックス」を引く辞書 (高速な削除用)
        private Dictionary<int, int> mIdToIndex = new Dictionary<int, int>(MAX_CULLING_OBJECTS);

        [SerializeField]
        private Transform mCullingTarget; // プレイヤーなど
        [SerializeField]
        private float mCullDistance = 40.0f; // 表示限界距離

        private void Awake()
        {
            if(mInstance != null)
            {
                Destroy(gameObject);
                return;
            }
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeCullingGroup();
            InitializePools();
        }

        private void OnEnable()
        {
            // シーンがロードされた時のイベントに登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 破棄時にイベント解除（お作法）
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // シーンがロードされるたびに呼ばれる
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshCullingTarget();
        }

        private void RefreshCullingTarget()
        {
            // マネージャーが破棄されていたり、CullingGroupが未生成なら中断
            if (mCullingGroup == null) return;

            // 最新のメインカメラを探してセット
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mCullingGroup.targetCamera = mainCam;

                // ターゲットをプレイヤーにしたい場合はここで検索
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    mCullingTarget = player.transform;
                }
                else
                {
                    mCullingTarget = mainCam.transform;
                }

                mCullingGroup.SetDistanceReferencePoint(mCullingTarget);
            }
        }

        private void InitializeCullingGroup()
        {
            // 2. CullingGroup自体の作成 (これも一生に一度)
            mCullingGroup = new CullingGroup();
            mCullingGroup.SetBoundingDistances(new float[] { mCullDistance });
            mCullingGroup.onStateChanged = OnCullingStateChanged;
            // 配列を確保
            mSpheres = new BoundingSphere[MAX_CULLING_OBJECTS];
            mCullingGroup.SetBoundingSpheres(mSpheres);
            mCullingGroup.SetBoundingSphereCount(0);

            // 3. 【重要】最初のシーンのターゲットを設定するために1回呼ぶ
            RefreshCullingTarget();
        }

        private void InitializePools()
        {
            foreach(var element in mEffectTable.Effects)
            {
                // 各エフェクト専用のプールを作成
                var pool = new ObjectPool<ParticleSystem>(
                    createFunc: () => Instantiate(element.ParticleSystem, transform), // 足りない時に生成
                    // 取得時にカリング登録
                    actionOnGet: (obj) =>
                    {
                        obj.gameObject.SetActive(true);
                        // 取得時に念のためレンダラーをONにする
                        var rend = obj.GetComponent<ParticleSystemRenderer>();
                        if (rend != null)
                        {
                            rend.enabled = true;
                        }
                        RegisterCulling(obj);
                    },
                    //返却時にカリング解除
                    actionOnRelease: (obj) =>
                    {
                        UnregisterCulling(obj);
                        // 返却時に無効化
                        obj.gameObject.SetActive(false);
                    },  
                    actionOnDestroy: (obj) => Destroy(obj),                    // 溢れた時に破棄
                    collectionCheck: true,
                    defaultCapacity: element.DefaultCapacity,
                    maxSize: element.MaxSize
                );

                mPools.Add(element.ID, pool);

                // --- 事前生成処理 ---
                List<ParticleSystem> tempPrewarmList = new List<ParticleSystem>();

                for (int i = 0; i < element.DefaultCapacity; i++)
                {
                    var obj = pool.Get();

                    // Returnerに必要な情報を教えておく
                    if (obj.TryGetComponent<EffectReturner>(out var returner))
                    {
                        returner.Initialize(pool, transform);
                    }

                    tempPrewarmList.Add(obj);
                }

                // 生成が終わったらすべてプールに戻す
                // この時、actionOnReleaseによってSetActive(false)になる
                foreach (var obj in tempPrewarmList)
                {
                    pool.Release(obj);
                }
            }
        }

        private void Update()
        {
            // アクティブなパーティクルの位置情報をSphereに反映
            for (int i = 0; i < mActiveCount; i++)
            {
                // パーティクルが破棄されていた場合のガード
                if (mActiveParticles[i] != null)
                {
                    mSpheres[i].position = mActiveParticles[i].transform.position;
                    // 半径は見た目に合わせて調整（一律2.0fなど）
                    mSpheres[i].radius = 2.0f;
                }
            }

            // CullingGroupに更新通知
            mCullingGroup.SetBoundingSphereCount(mActiveCount);
        }

        private void RegisterCulling(ParticleSystem ps)
        {
            if (mActiveCount >= MAX_CULLING_OBJECTS) return;
            //リストの末尾に追加
            mActiveParticles.Add(ps);

            //IDとインデックスを紐づけ
            int index = mActiveCount;
            mIdToIndex[ps.GetInstanceID()] = index;
            mActiveCount++;
        }

        private void UnregisterCulling(ParticleSystem ps)
        {
            int id = ps.GetInstanceID();
            //登録されていない場合(エラー回避)
            if (!mIdToIndex.ContainsKey(id)) return;

            int indexToRemove = mIdToIndex[id];
            int lastIndex = mActiveCount - 1;
            //もし「削除対象」が「末尾」でなければ、末尾の要素を削除対象の位置に持ってくる
            if(indexToRemove != lastIndex)
            {
                ParticleSystem lastPs = mActiveParticles[lastIndex];

                //リストの穴埋め
                mActiveParticles[indexToRemove] = lastPs;

                //辞書の更新(末尾から移動してきたやつのIndexを更新)
                mIdToIndex[lastPs.GetInstanceID()] = indexToRemove;

                //Sphere情報の引継ぎ
                mSpheres[indexToRemove] = mSpheres[lastIndex];
            }
            // 末尾を削除 (Pop)
            mActiveParticles.RemoveAt(lastIndex);
            mIdToIndex.Remove(id);
            mActiveCount--;
        }

        // --- 距離判定イベント ---
        private void OnCullingStateChanged(CullingGroupEvent evt)
        {
            if (evt.index >= mActiveCount) return;

            ParticleSystem target = mActiveParticles[evt.index];
            if (target == null) return;

            // 距離バンド: 0=近い, 1=遠い
            bool isVisible = (evt.currentDistance == 0);

            // レンダラー(描画)のON/OFFを切り替える
            // これで「凍結したエフェクトがその場に残って見える」のを防ぐ
            var rend = target.GetComponent<ParticleSystemRenderer>();
            if (rend != null)
            {
                rend.enabled = isVisible;
            }


            // もし遠くにいて(isVisible=false)、かつ「すでに親から離脱している(単独で消えゆく火)」なら、
            if (!isVisible)
            {
                // 既存のEffectReturnerを取得して、親がいるか確認
                if (target.TryGetComponent<EffectReturner>(out var returner))
                {
                    // 親がマネージャー(mParent)に戻っている ＝ すでに本来の持ち主から切り離されている
                    // ※EffectReturnerにpublicなParent判定用プロパティがあると仮定
                    if (target.transform.parent == this.transform)
                    {
                        target.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (mCullingGroup != null)
            {
                mCullingGroup.Dispose();
                mCullingGroup = null;
            }
        }

        //取得型とvoidで併用できるようにTを使用
        //取得しない時：<T> = Transform
        //取得する時：<T> = ParticleSystem
        public T PlayEffect<T>(int effectID, Vector3 position, Quaternion rotation,Vector3 scale,Transform parent = null)
        {
            if (mPools.TryGetValue(effectID, out var pool))
            {
                var effect = pool.Get();
                if(parent != null)
                {
                    effect.transform.SetParent(parent);
                }
                effect.transform.position = position;
                effect.transform.rotation = rotation;
                if(scale.magnitude <= 0)
                {
                    effect.transform.localScale = Vector3.one;
                }
                else
                {
                    effect.transform.localScale = scale;
                }
                // 返却用のコンポーネントにプールを渡す
                if (effect.TryGetComponent<EffectReturner>(out var returner))
                {
                    returner.Initialize(pool,transform);
                }
                return effect.GetComponent<T>();
            }
            return default(T);
        }
    }
}
