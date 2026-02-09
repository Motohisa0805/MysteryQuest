using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    //プレイヤーのスタミナUI処理クラス
    public class StaminaHUDController : MonoBehaviour
    {
        //スタミナ本体(緑色)
        [SerializeField]
        private Slider  mStaminaWheel;
        //スタミナの軌跡(赤色)
        [SerializeField]
        private Slider  mUsageWheel;

        private float   mStamina;
        private float   mMaxStamina;

        private void Start()
        {
            PlayerUIManager.Instance.StaminaHUDController = this;
            mStamina = (int)PlayerStatusManager.Instance.PlayerStatusData.MaxStamina;
            mMaxStamina = (int)PlayerStatusManager.Instance.PlayerStatusData.MaxStamina;
        }

        public void CheckStamina()
        {
            if(PlayerStatusManager.Instance.PlayerStatusData.CurrentStamina / PlayerStatusManager.Instance.PlayerStatusData.MaxStamina < 1.0f)
            {
                gameObject.SetActive(true);
            }
        }

        public void UpdateStaminaHUD()
        {
            mStamina = PlayerStatusManager.Instance.PlayerStatusData.CurrentStamina;
            mMaxStamina = PlayerStatusManager.Instance.PlayerStatusData.MaxStamina;
            bool useStamina = PlayerStatusManager.Instance.UseStamina;
            if (useStamina && !PlayerStatusManager.Instance.IsStaminaCoolDown)
            {
                mUsageWheel.value = mStamina / mMaxStamina + 0.05f;
            }
            else
            {
                mUsageWheel.value = mStamina / mMaxStamina;
            }
            if (!PlayerStatusManager.Instance.IsStaminaCoolDown)
            {
                mStaminaWheel.value = mStamina / mMaxStamina;
            }
            if(mStamina / mMaxStamina >= 1.0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
