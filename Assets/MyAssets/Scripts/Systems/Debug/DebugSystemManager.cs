using UnityEngine;

namespace MyAssets
{
    public class DebugSystemManager : MonoBehaviour
    {
        private DebugSystemManager instance;
        public DebugSystemManager Instance => instance;


        private bool mDebugMode;

        public bool DebugMode => mDebugMode;

        [SerializeField]
        private Transform mDebugPanelPrefab;

        [SerializeField]
        private Transform mDebugInputPanel;

        [SerializeField]
        private Transform mWoodBoxCreateTransform;
        [SerializeField]
        private Transform mWoodBox;

        [SerializeField]
        private Transform mWoodCreateTransform;
        [SerializeField]
        private Transform mWood;

        [SerializeField]
        private Transform mCombustibleCreateTransform;
        [SerializeField]
        private Transform mCombustible;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas)
            {
                Transform obj = Instantiate(mDebugPanelPrefab, canvas.transform);
                mDebugInputPanel = obj.Find("DebugPanel");
            }
            if(mDebugInputPanel != null)
            {
                mDebugInputPanel.gameObject.SetActive(mDebugMode);
            }
        }

        private void Update()
        {
            if(InputManager.GetKeyDown(KeyCode.eF1))
            {
                mDebugMode = !mDebugMode;
                if(mDebugInputPanel != null)
                {
                    mDebugInputPanel.gameObject.SetActive(mDebugMode);
                }
                if(mDebugMode)
                {
                    InputManager.SetNoneMouseMode();
                }
                else
                {
                    InputManager.SetLockedMouseMode();
                }
            }
            if(mDebugMode)
            {
                DebugFunc();
            }
        }

        private void DebugFunc()
        {
            if(InputManager.GetKeyDown(KeyCode.Num1))
            {
                Transform obj = Instantiate(mWoodBox,mWoodBoxCreateTransform);
                obj.localPosition = Vector3.zero;
                //obj.localRotation = Quaternion.identity;
                //obj.localScale = Vector3.one;
            }
            if (InputManager.GetKeyDown(KeyCode.Num2))
            {
                Transform obj = Instantiate(mWood, mWoodCreateTransform);
                obj.localPosition = Vector3.zero;
                //obj.localRotation = Quaternion.identity;
                //obj.localScale = Vector3.one;
            }
            if (InputManager.GetKeyDown(KeyCode.Num3))
            {
                Transform obj = Instantiate(mCombustible, mCombustibleCreateTransform);
                obj.localPosition = Vector3.zero;
                //obj.localRotation = Quaternion.identity;
                //obj.localScale = Vector3.one;
            }
        }
    }
}
