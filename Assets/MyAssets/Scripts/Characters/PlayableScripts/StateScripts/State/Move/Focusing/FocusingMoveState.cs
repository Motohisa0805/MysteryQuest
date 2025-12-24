using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class FocusingMoveState : StateBase<string>
    {
        public static readonly string mStateKey = "FocusingMove";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        PlayableAnimationFunction mAnimationFunction;

        ImpactChecker mImpactChecker;

        private TargetSearch mTargetSearch;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType6(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType3(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(JumpUpState.mStateKey)) { re.Add(new IsJumpUpTransition(actor, StateChanger, JumpUpState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
        }

        public override void Enter()
        {
            base.Enter();
            TPSCamera.FocusingTarget = mTargetSearch.TargetObject;

            mAnimationFunction.MoveStateClear();
            mAnimationFunction.SetModeBlend(2);
        }

        public override void Execute_Update(float time)
        {
            mAnimationFunction.UpdateFocusingMoveAnimation();
            mAnimationFunction.SpritDushClear();

            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FocusingRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.MoveStateClear();
            mAnimationFunction.SetModeBlend(0);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            if (collision.collider.GetComponent<ChemistryObject>() != null)
            {
                mImpactChecker.ApplyImpactPower(collision);
            }
        }
    }
}
