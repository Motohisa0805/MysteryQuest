using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public class ObjectRespawner : MonoBehaviour
    {
        [SerializeField]
        private Transform mRespawnPoint;

        [SerializeField]
        private List<RespawnObject> mRespawnObjects = new List<RespawnObject>();


        private void Awake()
        {
            RespawnObject[] respawnObjects = GetComponentsInChildren<RespawnObject>();
            mRespawnObjects = new List<RespawnObject>(respawnObjects);
            foreach (var respawnObject in mRespawnObjects)
            {
                respawnObject.Setup(this);
            }
        }

        private void Start()
        {
            mRespawnPoint = transform;
        }

        public void Respawn(RespawnObject respawnObject,Quaternion quaternion)
        {
            respawnObject.transform.position = mRespawnPoint.position;
            respawnObject.transform.rotation = quaternion;
        }
    }
}

