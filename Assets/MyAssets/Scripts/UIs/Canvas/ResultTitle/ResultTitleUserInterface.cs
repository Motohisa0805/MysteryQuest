using UnityEngine;
using UnityEngine.UI;

namespace MyAssets
{
    public class ResultTitleUserInterface : MonoBehaviour
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
            string text = mText.text;
            if(ResultManager.IsPlayerDeath)
            {
                text = "GAME OVER...";
            }
            else
            {
                text = "ステージクリア!";
            }
            mText.text = text;
        }
    }
}
