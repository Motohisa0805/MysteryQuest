using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{

    [CreateAssetMenu(menuName = "Effect/EffectTable")]
    public class EffectTable : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("エフェクトリスト")]
        [SerializeField]
        private List<EffectElement> mEffects = new List<EffectElement>();

        public List<EffectElement> Effects => mEffects;

        //検索を高速化するための辞書(実行時に構築)
        private Dictionary<int, EffectElement> mEffectDic = new Dictionary<int, EffectElement>();

        public void Initialize()
        {
            mEffectDic = new Dictionary<int, EffectElement>();
            foreach (var effect in mEffects)
            {
                var key = effect.ID;
                if (!mEffectDic.ContainsKey(key))
                {
                    mEffectDic.Add(key, effect);
                }
            }
        }

        //検索機能
        public bool TryGetReaction(int id, out EffectElement result)
        {
            if (mEffectDic == null) Initialize();

            return mEffectDic.TryGetValue(id, out result);
        }

        public EffectElement GetEffect(int id)
        {
            if(mEffectDic.TryGetValue(id, out var element))
            {
                return element;
            }
            Debug.LogWarning($"Effect ID {id} が見つかりません。");
            return null;   
        }

        public EffectElement GetEffect(string label)
        {
            return GetEffect(Animator.StringToHash(label));
        }

        // --- Unity Editor用自動処理 ---

        // インスペクターで値が変更された時に呼ばれる（エディタ専用）
        private void OnValidate()
        {
            // リスト内のすべての要素に対してIDを再計算
            foreach (var element in mEffects)
            {
                element.GenerateID();
            }
        }

        // --- ISerializationCallbackReceiver の実装 ---
        // ScriptableObjectが読み込まれた際にDictionaryを構築する
        public void OnAfterDeserialize()
        {
            mEffectDic.Clear();
            foreach (var element in mEffects)
            {
                if (!mEffectDic.ContainsKey(element.ID))
                {
                    mEffectDic.Add(element.ID, element);
                }
            }
        }

        public void OnBeforeSerialize() { /* 何もしない */ }
    }
}
