using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MyAssets
{
    //子オブジェクトにあるオブジェクトを最初と同じ配置で
    //設定し直すスクリプト
    //詰み対策も兼ねている
    public class GimicReseter : MonoBehaviour,IGimmickReceiver
    {
        private List<ChemistryObject> mGimicObjects = new List<ChemistryObject>();

        private List<MaterialType> mObjectMaterials = new List<MaterialType>();
        private List<Vector3> mPositions = new List<Vector3>();

        [SerializeField]
        private ChemistryObject[] mChemistryObjects;

        private void Awake()
        {
            mGimicObjects.Clear();
            ChemistryObject[] objects = GetComponentsInChildren<ChemistryObject>();
            mGimicObjects = objects.OrderBy(obj => obj.transform.position.y).ToList();
            foreach(ChemistryObject obj in objects)
            {
                MaterialType type = obj.Material;
                mObjectMaterials.Add(type);
                Vector3 pos = obj.gameObject.transform.position;
                mPositions.Add(pos);
            }
        }

        //オブジェクトを最初の位置に設定し直す
        private void ResetObject()
        {
            if(mChemistryObjects.Length <= 0 || mObjectMaterials.Count <= 0) {  return; }
            foreach (ChemistryObject obj in mGimicObjects)
            {
                if(obj != null)
                {
                    Destroy(obj.gameObject);
                }
            }
            mGimicObjects.Clear();
            
            for(int i = 0; i < mObjectMaterials.Count; i++)
            {
                ChemistryObject chemistryObject = null;
                for(int j = 0; j < mChemistryObjects.Length; j++)
                {
                    if (mChemistryObjects[j].Material == mObjectMaterials[i])
                    {
                        chemistryObject= mChemistryObjects[j];
                    }
                }
                ChemistryObject obj = Instantiate(chemistryObject, mPositions[i],Quaternion.identity);
                obj.transform.SetParent(transform);
            }
            ChemistryObject[] objects = GetComponentsInChildren<ChemistryObject>();
            mGimicObjects = objects.OrderBy(obj => obj.transform.position.y).ToList();
        }


        public void OnActivate(float count = 0)
        {
            ResetObject();
        }

        public void OnDeactivate()
        {

        }
    }
}
