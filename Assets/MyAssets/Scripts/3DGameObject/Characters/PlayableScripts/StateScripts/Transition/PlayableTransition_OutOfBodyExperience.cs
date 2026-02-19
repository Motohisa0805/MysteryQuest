using UnityEngine;

namespace MyAssets
{
    public class IsOutOfBodyExperienceTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private PlayableChracterController mController;
        public IsOutOfBodyExperienceTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponentInChildren<PlayableChracterController>();
        }
        public override bool IsTransition() => mInput.Skill && mController.Grounded;
    }
}