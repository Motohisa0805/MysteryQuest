using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class StaminaHUDController : MonoBehaviour
    {

        [SerializeField]
        private Slider mStaminaWheel;
        [SerializeField]
        private Slider mUsageWheel;

        private float mStamina;
        private float mMaxStamina;

        private void Start()
        {
            PlayerUIManager.Instance.StaminaHUDController = this;
            mStamina = (int)PlayerStatusManager.Instance.PlayerStatusData.MaxSP;
            mMaxStamina = (int)PlayerStatusManager.Instance.PlayerStatusData.MaxSP;
        }

        public void CheckStamina()
        {
            if(PlayerStatusManager.Instance.PlayerStatusData.CurrentSP / PlayerStatusManager.Instance.PlayerStatusData.MaxSP < 1.0f)
            {
                gameObject.SetActive(true);
            }
        }

        public void UpdateStaminaHUD()
        {
            mStamina = PlayerStatusManager.Instance.PlayerStatusData.CurrentSP;
            mMaxStamina = PlayerStatusManager.Instance.PlayerStatusData.MaxSP;
            if (InputManager.GetKey(KeyCode.eSprint) && !PlayerStatusManager.Instance.IsStaminaCoolDown)
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
