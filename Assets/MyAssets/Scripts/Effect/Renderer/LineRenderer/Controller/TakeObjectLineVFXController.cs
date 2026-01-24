using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class TakeObjectLineVFXController : MonoBehaviour
    {
        private List<LineRendererMovement> mLineList = new List<LineRendererMovement>();


        private void Awake()
        {
            LineRendererMovement[] lineRendererMovements = GetComponentsInChildren<LineRendererMovement>();
            mLineList.Clear();
            mLineList = new List<LineRendererMovement>(lineRendererMovements);
        }

        public void SetOriginTransform(Transform transform)
        {
            foreach (var line in mLineList)
            {
                line.StartTransform = transform;
            }
        }

        public void SetEndTransform(Transform endTransform)
        {
            foreach(var line in mLineList)
            {
                line.EndTransform = endTransform;
            }
        }
    }
}
