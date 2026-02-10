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
            Fixed,
        }
        [SerializeField]
        private static Type         mType;
        public static Type          CameraType
        {
            get { return mType; }
            set { mType = value; }
        }
        [SerializeField]
        private Type                mCurrentType;
        [SerializeField]
        private Type                mPastType;
        //全カメラ共通の設定
        [SerializeField]
        private static Transform    mTarget; //注視対象
        [SerializeField]
        private static Transform    mFocusingTarget;
        public static Transform     FocusingTarget { get {  return mFocusingTarget; } set { mFocusingTarget = value; } }

        public Vector3              mAttentionPoint;

        [SerializeField]
        private float               mYaw; //カメラの水平回転角
        [SerializeField]
        private float               mPitch; //カメラの垂直回転角
        private Vector3             mCurrentVelocity; //カメラの現在の速度
        private Vector3             mDesiredPosition; //カメラの目標位置
        private Vector3             mCurrentPosition; //カメラの現在位置

        private float               mSensitivityCorrection = 6.0f;
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
        private CameraSettingsTPS   mFixedSettings;

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

            mCurrentType = Type.Free;
        }

        private float GetCameraSensitivity(float sensitivity)
        {
            if(InputManager.IsCurrentControlSchemeKeyBoard)
            {
                return sensitivity;
            }
            return sensitivity * mSensitivityCorrection;
        }
        // Update is called once per frame
        private void Update()
        {
            if (mTarget == null || Time.timeScale <= 0.0f)
            {
                return;
            }
            mCurrentType = mType;
            if (mCurrentType == Type.Free)
            {
                // マウス入力を取得
                Vector2 mouse = InputManager.GetKeyValue(KeyCode.eLook);
                float mouseX = mouse.x;
                float mouseY = mouse.y;
                float sensitivity = GetCameraSensitivity(mFreeCameraSettings.Sensitivity);
                // カメラの回転角を更新
                mYaw += mouseX * (sensitivity * DataManager.SettingData.gInputRate);
                mPitch -= mouseY * (sensitivity * DataManager.SettingData.gInputRate);
                mPitch = Mathf.Clamp(mPitch, mFreeCameraSettings.MinAngle, mFreeCameraSettings.MaxAngle);
            }
            else if(mCurrentType == Type.ShoulderView)
            {
                // マウス入力を取得
                Vector2 mouse = InputManager.GetKeyValue(KeyCode.eLook);
                float mouseX = mouse.x;
                float mouseY = mouse.y;
                float sensitivity = GetCameraSensitivity(mShoulderViewSettings.Sensitivity);
                // カメラの回転角を更新
                mYaw += mouseX * (sensitivity * DataManager.SettingData.gInputRate);
                mPitch -= mouseY * (sensitivity * DataManager.SettingData.gInputRate);
                mPitch = Mathf.Clamp(mPitch, mShoulderViewSettings.MinAngle, mShoulderViewSettings.MaxAngle);
            }
            else if(mCurrentType == Type.Focusing)
            {
                if(mCurrentType == mPastType)
                {
                    // マウス入力を取得
                    Vector2 mouse = InputManager.GetKeyValue(KeyCode.eLook);
                    float mouseX = mouse.x;
                    float mouseY = mouse.y;
                    float sensitivity = GetCameraSensitivity(mFocusingSettings.Sensitivity);
                    // カメラの回転角を更新
                    mYaw += mouseX * (sensitivity * DataManager.SettingData.gInputRate);
                    mPitch -= mouseY * (sensitivity * DataManager.SettingData.gInputRate);
                    mPitch = Mathf.Clamp(mPitch, mFocusingSettings.MinAngle, mFocusingSettings.MaxAngle);
                }
            }
            else if (mCurrentType == Type.Fixed)
            {
                // 固定カメラの場合、回転や位置の更新は行わない
                mPitch = 0;
                mYaw = 0;
            }
        }

        private void FixedUpdate()
        {
            if (mTarget == null) { return; }
            Vector3 lookAt = Vector3.zero;
            Vector3 offset = Vector3.zero;
            Vector3 linecastStart = Vector3.zero;
            float smoothTime = 0;
            if (mCurrentType == Type.Free)
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
            else if (mCurrentType == Type.ShoulderView)
            {
                mAttentionPoint = mTarget.transform.position;
                // カメラの目標位置を計算
                Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                offset = rotation * new Vector3(0, 0, -mShoulderViewSettings.Distance) + mShoulderViewSettings.Offset;
                mDesiredPosition = mAttentionPoint + offset;
                linecastStart = mAttentionPoint + mShoulderViewSettings.Offset;
                smoothTime = mShoulderViewSettings.SmoothTime;
                lookAt = mAttentionPoint + mShoulderViewSettings.Offset;
            }
            else if (mCurrentType == Type.Focusing)
            {
                // 1. プレイヤーからターゲットへの方向を計算
                Vector3 targetPos = Vector3.zero;
                Vector3 playerPos = Vector3.zero;
                Vector3 direction = Vector3.zero;
                // --- 左右のオフセット決定ロジック ---
                // プレイヤーから見てカメラが左右どちらにいたかを判定（初回切り替え時のみ保持するのが理想）
                float side = 1.0f;
                if (mFocusingTarget != null)
                {
                    // 1. プレイヤーからターゲットへの方向を計算
                    targetPos = mFocusingTarget.position;
                    playerPos = mTarget.position; // mTargetは注視対象(プレイヤー)
                    direction = (targetPos - playerPos).normalized;
                }
                else
                {
                    // 1. プレイヤーからターゲットへの方向を計算
                    targetPos = mTarget.forward + mTarget.position;
                    playerPos = mTarget.position; // mTargetは注視対象(プレイヤー)
                    direction = (targetPos - playerPos).normalized;

                    // --- 左右のオフセット決定ロジック ---
                    // プレイヤーから見てカメラが左右どちらにいたかを判定（初回切り替え時のみ保持するのが理想）
                    side = 0;
                }
                
                // 注目時のYawを計算
                Quaternion targetRot = Quaternion.LookRotation(direction);
                float goalYaw = targetRot.eulerAngles.y;
                float goalPitch = targetRot.eulerAngles.x;
                if (goalPitch > 180) goalPitch -= 360;

                float followStrength = 0.05f;

                mYaw += Mathf.DeltaAngle(mYaw, goalYaw) * followStrength;
                mPitch += Mathf.DeltaAngle(mPitch, goalPitch) * followStrength;

                Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                // 注目時のオフセット計算（左右の肩越し視点を維持）
                Vector3 relativeCameraPos = mTarget.InverseTransformPoint(mCurrentPosition);
                side = (relativeCameraPos.x >= 0) ? 1.0f : -1.0f;
                Vector3 sideOffset = rotation * new Vector3(mFocusingSettings.Offset.x * side, 0, 0);

                mDesiredPosition = playerPos + (rotation * new Vector3(0, 0, -mFocusingSettings.Distance)) + Vector3.up * mFocusingSettings.Offset.y;

                linecastStart = playerPos + mFocusingSettings.Offset;
                smoothTime = mFocusingSettings.SmoothTime;
                lookAt = (playerPos + targetPos) * 0.5f; // プレイヤーと敵の中間を見る
            }
            else if (mCurrentType == Type.Fixed)
            {
                mAttentionPoint = mTarget.transform.position;
                // カメラの目標位置を計算
                Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                offset = rotation * new Vector3(0, 0, -mFixedSettings.Distance) + mFixedSettings.Offset;
                mDesiredPosition = mAttentionPoint + offset;
                linecastStart = mAttentionPoint + mFixedSettings.Offset;
                smoothTime = mFixedSettings.SmoothTime;
                lookAt = mAttentionPoint + mFixedSettings.Offset;
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
            mPastType = mCurrentType;
        }
    }
}
