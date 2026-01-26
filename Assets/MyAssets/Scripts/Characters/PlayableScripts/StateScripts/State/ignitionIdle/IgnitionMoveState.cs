using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class IgnitionMoveState : StateBase<string>
    {

        public static readonly string mStateKey = "IgnitionIdle";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        private DamageChecker mImpactChecker;

        private EquipmentController mEquipmentController;

        private PlayableInput mPlayableInput;

        private TargetSearch mTargetSearch;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType10(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetBool("up Right Arm", false);
            mAnimationFunction.Animator.SetInteger("attack State", -1);
            mAnimationFunction.Animator.SetInteger("ignitionI", 0);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 1,true);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            if(mPlayableInput.Focusing)
            {
                mAnimationFunction.SetModeBlend(2);
            }
            else
            {
                mAnimationFunction.SetModeBlend(1);
            }
            mAnimationFunction.UpdateLayerWeight();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateFocusingMoveAnimation();
            mAnimationFunction.SpritDushClear();
            base.Execute_Update(time);
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
            mController.BodyRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("ignitionI", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
