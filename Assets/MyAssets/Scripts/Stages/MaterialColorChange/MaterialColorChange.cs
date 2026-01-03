using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class MaterialColorChange : MonoBehaviour
    {
        [SerializeField]
        private bool mIsEmissionColorChange;


        private MeshRenderer mMeshRenderer;

        private List<Material> mMaterial = new List<Material>();

        private void Awake()
        {
            mMeshRenderer = GetComponent<MeshRenderer>();
            foreach (var mat in mMeshRenderer.materials)
            {
                mMaterial.Add(mat);
            }
        }

        public void ChangeEmissionColor()
        {
            foreach(var mat in mMaterial)
            {
                if (mIsEmissionColorChange)
                {
                    mat.EnableKeyword("_EMISSION");
                }
            }
        }

        public void DisableEmissionColor()
        {
            foreach(var mat in mMaterial)
            {
                if(mIsEmissionColorChange)
                {
                    mat.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}
