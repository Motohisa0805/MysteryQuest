using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.ParticleSystem;

namespace MyAssets
{
    public class EffectReturner : MonoBehaviour
    {
        private IObjectPool<ParticleSystem> mPool;
        private ParticleSystem mParticleSystem;
        private Transform mParent;
        public ParticleSystem ParticleSystem => mParticleSystem;

        private void Awake()
        {
            mParticleSystem = GetComponent<ParticleSystem>();

            var main = mParticleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
        public void Initialize(IObjectPool<ParticleSystem> pool, Transform originParent)
        {
            mPool = pool;
            mParent = originParent;
        }
        // 手動で止めたい時に呼ぶメソッド
        public void StopAndReturn()
        {
            // ループエフェクトを安全に停止させる
            // withChildren: true にすると子要素のパーティクルも一緒に止まる
            mParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            transform.SetParent(mParent);
        }
        private void OnParticleSystemStopped()
        {
            if (mPool != null)
            {
                // 返却前に親をManager（実家）に戻す
                transform.SetParent(mParent);
                mPool.Release(gameObject.GetComponent<ParticleSystem>());
            }
        }
    }
}

