using UnityEngine;

namespace MyAssets
{
    //AnimatorのIK処理を親の管理クラスに委譲するための中継クラス
    //管理クラスとAnimatorは親子関係のためOnAnimatorIKを管理クラスで受け取れないため
    //この中継クラスをAnimatorにアタッチしてIK処理を委譲する
    public class IKRelay : MonoBehaviour
    {
        // 親の管理クラス（OnAnimatorIKの処理を実際に書きたいクラス）
        private PlayableChracterController mParentManager;

        private void Awake()
        {
            // アタッチし忘れ防止（自動で親を探す）
            if (mParentManager == null)
            {
                mParentManager = GetComponentInParent<PlayableChracterController>();
            }
        }

        // Animatorから呼ばれる
        private void OnAnimatorIK(int layerIndex)
        {
            if (mParentManager != null)
            {
                float t = Time.deltaTime;
                // 親のpublic関数を呼び出して処理を委譲する
                mParentManager.StateMachine.IKAnimatorUpdate(t);
            }
        }
    }
}
