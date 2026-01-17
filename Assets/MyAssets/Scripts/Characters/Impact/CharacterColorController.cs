using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyAssets
{
    public class CharacterColorController : MonoBehaviour
    {
        // マテリアルと元の色のペアを管理する内部クラス
        private class MaterialData
        {
            public Material gMaterial;
            public Color    mOriginalColor;
            public string   mPropertyName;
        }

        private List<MaterialData>  mMaterialList = new List<MaterialData>();
        private Coroutine           mFlashRoutine;

        private void Awake()
        {
            //全ての SkinnedMeshRenderer を取得
            var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var renderer in renderers)
            {
                // 各レンダラーが持つ全マテリアルをチェック
                foreach (var mat in renderer.materials)
                {
                    // "_Color" があるか、なければ URP の "_BaseColor" があるかチェック
                    string colorProp = "";
                    if (mat.HasProperty("_Color")) colorProp = "_Color";
                    else if (mat.HasProperty("_BaseColor")) colorProp = "_BaseColor";

                    if (!string.IsNullOrEmpty(colorProp))
                    {
                        mMaterialList.Add(new MaterialData
                        {
                            gMaterial = mat,
                            mOriginalColor = mat.GetColor(colorProp),
                            mPropertyName = colorProp // プロパティ名を保存しておく
                        });
                    }
                    else
                    {
                        Debug.LogWarning($"{mat.name} には色プロパティが見つかりませんでした。");
                    }
                }
            }
        }

        public void FlashRed(float duration)
        {
            if (mFlashRoutine != null) StopCoroutine(mFlashRoutine);
            mFlashRoutine = StartCoroutine(FlashSequence(duration));
        }

        private IEnumerator FlashSequence(float duration)
        {
            // 全マテリアルを赤に設定
            SetAllColors(Color.red);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // 各マテリアルを個別に元の色へ Lerp
                foreach (var data in mMaterialList)
                {
                    data.gMaterial.color = Color.Lerp(Color.red, data.mOriginalColor, t);
                }
                yield return null;
            }

            // 最後に確実に元の色に戻す
            ResetAllColors();
        }

        private void SetAllColors(Color color)
        {
            foreach (var data in mMaterialList)
            {
                data.gMaterial.color = color;
            }
        }

        private void ResetAllColors()
        {
            foreach (var data in mMaterialList)
            {
                data.gMaterial.color = data.mOriginalColor;
            }
        }
    }
}