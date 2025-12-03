using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class ClimbJumpingState : StateBase<string>
    {
        public static readonly string mStateKey = "ClimbJumping";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private Rigidbody mRigidbody;

        private Animator mAnimator;

        [SerializeField]
        private float mClimbJumpingTime;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType5(actor, StateChanger, IdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mRigidbody = actor.GetComponent<Rigidbody>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", -1);
            mAnimator.SetInteger("climbState", 0);
            mController.Movement.ClimbJumpingTimer.Start(mClimbJumpingTime);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.Movement.Climb();
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("climbState", -1);
            mController.Movement.MovementCompensator.ClearStepFunc(false);
        }
    }
}
