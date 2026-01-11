using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class SecondAttackState : StateBase<string>
    {
        public static readonly string mStateKey = "SecondAttack";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private DamageChecker mImpactChecker;

        private EquipmentController mEquipmentController;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(FirstAttackState.mStateKey)) { re.Add(new IsFirstAttackTransition2(actor, StateChanger, FirstAttackState.mStateKey)); }
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType9(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType6(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            Collider hand = mController.HandTransforms[(int)SetItemTransform.TransformType.Right].GetCollider();
            if (hand)
            {
                hand.enabled = true;
                //武器の素振りSE
                SoundManager.Instance.PlayOneShot3D(0, mController.transform);
                mEquipmentController.SwingEffectHandler.ActivateSlachEffect(true);
            }
            mAnimationFunction.Animator.SetInteger("attack State", 2);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            Collider hand = mController.HandTransforms[(int)SetItemTransform.TransformType.Right].GetCollider();
            if (hand)
            {
                hand.enabled = false;
                mEquipmentController.SwingEffectHandler.ActivateSlachEffect(false);
            }
            mAnimationFunction.Animator.SetInteger("attack State", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
