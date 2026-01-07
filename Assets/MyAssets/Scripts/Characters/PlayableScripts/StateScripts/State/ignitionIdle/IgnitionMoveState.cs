using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class IgnitionMoveState : StateBase<string>
    {

        public static readonly string mStateKey = "IgnitionIdle";
        public override string Key => mStateKey;

        PlayableChracterController mController;

        PlayableAnimationFunction mAnimationFunction;

        ImpactChecker mImpactChecker;

        private EquipmentController mEquipmentController;

        private PlayableInput mPlayableInput;

        private TargetSearch mTargetSearch;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType10(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
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
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetBool("up Right Arm", false);
            mAnimationFunction.Animator.SetInteger("attack State", -1);
            mAnimationFunction.Animator.SetInteger("ignitionI", 0);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 1);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);
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
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateFocusingMoveAnimation();
            mAnimationFunction.SpritDushClear();
            base.Execute_Update(time);
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateIdleToRunAnimation();
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
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
            mImpactChecker.ApplyImpactPower(collision);
        }
    }
}
