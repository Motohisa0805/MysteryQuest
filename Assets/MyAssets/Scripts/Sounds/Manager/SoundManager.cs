using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyAssets
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager mInstance;
        public static SoundManager  Instance => mInstance;

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

        //CullingGroup関連の変数
        private CullingGroup        mCullingGroup;
        private BoundingSphere[]    mSpheres;
        // プレイヤーやカメラなど、距離判定の中心
        [Header("Culling Settings")]
        [SerializeField]
        private Transform           mListenerTarget;
        // これ以上離れたら音を止める距離
        [SerializeField]
        private float               mCullDistance = 10.0f;

        // 開始0.2秒間は音を無視する
        [SerializeField]
        private float mMuteDurationAtStart = 0.2f; 

        private void Awake()
        {
            if (mInstance != null)
            {
                Destroy(gameObject);
                return;
            }
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // 2. CullingGroup自体の作成 (これも一生に一度)
            mCullingGroup = new CullingGroup();
            mCullingGroup.SetBoundingDistances(new float[] { mCullDistance });
            mCullingGroup.onStateChanged = OnCullingStateChanged;
            // 配列を確保
            mSpheres = new BoundingSphere[mMaxAudioIndex];
            mCullingGroup.SetBoundingSpheres(mSpheres);
            mCullingGroup.SetBoundingSphereCount(0);

            // 最初のシーンのターゲットを設定するために1回呼ぶ
            RefreshCullingTarget();


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

        private void OnEnable()
        {
            // シーンがロードされた時のイベントに登録
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 破棄時にイベント解除（お作法）
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // シーンがロードされるたびに呼ばれる
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshCullingTarget();
        }

        private void RefreshCullingTarget()
        {
            // マネージャーが破棄されていたり、CullingGroupが未生成なら中断
            if (mCullingGroup == null) return;

            // 最新のメインカメラを探してセット
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mCullingGroup.targetCamera = mainCam;

                // ターゲットをプレイヤーにしたい場合はここで検索
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    mListenerTarget = player.transform;
                }
                else
                {
                    mListenerTarget = mainCam.transform;
                }

                mCullingGroup.SetDistanceReferencePoint(mListenerTarget);
            }
        }

        private void OnCullingStateChanged(CullingGroupEvent evt)
        {
            if (evt.index >= mAudioObjects.Count) return;
            AudioSource targetSource = mAudioObjects[evt.index];
            // 非アクティブなオブジェクトへの処理はスキップ
            if (!targetSource.gameObject.activeSelf) return;
            // BGMなどはカリング対象外にする場合、ここで弾く (例: spatialBlend == 0 は対象外など)
            if (targetSource.spatialBlend < 0.1f) return;
            // evt.currentDistance: 0 = 範囲内, 1 = 範囲外
            if (evt.currentDistance == 0)
            {
                // 範囲内に入った -> 再生再開 (UnPause)
                // ※Pauseされていた場合のみ再開
                targetSource.UnPause();
            }
            else
            {
                // 範囲外に出た -> 一時停止 (Pause)
                // Stopだと再開時に頭出しになるためPause推奨
                targetSource.Pause();
            }
        }

        private void Update()
        {
            if (mCullingGroup == null || mAudioObjects == null) return;

            // AudioSourceの位置をBoundingSphereに反映
            int count = mAudioObjects.Count;
            for (int i = 0; i < count; i++)
            {
                if (mAudioObjects[i].gameObject.activeSelf)
                {
                    mSpheres[i].position = mAudioObjects[i].transform.position;
                    mSpheres[i].radius = 1.0f;
                }
                else
                {
                    // 非アクティブなものは判定外の遠くへ飛ばしておく
                    mSpheres[i].position = new Vector3(0, -99999, 0);
                    mSpheres[i].radius = 0.1f;
                }
            }

            // CullingGroupに最新の座標を適用
            mCullingGroup.SetBoundingSpheres(mSpheres);
            mCullingGroup.SetBoundingSphereCount(count);
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
        public void StopBGM()
        {
            if(mPlayingBGMAudioSource == null)
            {
                return;
            }
            mPlayingBGMAudioSource.Stop();
        }

        public void UnPauseStart()
        {
            if(mPlayingBGMAudioSource == null) 
            {
                return; 
            }
            mPlayingBGMAudioSource.UnPause();
        }

        public void PauseBGM()
        {
            if (mPlayingBGMAudioSource == null)
            {
                return;
            }
            mPlayingBGMAudioSource.Pause();
        }


        public void PlayOneShot3D(int id,Vector3 postion,Transform parent = null,bool loop = false,bool destroyCollection = false, float endSECount = -1)
        {
            if (Time.timeSinceLevelLoad < mMuteDurationAtStart)
            {
                return;
            }
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

            if (parent)
            {
                audioSource.transform.SetParent(parent);
                audioSource.transform.localPosition = Vector3.zero;
            }
            else
            {
                audioSource.transform.SetParent(transform); // SoundManagerの子に戻す
                audioSource.transform.position = postion;
            }
            audioSource.clip = clip;
            if(!audioSource.loop)
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                audioSource.Play();
            }
            if (!destroyCollection)
            {
                // 再生終了後に回収する仕組みが必要
                if (endSECount < 0)
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



            audioSource.clip = clip;
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


        private void OnDestroy()
        {
            if (mCullingGroup != null)
            {
                mCullingGroup.Dispose();
                mCullingGroup = null;
            }
        }
    }
}
