using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class WeaponTakingOutState : StateBase<string>
    {
        public static readonly string mStateKey = "TakinOut";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        private DamageChecker mImpactChecker;

        private PlayableInput mPlayableInput;

        private EquipmentController mEquipmentController;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType7(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType4(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(FocusingMoveState.mStateKey)) { re.Add(new IsFocusingMoveTransition2(actor, StateChanger, FocusingMoveState.mStateKey)); }
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
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.SetBattleMode(true);
            mAnimationFunction.SetToolState(true);
            mEquipmentController.ChangeParent(SetItemTransform.TransformType.Weapon,SetItemTransform.TransformType.Right);
            mEquipmentController.IsBattleMode = true;
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1,1);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(3, 1,true);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            mAnimationFunction.UpdateLayerWeight();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateFocusingMoveAnimation();
            mAnimationFunction.SpritDushClear();
            base.Execute_Update(time);
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
            mAnimationFunction.SetToolState(false);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
