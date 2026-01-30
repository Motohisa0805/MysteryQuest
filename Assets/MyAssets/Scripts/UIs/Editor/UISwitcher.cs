using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyAssetsEditor
{
    public class UISwitcher : MonoBehaviour
    {
        public GameObject[] mPanels;
#if UNITY_EDITOR
        //コンテキストメニュー(右クリックや歯車アイコン)から実行可能にする
        [ContextMenu("Show Title")]
        void ShowTitle() => SwitchPanel(0);
        [ContextMenu("Show Settings")]
        void ShowSettings() => SwitchPanel(1);

        [ContextMenu("Hide All")]
        void HideAll() => SwitchPanel(-1);
        private void SwitchPanel(int index)
        {
            Undo.RecordObjects(mPanels, "UI Switch");
            for(int i = 0; i < mPanels.Length; i++)
            {
                if (mPanels[i] != null)
                    mPanels[i].SetActive(i == index);
            }
        }
#endif
    }
}
