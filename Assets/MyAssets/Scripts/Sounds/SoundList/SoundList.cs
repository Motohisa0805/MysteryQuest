using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "Sound Objects/SoundList")]
public class SoundList : ScriptableObject, ISerializationCallbackReceiver
{

    [System.Serializable]
    public class SEElement
    {
        [SerializeField] 
        private int mID; // Enumの代わりの数値ID
        public int ID => mID;
        [SerializeField]
        private string mLabel;
        public string Label => mLabel;
        [SerializeField]
        private AudioClip[] mClips;
        public AudioClip[] Clips => mClips;
        public int MaxClips => mClips.Length;
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

    // --- ISerializationCallbackReceiver の実装 ---
    // ScriptableObjectが読み込まれた際にDictionaryを構築する
    public void OnAfterDeserialize()
    {
        mSEDic.Clear();
        foreach (var element in mSEList)
        {
            if (!mSEDic.ContainsKey(element.ID))
            {
                mSEDic.Add(element.ID, element);
            }
        }
    }

    public void OnBeforeSerialize() { /* 何もしない */ }
}
