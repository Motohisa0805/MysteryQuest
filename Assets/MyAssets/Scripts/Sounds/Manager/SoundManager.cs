using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace MyAssets
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance;
        public static SoundManager  Instance => instance;

        [SerializeField]
        private SoundList           mSoundList;
        //初期化時のオーディオソース数
        private int                 mInitSoundIndex = 10;

        private int                 mMaxAudioIndex = 20;

        private List<AudioSource>     mAudioObjects = new List<AudioSource>();
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
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<AudioSource>();
            for (int i = 0; i < mInitSoundIndex;i++)
            {
                GameObject obj = Instantiate(gameObject, transform);
                obj.name = "SoundSorce" + i.ToString();
                obj.SetActive(false);
                mAudioObjects.Add(obj.GetComponent<AudioSource>());
            }
        }

        private AudioSource CreateAudioSource()
        {
            if (mAudioObjects.Count > mMaxAudioIndex) { return null; }
            GameObject obj = Instantiate(gameObject, transform);
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


        public void PlayOneShot3D(SoundList.SEType SEType,Transform parent,bool loop = false, bool isFollow = false,float endSECount = -1)
        {
            //クリップを取得
            SoundList.SEElement seElement = mSoundList.SEList[(int)SEType];
            AudioClip clip = seElement.Clips[Random.Range(0, mSoundList.SEList[(int)SEType].MaxClips)];
            //オーディオソースを探す
            AudioSource audioSource = SerchAudios();
            if(audioSource == null || clip == null)
            {
                return;
            }
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

        private IEnumerator ReturnToPool(AudioSource source, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            source.Stop(); // 念のため
            source.transform.SetParent(transform); // 親を戻す
            source.gameObject.SetActive(false);
        }
    }
}
