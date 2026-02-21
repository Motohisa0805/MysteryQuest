using UnityEngine;
using UnityEngine.Pool;

namespace MyAssets
{
    public class EffectReturner : MonoBehaviour
    {
        private IObjectPool<ParticleSystem> mPool;
        private ParticleSystem mParticleSystem;
        private Transform mParent;
        public Transform Parent => mParent;
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
        public void StopAndReturn(bool detachImmediately = true)
        {
            if (mParent == null || transform == null) { return; }
            Component[] components = GetComponents<Component>();
            foreach (var component in components)
            {
                if(component is BoxCollider box)
                {
                    box.enabled = false;
                }
                if (component is SphereCollider sphere)
                {
                    sphere.enabled = false;
                }
                if (component is CapsuleCollider capsule)
                {
                    capsule.enabled = false;
                }
            }

            mParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (detachImmediately)
            {
                // 今すぐ親をManagerに戻す（置き去りにする）
                transform.SetParent(mParent);
            }
            mParent = null;
            // falseの場合は、OnParticleSystemStoppedが呼ばれるまで親（燃えている箱など）にくっついたまま
        }
        private void OnParticleSystemStopped()
        {
            if (mPool != null)
            {
                // 返却前に親をManager（実家）に戻す
                //transform.SetParent(mParent);
                mPool.Release(gameObject.GetComponent<ParticleSystem>());
            }
        }
    }
}

