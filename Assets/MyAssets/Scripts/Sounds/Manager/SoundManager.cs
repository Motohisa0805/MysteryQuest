using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance;
        public static SoundManager  Instance => instance;

        [SerializeField]
        private SoundList           mSoundList;
        [SerializeField]
        private SoundList           mBGMSoundList;
        //初期化時のオーディオソース数
        private int                 mInitSoundIndex = 10;

        private int                 mMaxAudioIndex = 50;

        private List<AudioSource>   mAudioObjects = new List<AudioSource>();

        [SerializeField]
        private GameObject          mAudioSourcePrefab;

        private AudioSource         mPlayingBGMAudioSource;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            for (int i = 0; i < mInitSoundIndex;i++)
            {
                if(mAudioSourcePrefab == null)
                {
                    Debug.LogError("AudioSourcePrefab is null in SoundManager");
                    return;
                }
                GameObject obj = Instantiate(mAudioSourcePrefab, transform);
                obj.AddComponent<AudioSource>();
                obj.name = "SoundSorce" + i.ToString();
                obj.SetActive(false);
                mAudioObjects.Add(obj.GetComponent<AudioSource>());
            }
        }

        private AudioSource CreateAudioSource()
        {
            if (mAudioObjects.Count > mMaxAudioIndex) { return null; }
            GameObject obj = Instantiate(mAudioSourcePrefab, transform);
            obj.AddComponent<AudioSource>();
            mAudioObjects.Add(obj.GetComponent<AudioSource>());
            obj.name = "SoundSorce" + mAudioObjects.Count.ToString();
            obj.SetActive(false);
            return obj.GetComponent<AudioSource>();
        }

        private AudioSource SerchAudios()
        {
            for(int i = 0; i < mAudioObjects.Count;i++)
            {
                //オーディオソースが有効ならコンテニュー
                if (mAudioObjects[i].gameObject.activeSelf)
                {
                    continue;
                }
                return mAudioObjects[i];
            }
            //全て有効中なら新しくオーディオソースを追加する
            AudioSource audioSource = CreateAudioSource();
            return audioSource;
        }

        public void PlayBGM(int id, bool loop = true)
        {
            //クリップを取得
            SoundList.SEElement bgmElement = mBGMSoundList.GetElement(id);
            AudioClip clip = bgmElement.Clips[Random.Range(0, bgmElement.MaxClips)];
            AudioSource audioSource = null;
            if (mPlayingBGMAudioSource != null)
            {
                audioSource = mPlayingBGMAudioSource;
            }
            else
            {
                //オーディオソースを探す
                audioSource = SerchAudios();
            }
            if (audioSource == null || clip == null)
            {
                return;
            }
            audioSource.clip = clip;
            //オーディオソースを有効に
            audioSource.gameObject.SetActive(true);
            audioSource.loop = loop;
            audioSource.volume = bgmElement.volume;
            audioSource.spatialBlend = 0.0f;
            audioSource.maxDistance = bgmElement.MaxDistance;
            audioSource.minDistance = bgmElement.MinDistance;
            audioSource.Play();
            mPlayingBGMAudioSource = audioSource;
        }

        public void UnPauseStart()
        {
            if(mPlayingBGMAudioSource == null) 
            {
                Debug.LogWarning("Not Find PlayingBGMAudioSource");
                return; 
            }
            mPlayingBGMAudioSource.UnPause();
        }

        public void PauseBGM()
        {
            if (mPlayingBGMAudioSource == null)
            {
                Debug.LogWarning("Not Find PlayingBGMAudioSource");
                return;
            }
            mPlayingBGMAudioSource.Pause();
        }


        public void PlayOneShot3D(int id,Transform parent,bool loop = false, bool isFollow = false,bool destroyCollection = false, float endSECount = -1)
        {
            //クリップを取得
            SoundList.SEElement seElement = mSoundList.GetElement(id); ;
            AudioClip clip = seElement.Clips[Random.Range(0, seElement.MaxClips)];
            //オーディオソースを探す
            AudioSource audioSource = SerchAudios();
            if(audioSource == null || clip == null)
            {
                return;
            }
            audioSource.enabled = true;
            //オーディオソースを有効に
            audioSource.gameObject.SetActive(true);
            audioSource.loop = loop;
            audioSource.spatialBlend = 1.0f;
            audioSource.maxDistance = seElement.MaxDistance;
            audioSource.minDistance = seElement.MinDistance;

            if (isFollow)
            {
                audioSource.transform.SetParent(parent);
                audioSource.transform.localPosition = Vector3.zero;
            }
            else
            {
                audioSource.transform.SetParent(transform); // SoundManagerの子に戻す
                audioSource.transform.position = parent.position;
            }

            audioSource.PlayOneShot(clip);
            if(!destroyCollection)
            {
                // 再生終了後に回収する仕組みが必要
                if(endSECount < 0)
                {
                    StartCoroutine(ReturnToPool(audioSource, clip.length));
                }
                else
                {
                    StartCoroutine(ReturnToPool(audioSource, endSECount));
                }
            }
        }

        public void PlayOneShot2D(int id, bool loop = false, float endSECount = -1,float pitch = 1)
        {
            //クリップを取得
            SoundList.SEElement seElement = mSoundList.GetElement(id);
            AudioClip clip = seElement.Clips[Random.Range(0, seElement.MaxClips)];
            //オーディオソースを探す
            AudioSource audioSource = SerchAudios();
            if (audioSource == null || clip == null)
            {
                return;
            }
            //オーディオソースを有効に
            audioSource.gameObject.SetActive(true);
            audioSource.loop = loop;
            audioSource.pitch = pitch;
            audioSource.spatialBlend = 0.0f;
            audioSource.maxDistance = seElement.MaxDistance;
            audioSource.minDistance = seElement.MinDistance;




            audioSource.PlayOneShot(clip);
            StartCoroutine(ReturnToPool(audioSource, clip.length));
        }

        private IEnumerator ReturnToPool(AudioSource source, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            source.Stop(); // 念のため
            source.transform.SetParent(transform); // 親を戻す
            source.gameObject.SetActive(false);
        }

        public void ReturnAudioSource(AudioSource source)
        {
            source.Stop(); // 念のため
            source.transform.SetParent(transform); // 親を戻す
            source.gameObject.SetActive(false);
        }



    }
}
