using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "Sound Objects/SoundList")]
public class SoundList : ScriptableObject
{
    //ABS‡
    public enum SEType
    {
        Attack,
        Damage,
        Fire,
        Footstep,
        CountDown,
    }

    [System.Serializable]
    public class SEElement
    {
        [SerializeField]
        private SEType mType;
        public SEType Type => mType;
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

    public SEElement GetElement(SEType type)
    {
        return SEList.Find(x => x.Type == type);
    }
}
