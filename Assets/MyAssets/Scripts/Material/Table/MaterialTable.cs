using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [CreateAssetMenu(menuName = "Material/MaterialTable")]
    public class MaterialTable : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Materialリスト")]
        [SerializeField]
        private List<MaterialElement> mMaterials = new List<MaterialElement>();

        public List<MaterialElement> Materials => mMaterials;

        //検索を高速化するための辞書(実行時に構築)
        private Dictionary<int, MaterialElement> mMaterialDic = new Dictionary<int, MaterialElement>();

        public void Initialize()
        {
            mMaterialDic = new Dictionary<int, MaterialElement>();
            foreach (var material in mMaterials)
            {
                var key = material.ID;
                if (!mMaterialDic.ContainsKey(key))
                {
                    mMaterialDic.Add(key, material);
                }
            }
        }

        //検索機能
        public bool TryGetReaction(int id, out MaterialElement result)
        {
            if (mMaterialDic == null) Initialize();

            return mMaterialDic.TryGetValue(id, out result);
        }

        public MaterialElement GetEffect(int id)
        {
            if (mMaterialDic.TryGetValue(id, out var element))
            {
                return element;
            }
            Debug.LogWarning($"Effect ID {id} が見つかりません。");
            return null;
        }

        // --- ISerializationCallbackReceiver の実装 ---
        // ScriptableObjectが読み込まれた際にDictionaryを構築する
        public void OnAfterDeserialize()
        {
            mMaterialDic.Clear();
            foreach (var element in mMaterials)
            {
                if (!mMaterialDic.ContainsKey(element.ID))
                {
                    mMaterialDic.Add(element.ID, element);
                }
            }
        }

        public void OnBeforeSerialize() { /* 何もしない */ }
    }
}
