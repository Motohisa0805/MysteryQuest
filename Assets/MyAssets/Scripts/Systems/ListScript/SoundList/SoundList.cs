using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "Sound Objects/SoundList")]
public class SoundList : ScriptableObject, ISerializationCallbackReceiver
{

    [System.Serializable]
    public class SEElement
    {
        [SerializeField]
        [Tooltip("ラベルから自動生成される固有ID（編集不要）")]
        private int mID; 
        public int ID => mID;
        [SerializeField]
        private string mLabel;
        public string Label => mLabel;
        [SerializeField]
        private AudioClip[] mClips;
        public AudioClip[] Clips => mClips;
        public int MaxClips => (mClips != null) ? mClips.Length : 0;
        [Range(0, 1)] 
        public float volume = 1f;
        [Range(0, 2)] 
        public float pitchRange = 0.1f;
        [Range(0, 1)]
        public float mSpatialBlend;
        [SerializeField]
        private float mMinDistance = 1f;
        public float MinDistance => mMinDistance;
        [SerializeField]
        private float mMaxDistance = 10f;
        public float MaxDistance => mMaxDistance;

        //IDを自動生成するメソッド
        public void GenerateID()
        {
            if(string.IsNullOrEmpty(mLabel))
            {
                mID = 0;
            }
            else
            {
                mID = Animator.StringToHash(mLabel);
            }
        }
    }
    [SerializeField]
    private List<SEElement> mSEList = new List<SEElement>();
    public List<SEElement> SEList => mSEList;

    // 実行時に高速検索するための辞書
    private Dictionary<int,SEElement> mSEDic = new Dictionary<int,SEElement>();

    // 辞書経由で取得（intで呼び出し）
    public SEElement GetElement(int id)
    {
        if (mSEDic.TryGetValue(id, out var element))
        {
            return element;
        }
        Debug.LogWarning($"Sound ID {id} が見つかりません。");
        return null;
    }
    public SEElement GetElement(string label)
    {
        return GetElement(Animator.StringToHash(label));
    }

    // --- Unity Editor用自動処理 ---

    // インスペクターで値が変更された時に呼ばれる（エディタ専用）
    private void OnValidate()
    {
        // リスト内のすべての要素に対してIDを再計算
        foreach (var element in mSEList)
        {
            element.GenerateID();
        }
    }

    // --- ISerializationCallbackReceiver の実装 ---
    // ScriptableObjectが読み込まれた際にDictionaryを構築する
    public void OnAfterDeserialize()
    {
        mSEDic.Clear();
        foreach (var element in mSEList)
        {
            // IDが0（名前が空）の場合は登録しない
            if (element.ID == 0) continue;

            if (!mSEDic.ContainsKey(element.ID))
            {
                mSEDic.Add(element.ID, element);
            }
            else
            {
                // 同じ名前（ハッシュ値）が存在する場合の警告
                Debug.LogWarning($"SoundList: 重複したラベルがあります: {element.Label}");
            }
        }
    }

    public void OnBeforeSerialize() { /* 何もしない */ }
}
