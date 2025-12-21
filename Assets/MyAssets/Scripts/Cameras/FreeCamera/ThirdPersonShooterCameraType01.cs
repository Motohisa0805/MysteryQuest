using UnityEngine;

namespace MyAssets
{

    //TPS専用のカメラクラス
    public class TPSCamera : MonoBehaviour
    {
        public enum Type
        {
            Free,
            ShoulderView,
            Focusing,
        }
        [SerializeField]
        private static Type         mType;
        public static Type          CameraType
        {
            get { return mType; }
            set { mType = value; }
        }

        [SerializeField]
        private Type                mPastType;
        //全カメラ共通の設定
        [SerializeField]
        private static Transform    mTarget; //注視対象
        [SerializeField]
        private static Transform    mFocusingTarget;
        public static Transform     FocusingTarget { get {  return mFocusingTarget; } set { mFocusingTarget = value; } }

        public Vector3              mAttentionPoint;


        private float               mYaw; //カメラの水平回転角
        private float               mPitch; //カメラの垂直回転角
        private Vector3             mCurrentVelocity; //カメラの現在の速度
        private Vector3             mDesiredPosition; //カメラの目標位置
        private Vector3             mCurrentPosition; //カメラの現在位置
        //==============================
        //フリーカメラ設定
        //==============================
        [SerializeField]
        private CameraSettingsTPS   mFreeCameraSettings;
        //==============================
        //ショルダービュー設定
        //==============================
        [SerializeField]
        private CameraSettingsTPS   mShoulderViewSettings;

        [SerializeField]
        private CameraSettingsTPS   mFocusingSettings;

        [SerializeField]
        private LayerMask           mLayerMask; //カメラの当たり判定に使用するレイヤーマスク
        public LayerMask            LayerMask => mLayerMask;
        private void Awake()
        {
            FreeCameraTargetPoint targetPoint = FindFirstObjectByType<FreeCameraTargetPoint>();
            if (targetPoint != null)
            {
                mTarget = targetPoint.transform;
            }
            if (mTarget == null)
            {
                Debug.LogError("Target not assigned in " + gameObject.name);
            }
            mCurrentPosition = transform.position;
            Vector3 angles = transform.eulerAngles;
            mYaw = angles.y;
            mPitch = angles.x;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            if (mTarget == null)
            {
                Debug.LogWarning("Target not assigned in " + gameObject.name);
                return;

            }
            mCurrentPosition = transform.position;
            Vector3 angles = transform.eulerAngles;
            mYaw = angles.y;
            mPitch = angles.x;

            mType = Type.Free;
        }
        // Update is called once per frame
        private void Update()
        {
            if (mTarget == null)
            {
                return;
            }
            if(mType == Type.Free)
            {
                // マウス入力を取得
                Vector2 mouse = InputManager.GetKeyValue(KeyCode.eLook);
                float mouseX = mouse.x;
                float mouseY = mouse.y;
                // カメラの回転角を更新
                mYaw += mouseX * mFreeCameraSettings.Sensitivity;
                mPitch -= mouseY * mFreeCameraSettings.Sensitivity;
                mPitch = Mathf.Clamp(mPitch, mFreeCameraSettings.MinAngle, mFreeCameraSettings.MaxAngle);
            }
            else if(mType == Type.ShoulderView)
            {
                // マウス入力を取得
                Vector2 mouse = InputManager.GetKeyValue(KeyCode.eLook);
                float mouseX = mouse.x;
                float mouseY = mouse.y;
                // カメラの回転角を更新
                mYaw += mouseX * mShoulderViewSettings.Sensitivity;
                mPitch -= mouseY * mShoulderViewSettings.Sensitivity;
                mPitch = Mathf.Clamp(mPitch, mShoulderViewSettings.MinAngle, mShoulderViewSettings.MaxAngle);
            }
            else if(mType == Type.Focusing)
            {

            }
        }

        private void FixedUpdate()
        {
            if (mTarget == null) { return; }
            Vector3 lookAt = Vector3.zero;
            Vector3 offset = Vector3.zero;
            Vector3 linecastStart = Vector3.zero;
            float smoothTime = 0;
            if (mType == Type.Free)
            {
                mAttentionPoint = mTarget.transform.position;
                // カメラの目標位置を計算
                Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                offset = rotation * new Vector3(0, 0, -mFreeCameraSettings.Distance) + mFreeCameraSettings.Offset;
                mDesiredPosition = mAttentionPoint + offset;
                linecastStart = mAttentionPoint + mFreeCameraSettings.Offset;
                smoothTime = mFreeCameraSettings.SmoothTime;
                lookAt = mAttentionPoint + mFreeCameraSettings.Offset;
            }
            else if (mType == Type.ShoulderView)
            {
                // カメラの目標位置を計算
                Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                offset = rotation * new Vector3(0, 0, -mShoulderViewSettings.Distance) + mShoulderViewSettings.Offset;
                mDesiredPosition = mAttentionPoint + offset;
                linecastStart = mAttentionPoint + mShoulderViewSettings.Offset;
                smoothTime = mShoulderViewSettings.SmoothTime;
                lookAt = mAttentionPoint + mShoulderViewSettings.Offset;
            }
            else if (mType == Type.Focusing)
            {
                if(mType != mPastType)
                {

                }

                if (mFocusingTarget != null)
                {
                    // 1. プレイヤーからターゲットへの方向を計算
                    Vector3 targetPos = mFocusingTarget.position;
                    Vector3 playerPos = mTarget.position; // mTargetは注視対象(プレイヤー)
                    Vector3 direction = (targetPos - playerPos).normalized;

                    // --- 左右のオフセット決定ロジック ---
                    // プレイヤーから見てカメラが左右どちらにいたかを判定（初回切り替え時のみ保持するのが理想）
                    float side = 1.0f;
                    Vector3 relativeCameraPos = mTarget.InverseTransformPoint(mCurrentPosition);
                    side = (relativeCameraPos.x >= 0) ? 1.0f : -1.0f; // 右なら1, 左なら-1

                    // 注目時のYawを計算
                    Quaternion targetRot = Quaternion.LookRotation(direction);
                    mYaw = targetRot.eulerAngles.y;

                    // Pitchの制御（ターゲットを画面のどの高さに置くか）
                    // そのまま計算すると真ん中すぎるので、少しオフセットを加えるのが一般的です
                    float targetPitch = targetRot.eulerAngles.x;
                    // eulerAnglesは0-360なので、計算しやすいように調整
                    if (targetPitch > 180) targetPitch -= 360;
                    mPitch = Mathf.Lerp(mPitch, targetPitch, Time.fixedDeltaTime * 10f); // 滑らかに追従



                    // 3. カメラ位置の計算（Freeカメラのロジックを流用しつつ距離などを調整）
                    Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                    offset = rotation * new Vector3(0, 0, -mFocusingSettings.Distance) + mFocusingSettings.Offset;
                    // 横にずらすためのオフセット計算
                    // プレイヤーの真後ろから「side」の方向に少しずらす
                    Vector3 sideOffset = targetRot * new Vector3(mFocusingSettings.Offset.x * side, 0, 0);

                    // カメラの目標位置：プレイヤー位置 + 背後への距離 + 横へのオフセット
                    mDesiredPosition = playerPos - (direction * mFocusingSettings.Distance) + sideOffset + Vector3.up * mFocusingSettings.Offset.y;

                    linecastStart = playerPos + mFocusingSettings.Offset;
                    smoothTime = mFocusingSettings.SmoothTime;
                    lookAt = (playerPos + targetPos) * 0.5f; // プレイヤーと敵の中間を見る
                }
                else
                {
                    // ターゲットがいない場合はFreeに戻すなどの処理
                    mType = Type.Free;
                }
            }
            // カメラの当たり判定
            RaycastHit hit;
            if (Physics.Linecast(linecastStart, mDesiredPosition, out hit, mLayerMask))
            {
                mDesiredPosition = hit.point;
            }
            mCurrentPosition = Vector3.SmoothDamp(mCurrentPosition, mDesiredPosition, ref mCurrentVelocity, smoothTime);
            transform.position = mCurrentPosition;
            transform.LookAt(lookAt);
        }

        private void LateUpdate()
        {
            mPastType = mType;
        }
    }
}
