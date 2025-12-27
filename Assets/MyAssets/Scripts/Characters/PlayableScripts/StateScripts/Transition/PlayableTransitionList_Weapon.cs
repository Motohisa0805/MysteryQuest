using UnityEngine;

namespace MyAssets
{
    public class IsTakingOutTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private EquipmentController mEquipmentController;
        public IsTakingOutTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }
        public override bool IsTransition() => mInput.Attack && !mEquipmentController.IsBattleMode;
    }

    public class IsStorageTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private EquipmentController mEquipmentController;
        public IsStorageTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }
        public override bool IsTransition() => mInput.Sprit && mEquipmentController.IsBattleMode;
    }
}