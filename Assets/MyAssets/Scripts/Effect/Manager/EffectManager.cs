using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

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
            InitializePools();
        }

        private void InitializePools()
        {
            foreach(var element in mEffectTable.Effects)
            {
                // 各エフェクト専用のプールを作成
                var pool = new ObjectPool<ParticleSystem>(
                    createFunc: () => Instantiate(element.ParticleSystem, transform), // 足りない時に生成
                    actionOnGet: (obj) => obj.gameObject.SetActive(true),                 // 使う時に有効化
                    actionOnRelease: (obj) => obj.gameObject.SetActive(false),             // 返却時に無効化
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
