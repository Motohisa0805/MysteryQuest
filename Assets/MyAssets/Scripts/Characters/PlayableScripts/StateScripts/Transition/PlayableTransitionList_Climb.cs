using UnityEngine;

namespace MyAssets
{
    public class IsClimbJumpingTransition : StateTransitionBase
    {
        readonly private Movement mMovement;
        public IsClimbJumpingTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mMovement = actor.GetComponent<Movement>();
        }
        public override bool IsTransition()
        {
            return mMovement.MovementCompensator.IsClimbJumping;
        }
    }

    public class IsClimbJumpTransition : StateTransitionBase
    {
        readonly private Movement mMovement;
        public IsClimbJumpTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mMovement = actor.GetComponent<Movement>();
        }
        public override bool IsTransition()
        {
            return mMovement.MovementCompensator.IsClimb;
        }
    }

    public class IsClimbTransition : StateTransitionBase
    {
        readonly private PlayableChracterController mController;
        public IsClimbTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return mController.Rigidbody.linearVelocity.y <= 0.0f;
        }
    }
}