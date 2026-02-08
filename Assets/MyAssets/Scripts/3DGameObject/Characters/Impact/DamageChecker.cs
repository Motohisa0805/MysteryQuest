using UnityEngine;

namespace MyAssets
{
    //キャラクターのダメージ計算を物理・疑似化学エンジン機能を元に管理するクラス
    [RequireComponent(typeof(ChemistryObject))]
    public class DamageChecker : MonoBehaviour
    {
        [SerializeField]
        private float                       mImpactPower;

        public float                        ImpactPower => mImpactPower;

        private float                       mMinImpactPower = 1000f;
        private float                       mMaxImpactPower = 1600f;

        public bool                         IsEnabledDamage => mImpactPower > 0;
        public bool                         IsEnabledSmallDamage => mImpactPower > mMinImpactPower && mImpactPower < mMaxImpactPower;
        public bool                         IsEnabledBigDamage => mImpactPower > mMaxImpactPower && mImpactPower > mMinImpactPower;

        public bool                         IsEnabledFallDamage => mImpactPower > 1000;

        private float                       mMinObjectSpeed = 5.0f;

        // 落下ダメージのしきい値
        private const float                 mFallDamageThreshold = 10f;

        //エレメントの継続的ダメージで使用
        private Timer                       mElementDamageTimer = new Timer();

        private ChemistryObject             mChemistryObject;

        private CharacterColorController    mCharacterColorController;

        //もし他キャラクターを追加するならここはキャラクタークラスになる。
        private PlayableChracterController  mChracterController;
        private float                       mFallTimer = 0f; // 落下時間を蓄積する変数
        public float                        FallTimer { get { return mFallTimer; } set { mFallTimer = value; } }
        private void Awake()
        {
            mChemistryObject = GetComponent<ChemistryObject>();
            if(mChemistryObject == null)
            {
                Debug.LogError("Not Found ChemistryObject" + gameObject.name);
            }
            mCharacterColorController = GetComponentInChildren<CharacterColorController>();
            if(mCharacterColorController == null)
            {
                Debug.LogError("Not Found CharacterColorController" + gameObject.name);
            }
            mChracterController = GetComponent<PlayableChracterController>();
            if (mChracterController == null)
            {
                Debug.LogError("Not Found PlayableChracterController" + gameObject.name);
            }
        }

        private void Update()
        {
            mElementDamageTimer.Update(Time.deltaTime);
            if(mChemistryObject.CurrentElements != ElementType.None)
            {
                ApplyElementDamage();
            }

            // 接地判定（CharacterControllerやレイキャスト、またはGroundedフラグを使用）
            if (!mChracterController.Grounded) // 空中にいる場合
            {
                // 落下速度が下向きの場合のみカウントするとより正確（ジャンプの上昇中を除外）
                if (GetComponent<Rigidbody>().linearVelocity.y < 0)
                {
                    mFallTimer += Time.deltaTime;
                }
            }
            else
            {
                // 着地したらタイマーをリセットする
                mFallTimer = 0f; 
            }
        }

        private void ApplyElementDamage()
        {
            if (!mElementDamageTimer.IsEnd()) { return; }
            if(mChemistryObject.CurrentElements == ElementType.Fire)
            {
                mImpactPower = mMinImpactPower;
                mElementDamageTimer.Start(1.0f);
                mCharacterColorController.FlashRed(0.5f);
                PlayerStatusManager.Instance.ChangeHP(-GetCalculatedDamage());
            }
        }

        public void ApplyDamagePower(Collision collision)
        {
            
            //キャラクター自身
            Rigidbody ri = GetComponent<Rigidbody>();
            //衝突者
            Rigidbody targetRb = collision.rigidbody;

            if (targetRb != null)
            {
                if (CheckFallImpact(collision, ri))
                {
                    // 垂直方向の相対速度のみを取り出す
                    float verticalVelocity = Mathf.Abs(collision.relativeVelocity.y);

                    if (verticalVelocity > mFallDamageThreshold)
                    {
                        float mass = ri != null ? ri.mass : 1f;
                        mImpactPower = verticalVelocity * (mass * mFallTimer);
                        mFallTimer = 0f;
                    }
                }
                else
                {
                    //相手が「自分より速い」かつ「一定以上の危険な速度（mMinObjectSpeed）」で動いているか
                    //さらに、相手が「自分の方に向かって」動いているかをチェック
                    Vector3 relativePos = transform.position - targetRb.transform.position;
                    float dirDot = Vector3.Dot(targetRb.linearVelocity.normalized, relativePos.normalized);

                    // directionDot > 0 なら、相手は自分の方に向かって動いている
                    if (collision.impulse.magnitude > mMinObjectSpeed &&
                        targetRb.linearVelocity.magnitude > ri.linearVelocity.magnitude &&
                        dirDot > 0)
                    {
                        Apply(targetRb, collision);
                        return;
                    }
                }
            }
            else
            {
                if(CheckFallImpact(collision, ri))
                {
                    // 垂直方向の相対速度のみを取り出す
                    float verticalVelocity = Mathf.Abs(collision.relativeVelocity.y);

                    if (verticalVelocity > mFallDamageThreshold)
                    {
                        float mass = ri != null ? ri.mass : 1f;
                        mImpactPower = verticalVelocity * (mass * mFallTimer);
                        mFallTimer = 0f;
                    }
                }
            }
        }

        private bool CheckFallImpact(Collision collision, Rigidbody ri)
        {
            // 衝突した面の法線を確認 (真下からの衝撃か)
            // ContactPoint.normalは衝突面から外側へのベクトル
            foreach (ContactPoint contact in collision.contacts)
            {
                // 法線が上向きか判断
                if (contact.normal.y > 0.7f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void Apply(Rigidbody targetRb, Collision collision)
        {
            float impactVelocity = collision.relativeVelocity.magnitude;
            float mass = targetRb.mass;
            float impactPower = 0.5f * (mass / 10) * (impactVelocity * impactVelocity);

            if (impactPower >= mMinImpactPower)
            {
                mImpactPower = impactPower;
                mCharacterColorController.FlashRed(0.5f);
            }
        }

        public void ClearImpactPower()
        {
            mImpactPower = 0f;
        }

        public void AddDamage(float damage)
        {
            mFallTimer = 0f;
            mImpactPower = damage;
        }

        public int GetCalculatedDamage()
        {
            if (mImpactPower < mMinImpactPower) return 0;

            int damage = 0;

            // 1. 物理パワーをベースダメージに変換
            if (mImpactPower >= mMaxImpactPower)
            {
                // 1600を超えた分、さらにダメージを上乗せする計算
                float extraPower = mImpactPower - mMaxImpactPower;
                damage = 120 + Mathf.FloorToInt(extraPower / 200f) * 30;
            }
            else if (mImpactPower >= mMaxImpactPower - 100)
            {
                damage = 60; // 小ダメージ（ハート半分）
            }
            else if (mImpactPower >= mMaxImpactPower -600)
            {
                damage = 30; // 落下・かすり傷（ハート1/4）
            }

            // 30の倍数にクランプ（念のための処理）
            return (damage / 30) * 30;
        }
    }
}
