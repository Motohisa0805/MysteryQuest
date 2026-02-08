using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyAssets
{
    public class EquipmentController : MonoBehaviour
    {
        private PlayableChracterController mController;

        private bool mBattleMode = false;
        public bool IsBattleMode { get { return mBattleMode; }set { mBattleMode = value; } }

        //剣のエフェクト管理クラス
        private SwingEffectHandler mSwingEffectHandler;
        public SwingEffectHandler SwingEffectHandler => mSwingEffectHandler;

        private SwordStick mSwordStick;
        public SwordStick SwordStick => mSwordStick;

        private void Awake()
        {
            mController = GetComponent<PlayableChracterController>();
            if(mController == null)
            {
                Debug.LogError("Not Find PlayableChracterController : " + gameObject.name);
            }
        }
        
        public void SetEquipment(SetItemTransform.TransformType handType,int id)
        {
            //初期化で剣を生成
            SetItemTransform[] hands = mController.HandTransforms;
            SetItemTransform rightHand = null;
            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i].Type == handType)
                {
                    rightHand = hands[i];
                    break;
                }
            }
            ItemInfo itemInfo = ItemManager.Instance.ItemTable.Items[id];
            // 1. 生成をリクエスト（この時点ではまだ生成されていない）
            var handle = itemInfo.gObject.InstantiateAsync(rightHand.transform);
            // 2. 完了した時の処理を登録
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    // 生成されたオブジェクトを取得
                    GameObject spawnedWeapon = op.Result;
                    rightHand.SetItemID(itemInfo.gId);
                    rightHand.HaveObject = spawnedWeapon;
                    spawnedWeapon.transform.localPosition = Vector3.zero;
                    spawnedWeapon.transform.localRotation = Quaternion.identity;
                    //剣のエフェクト管理クラスを取得しておく
                    mSwingEffectHandler = spawnedWeapon.GetComponentInChildren<SwingEffectHandler>();
                    mSwingEffectHandler?.ActivateSlachEffect(false);
                    mSwordStick = spawnedWeapon.GetComponent<SwordStick>();
                }
            };
            //もし武器とセットにあるオブジェクトがあるなら
            // 1. 生成をリクエスト（この時点ではまだ生成されていない）
            if(itemInfo.gSetObject == null) { return; }
            handle = itemInfo.gSetObject.InstantiateAsync(rightHand.transform);
            // 2. 完了した時の処理を登録
            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    // 生成されたオブジェクトを取得
                    GameObject spawnedWeapon = op.Result;
                    spawnedWeapon.transform.localPosition = Vector3.zero;
                    spawnedWeapon.transform.localRotation = Quaternion.identity;
                }
            };
        }

        public void ChangeParent(SetItemTransform.TransformType from,SetItemTransform.TransformType to)
        {
            if(from == SetItemTransform.TransformType.None || to == SetItemTransform.TransformType.None)
            {
                return;
            }
            SetItemTransform[] parents = mController.HandTransforms;
            SetItemTransform currentParent = null;
            SetItemTransform nextParent = null;
            for (int i = 0; i < parents.Length; i++)
            {
                if (parents[i].Type == from)
                {
                    currentParent = parents[i];
                }
                if (parents[i].Type == to)
                {
                    nextParent = parents[i];
                }
            }
            GameObject obj = currentParent.HaveObject;
            if(obj == null)
            {
                return;
            }
            obj.transform.SetParent(null);
            obj.transform.SetParent(nextParent.transform);
            nextParent.HaveObject = obj;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }

        private void Start()
        {
            mBattleMode = false;
            //初期化で右手に剣を持たせる
            SetEquipment(SetItemTransform.TransformType.Weapon,0);
        }
    }
}
