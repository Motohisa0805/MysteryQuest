using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class TimerUserInterface : MonoBehaviour
    {
        private Text mText;

        private void Awake()
        {
            mText = GetComponentInChildren<Text>();
            if(mText == null)
            {
                Debug.LogError("Not Find Text Component" + gameObject.name);
            }
        }

        private void OnEnable()
        {
            if(mText == null) { return; }
            float mm = ResultManager.GameTimer.GetMinutes();
            float ss = ResultManager.GameTimer.GetSecond();
            mText.text = mm.ToString() + " : " + ss.ToString();
        }
    }
}
