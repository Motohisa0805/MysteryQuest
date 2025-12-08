using UnityEngine;

namespace MyAssets
{

    //TPS専用のカメラクラス
    public class TPSCamera : MonoBehaviour
    {
        public enum Type
        {
            Free,
            ShoulderView
        }
        [SerializeField]
        private static Type mType;
        public static Type CameraType
        {
            get { return mType; }
            set { mType = value; }
        }

        [SerializeField]
        private Type mCurrentType;
        //全カメラ共通の設定
        [SerializeField]
        private Transform mTarget; //注視対象
        private float mYaw; //カメラの水平回転角
        private float mPitch; //カメラの垂直回転角
        private Vector3 mCurrentVelocity; //カメラの現在の速度
        private Vector3 mDesiredPosition; //カメラの目標位置
        private Vector3 mCurrentPosition; //カメラの現在位置
        //==============================
        //フリーカメラ設定
        //==============================
        [SerializeField]
        private CameraSettingsTPS mFreeCameraSettings;
        //==============================
        //ショルダービュー設定
        //==============================
        [SerializeField]
        private CameraSettingsTPS mShoulderViewSettings;
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
                Debug.LogError("Target not assigned in " + gameObject.name);
                return;

            }
            mCurrentPosition = transform.position;
            Vector3 angles = transform.eulerAngles;
            mYaw = angles.y;
            mPitch = angles.x;

            mType = mCurrentType;
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
        }

        private void FixedUpdate()
        {
            if (mTarget == null) { return; }

            if (mType == Type.Free)
            {
                // カメラの目標位置を計算
                Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                Vector3 offset = rotation * new Vector3(0, 0, -mFreeCameraSettings.Distance) + mFreeCameraSettings.Offset;
                mDesiredPosition = mTarget.position + offset;
                // カメラの当たり判定
                RaycastHit hit;
                if (Physics.Linecast(mTarget.position + mFreeCameraSettings.Offset, mDesiredPosition, out hit, mFreeCameraSettings.LayerMask))
                {
                    mDesiredPosition = hit.point;
                }
                // カメラの位置を滑らかに追従
                mCurrentPosition = Vector3.SmoothDamp(mCurrentPosition, mDesiredPosition, ref mCurrentVelocity, mFreeCameraSettings.SmoothTime);
                transform.position = mCurrentPosition;
                transform.LookAt(mTarget.position + mFreeCameraSettings.Offset);
            }
            else if (mType == Type.ShoulderView)
            {
                // カメラの目標位置を計算
                Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
                Vector3 offset = rotation * new Vector3(0, 0, -mShoulderViewSettings.Distance) + mShoulderViewSettings.Offset;
                mDesiredPosition = mTarget.position + offset;
                // カメラの当たり判定
                RaycastHit hit;
                if (Physics.Linecast(mTarget.position + mShoulderViewSettings.Offset, mDesiredPosition, out hit, mShoulderViewSettings.LayerMask))
                {
                    mDesiredPosition = hit.point;
                }
                // カメラの位置を滑らかに追従
                mCurrentPosition = Vector3.SmoothDamp(mCurrentPosition, mDesiredPosition, ref mCurrentVelocity, mShoulderViewSettings.SmoothTime);
                transform.position = mCurrentPosition;
                transform.LookAt(mTarget.position + mShoulderViewSettings.Offset);
            }
        }
    }
}
