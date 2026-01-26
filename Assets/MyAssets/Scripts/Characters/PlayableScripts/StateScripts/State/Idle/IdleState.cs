using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class IdleState : StateBase<string>
    {
        public static readonly string   mStateKey = "Idle";
        public override string          Key => mStateKey;

        PlayableChracterController      mController;

        PlayableAnimationFunction       mAnimationFunction;

        private Movement                mMovement;

        DamageChecker                   mImpactChecker;

        private EquipmentController     mEquipmentController;

        private PlayableInput           mPlayableInput;

        private TargetSearch            mTargetSearch;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransition(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(ReadyFirstAttackState.mStateKey)) { re.Add(new IsReadyFirstAttackTransition(actor, StateChanger, ReadyFirstAttackState.mStateKey)); }
            if (StateChanger.IsContain(WeaponTakingOutState.mStateKey)) { re.Add(new IsTakingOutTransition(actor, StateChanger, WeaponTakingOutState.mStateKey)); }
            if (StateChanger.IsContain(WeaponStorageState.mStateKey)) { re.Add(new IsStorageTransition(actor, StateChanger, WeaponStorageState.mStateKey)); }
            if (StateChanger.IsContain(FocusingMoveState.mStateKey)) { re.Add(new IsFocusingMoveTransition(actor, StateChanger, FocusingMoveState.mStateKey)); }
            if (StateChanger.IsContain(JumpUpState.mStateKey)) { re.Add(new IsJumpUpTransition(actor, StateChanger, JumpUpState.mStateKey)); }
            if (StateChanger.IsContain(StandingToCrouchState.mStateKey)) { re.Add(new IsStandingToCrouchTransition(actor, StateChanger, StandingToCrouchState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpState.mStateKey)) { re.Add(new IsClimbJumpTransition(actor, StateChanger, ClimbJumpState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(ToLiftState.mStateKey)) { re.Add(new IsIdleToToLiftTransition(actor, StateChanger, ToLiftState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            if (StateChanger.IsContain(OutOfBodyExperienceState.mStateKey)) { re.Add(new IsOutOfBodyExperienceTransition(actor, StateChanger, OutOfBodyExperienceState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mMovement = actor.GetComponent<Movement>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("to Lift", -1);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0,true);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0,true);
            if (mEquipmentController.IsBattleMode)
            {
                mAnimationFunction.SetModeBlend(1);
            }
            else
            {
                mAnimationFunction.SetModeBlend(0);
            }
            if(mEquipmentController.SwordStick)
            {
                mAnimationFunction.Animator.SetBool("up Right Arm", mEquipmentController.SwordStick.IsHasStuckObject);
                if (mEquipmentController.SwordStick.IsHasStuckObject)
                {
                    mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 1);
                }
                else
                {
                    mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
                }
            }
            if(PlayerUIManager.Instance)
            {
                if(PlayerUIManager.Instance.ActionButtonController)
                {
                    PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Left,"アタック");
                    PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Up,"ジャンプ");
                }
            }
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mAnimationFunction.UpdateLayerWeight();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateIdleToRunAnimation();
            mController.StatusManager.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusManager.PlayerStatusData.MaxSpeed, mController.StatusManager.PlayerStatusData.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Execute_IKAnimatorUpdate(float time)
        {
            base.Execute_IKAnimatorUpdate(time);
            mAnimationFunction.FootIK.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
