using UnityEngine;

namespace MyAssets
{
    // 3Dオブジェクトの物理音管理クラス
    [RequireComponent(typeof(ChemistryObject))]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsSoundObject : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float mMinImpactForce = 1.0f;
        [SerializeField]
        private float mMaxImpactForce = 15.0f;
        [SerializeField]
        private float mMinSlideSpeed = 0.5f;
        [SerializeField]
        private float mMaxSlideSpeed = 10.0f;

        //LOD
        [SerializeField]
        private float mSoundCullDistance = 20.0f;

        private ChemistryObject mChemistryObject;
        private Rigidbody mRigidbody;
        // スライド音管理用
        private AudioSource mCurrentSlideSource;
        private int mContactCount = 0;

        //衝突連打防止用タイマー
        private Timer mNextImpactTime = new Timer();

        private void Awake()
        {
            mChemistryObject = GetComponent<ChemistryObject>();
            mRigidbody = GetComponent<Rigidbody>();
        }
        // ---------------------------------------------------------
        // 1. 衝撃音 (Impact) - OnCollisionEnter
        // ---------------------------------------------------------
        private void OnCollisionEnter(Collision collision)
        {
            // 接触カウント増加（滑り判定用）
            mContactCount++;

            //衝突音の処理
            float impactForce = collision.impulse.magnitude;

            if (impactForce < mMinImpactForce || Time.time < mNextImpactTime.Current) return;

            if (Vector3.Distance(transform.position, Camera.main.transform.position) > mSoundCullDistance) return;
            //音量の計算
            float volumeScale = Mathf.Clamp01((impactForce - mMinImpactForce) / (mMaxImpactForce - mMinImpactForce));
            // 音のラベル名を決定 (例: "Wood_Hit", "Stone_Hit")
            string soundLabel = GetImpactSoundLabel(mChemistryObject.Material);
            // SoundManagerに再生依頼
            SoundManager.Instance.PlayOneShot3D_Physic(soundLabel, transform.position, volumeScale, transform);
            //クールダウン設定
            mNextImpactTime.Start(Time.time + 0.1f);
        }
        // ---------------------------------------------------------
        // 2. 接触状態管理 - OnCollisionExit
        // ---------------------------------------------------------
        private void OnCollisionExit(Collision collision)
        {
            // 接触カウント減少
            mContactCount--;
            if (mContactCount <= 0)
            {
                mContactCount = 0;
                // 完全に空中に浮いたので滑り音を止める
                StopSlideSound();
            }
        }
        // ---------------------------------------------------------
        // 3. 滑り音 (Sliding) - Update
        // ---------------------------------------------------------
        private void Update()
        {
            //接地していない、またはRigidbodyが無効なら処理しない
            if(mContactCount == 0 || mRigidbody.IsSleeping())
            {
                if (mCurrentSlideSource != null) StopSlideSound();
                return;
            }

            float speed = mRigidbody.linearVelocity.magnitude;
            // 速度が遅すぎる、またはカメラから遠すぎる場合
            bool isFar = Vector3.Distance(transform.position, Camera.main.transform.position) > mSoundCullDistance;
            if (speed < mMinSlideSpeed || isFar)
            {
                if (mCurrentSlideSource != null) StopSlideSound();
                return;
            }

            //ここで初めて音を再生 or 更新
            if (mCurrentSlideSource == null)
            {
                // まだ再生していないなら、SoundManagerからループ音を借りてくる
                string slideLabel = GetSlideSoundLabel(mChemistryObject.Material);

                // PlayLoopSEはAudioSourceを返してくれるので、変数を保持する
                mCurrentSlideSource = SoundManager.Instance.PlayLoopSE(slideLabel, transform.position, transform);
            }

            //再生中なら、速度に合わせたパラメータを更新
            if(mCurrentSlideSource != null)
            {
                float intensity = Mathf.InverseLerp(mMinSlideSpeed, mMaxSlideSpeed, speed);

                // 音量: 速いほど大きい
                mCurrentSlideSource.volume = Mathf.Lerp(0.1f, 1.0f, intensity);

                // ピッチ: 速いほど高い (0.8倍 ～ 1.2倍くらいが自然)
                mCurrentSlideSource.pitch = Mathf.Lerp(0.8f, 1.2f, intensity);
            }
        }

        private void StopSlideSound()
        {
            if (mCurrentSlideSource != null)
            {
                // SoundManagerに返却（フェードアウト付き）
                SoundManager.Instance.StopLoopSE(mCurrentSlideSource, 0.3f);
                mCurrentSlideSource = null;
            }
        }

        // ---------------------------------------------------------
        // ヘルパー：素材から音のラベル名を生成
        // ---------------------------------------------------------
        private string GetImpactSoundLabel(MaterialType mat)
        {
            // SoundListに登録されているラベル名に合わせて文字列結合
            // 例: MaterialType.Wood -> "Wood_Hit"
            return mat.ToString() + "_Hit";
        }

        private string GetSlideSoundLabel(MaterialType mat)
        {
            // 例: MaterialType.Stone -> "Stone_Slide"
            return mat.ToString() + "_Slide";
        }

        // オブジェクト破棄時にも音を止める
        private void OnDisable()
        {
            StopSlideSound();
        }
    }
}
