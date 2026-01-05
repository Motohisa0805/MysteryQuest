using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ジャンプせず一番高い所に登る状態のジャンプ状態
    [System.Serializable]
    public class ClimbJumpState : StateBase<string>
    {
        public static readonly string mStateKey = "ClimbJump";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private Animator mAnimator;

        private Movement mMovement;

        private ImpactChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ClimbState.mStateKey)) { re.Add(new IsClimbTransition(actor, StateChanger, ClimbState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mMovement = actor.GetComponent<Movement>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", -1);
            mAnimator.SetInteger("climbState", 0);

            mMovement.ClimbJump(mController.Movement.MovementCompensator.StepGoalPosition.y - mController.Movement.MovementCompensator.StepStartPosition.y);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mController.Movement.MovementCompensator.StepStartPosition = mController.transform.position;
        }
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyImpactPower(collision);
        }
    }

    //ジャンプした後に登る状態のジャンプ状態
    [System.Serializable]
    public class ClimbState : StateBase<string>
    {
        public static readonly string mStateKey = "Climb";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private Animator mAnimator;

        private ImpactChecker mImpactChecker;

        [SerializeField]
        private float mClimbJumpingTime;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType5(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
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
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyImpactPower(collision);
        }
    }
}
