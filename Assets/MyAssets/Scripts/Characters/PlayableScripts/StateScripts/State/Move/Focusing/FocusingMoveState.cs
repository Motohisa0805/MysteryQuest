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

        private EquipmentController mEquipmentController;

        private float mInitModeType;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType6(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType3(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(ReadyFirstAttackState.mStateKey)) { re.Add(new IsReadyFirstAttackTransition(actor, StateChanger, ReadyFirstAttackState.mStateKey)); }
            if (StateChanger.IsContain(WeaponTakingOutState.mStateKey)) { re.Add(new IsTakingOutTransition(actor, StateChanger, WeaponTakingOutState.mStateKey)); }
            if (StateChanger.IsContain(WeaponStorageState.mStateKey)) { re.Add(new IsStorageTransition(actor, StateChanger, WeaponStorageState.mStateKey)); }
            if (StateChanger.IsContain(StandingToCrouchState.mStateKey)) { re.Add(new IsStandingToCrouchTransition(actor, StateChanger, StandingToCrouchState.mStateKey)); }
            if (StateChanger.IsContain(PushStartState.mStateKey)) { re.Add(new IsPushStartTransition(actor, StateChanger, PushStartState.mStateKey)); }
            if (StateChanger.IsContain(JumpUpState.mStateKey)) { re.Add(new IsJumpUpTransition(actor, StateChanger, JumpUpState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpState.mStateKey)) { re.Add(new IsClimbJumpTransition(actor, StateChanger, ClimbJumpState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            TPSCamera.FocusingTarget = mTargetSearch.TargetObject;

            //mAnimationFunction.MoveStateClear();
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
            //mAnimationFunction.MoveStateClear();
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyImpactPower(collision);
        }
    }
}
