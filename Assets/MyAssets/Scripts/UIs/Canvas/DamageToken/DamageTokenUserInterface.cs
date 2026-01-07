using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class DamageTokenUserInterface : MonoBehaviour
    {
        private Text mText;

        private void Awake()
        {
            mText = GetComponentInChildren<Text>();
            if (mText == null)
            {
                Debug.LogError("Not Find Text Component" + gameObject.name);
            }
        }

        private void OnEnable()
        {
            if (mText == null) { return; }
            int damageNum = ResultManager.DamageTaken;
            mText.text = damageNum.ToString() + "É_ÉÅÅ[ÉW";
        }
    }
}
