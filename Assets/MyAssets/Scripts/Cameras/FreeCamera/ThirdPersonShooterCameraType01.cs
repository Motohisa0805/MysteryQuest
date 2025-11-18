using UnityEngine;

namespace MyAssets
{
    public class TPSCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform mTarget; //注視対象
        [SerializeField]
        private Vector3 mOffset; //注視対象からのオフセット
        [SerializeField]
        private float mDistance; //注視対象からの距離
        [SerializeField]
        private float mSensitivity; //感度
        [SerializeField]
        private float mMinAngle; //最小仰角
        [SerializeField]
        private float mMaxAngle; //最大仰角
        [SerializeField]
        private float mSmoothTime; //カメラの追従を滑らかにする時間
        [SerializeField]
        private LayerMask mLayerMask; //カメラの当たり判定に使用するレイヤーマスク
        private float mYaw; //カメラの水平回転角
        private float mPitch; //カメラの垂直回転角
        private Vector3 mCurrentVelocity; //カメラの現在の速度
        private Vector3 mDesiredPosition; //カメラの目標位置
        private Vector3 mCurrentPosition; //カメラの現在位置
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
        void Start()
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
        }
        // Update is called once per frame
        void Update()
        {
            if (mTarget == null)
            {
                return;
            }

        }

        private void FixedUpdate()
        {
            if (mTarget == null) { return; }

            // マウス入力を取得
            Vector2 mouse = InputManager.GetKeyValue(KeyCode.eLook);
            float mouseX = mouse.x;
            float mouseY = mouse.y;
            // カメラの回転角を更新
            mYaw += mouseX * mSensitivity;
            mPitch -= mouseY * mSensitivity;
            mPitch = Mathf.Clamp(mPitch, mMinAngle, mMaxAngle);

            // カメラの目標位置を計算
            Quaternion rotation = Quaternion.Euler(mPitch, mYaw, 0);
            Vector3 offset = rotation * new Vector3(0, 0, -mDistance) + mOffset;
            mDesiredPosition = mTarget.position + offset;
            // カメラの当たり判定
            RaycastHit hit;
            if (Physics.Linecast(mTarget.position + mOffset, mDesiredPosition, out hit, mLayerMask))
            {
                mDesiredPosition = hit.point;
            }
            // カメラの位置を滑らかに追従
            mCurrentPosition = Vector3.SmoothDamp(mCurrentPosition, mDesiredPosition, ref mCurrentVelocity, mSmoothTime);
            transform.position = mCurrentPosition;
            transform.LookAt(mTarget.position + mOffset);
        }
    }
}
