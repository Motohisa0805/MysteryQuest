using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class FocusingMoveState : StateBase<string>
    {
        public static readonly string mStateKey = "FocusingMove";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private DamageChecker mImpactChecker;

        private TargetSearch mTargetSearch;

        private EquipmentController mEquipmentController;

        private PropsObjectChecker mChecker;

        private float mInitModeType;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType6(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType3(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(ToLiftIdleState.mStateKey)) { re.Add(new IsToLiftIdleTransition(actor, StateChanger, ToLiftIdleState.mStateKey)); }
            if (StateChanger.IsContain(ToLiftRunState.mStateKey)) { re.Add(new IsToLiftMoveTransition(actor, StateChanger, ToLiftRunState.mStateKey)); }
            if (StateChanger.IsContain(ReleaseLiftState.mStateKey)) { re.Add(new IsReleaseLiftTransition(actor, StateChanger, ReleaseLiftState.mStateKey)); }

            if (StateChanger.IsContain(ReadyFirstAttackState.mStateKey)) { re.Add(new IsReadyFirstAttackTransition(actor, StateChanger, ReadyFirstAttackState.mStateKey)); }
            if (StateChanger.IsContain(WeaponTakingOutState.mStateKey)) { re.Add(new IsTakingOutTransition(actor, StateChanger, WeaponTakingOutState.mStateKey)); }
            if (StateChanger.IsContain(WeaponStorageState.mStateKey)) { re.Add(new IsStorageTransition(actor, StateChanger, WeaponStorageState.mStateKey)); }
            if (StateChanger.IsContain(StandingToCrouchState.mStateKey)) { re.Add(new IsStandingToCrouchTransition(actor, StateChanger, StandingToCrouchState.mStateKey)); }
            if (StateChanger.IsContain(ToLiftState.mStateKey)) { re.Add(new IsIdleToToLiftTransition(actor, StateChanger, ToLiftState.mStateKey)); }
            if (StateChanger.IsContain(ThrowStartState.mStateKey)) { re.Add(new IsThrowStartTransition(actor, StateChanger, ThrowStartState.mStateKey)); }
            if (StateChanger.IsContain(PushStartState.mStateKey)) { re.Add(new IsPushStartTransition(actor, StateChanger, PushStartState.mStateKey)); }
            //if (StateChanger.IsContain(JumpUpState.mStateKey)) { re.Add(new IsJumpUpTransition(actor, StateChanger, JumpUpState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpState.mStateKey)) { re.Add(new IsClimbJumpTransition(actor, StateChanger, ClimbJumpState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            TPSCamera.FocusingTarget = mTargetSearch.TargetObject;

            TPSCamera.CameraType = TPSCamera.Type.Focusing;
            mAnimationFunction.SetModeBlend(2);

            mAnimationFunction.Animator.SetBool("up Right Arm", mEquipmentController.SwordStick.IsHasStuckObject);
            if (mEquipmentController.SwordStick.IsHasStuckObject)
            {
                mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 1);
            }
            else if(!mChecker.HasTakedObject)
            {
                mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            }
            PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Left, "アタック");
            PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Down, "走る");
        }

        public override void Execute_Update(float time)
        {
            mController.Movement.ClimbCheck();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateFocusingMoveAnimation();
            mAnimationFunction.SpritDushClear();
            mChecker.UpdateTakedObjectPosition();
            base.Execute_Update(time);
            mController.StatusManager.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusManager.PlayerStatusData.MaxSpeed, mController.StatusManager.PlayerStatusData.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.BodyRotate();
        }

        public override void Execute_IKAnimatorUpdate(float time)
        {
            base.Execute_IKAnimatorUpdate(time);
            mAnimationFunction.FootIK.Update();
        }

        public override void Exit()
        {
            base.Exit();
            //mAnimationFunction.MoveStateClear();
            TPSCamera.CameraType = TPSCamera.Type.Free;
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
