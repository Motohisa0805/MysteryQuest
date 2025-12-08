using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class FallState : StateBase<string>
    {
        public static readonly string mStateKey = "Fall";
        public override string Key => mStateKey;

        PlayableChracterController mController;

        Animator mAnimator;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransition(actor, StateChanger, JumpDownState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 1);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.MaxSpeed, 5);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
