using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class FirstAttackState : StateBase<string>
    {
        public static readonly string mStateKey = "FirstAttack";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private DamageChecker mImpactChecker;

        private EquipmentController mEquipmentController;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(SecondAttackState.mStateKey)) { re.Add(new IsSecondAttackTransition(actor, StateChanger, SecondAttackState.mStateKey)); }
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType8(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType5(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override List<ActionButtonInfo> GetActionButtons()
        {
            return new List<ActionButtonInfo>()
            {
                new ActionButtonInfo((int)ActionButtonController.ActionButtonTag.Left, "çUåÇ")
            };
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
                //ïêäÌÇÃëfêUÇËSE
                SoundManager.Instance.PlayOneShot3D("Attack_Player", hand.transform.position, mController.transform);
                mEquipmentController.SwingEffectHandler.ActivateSlachEffect(true);
            }
            mAnimationFunction.Animator.SetInteger("attack State", 1);
        }

        public override void Execute_Update(float time)
        {
            if(mAnimationFunction.IsAnimationClipEnd("first Attack",0.5f))
            {
                mController.HandTransforms[(int)SetItemTransform.TransformType.Right].GetCollider().enabled = false;
            }
            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // IdleèÛë‘ÇÃì¡íËÇÃï®óùèàóùÇÇ±Ç±Ç…í«â¡Ç≈Ç´Ç‹Ç∑
            // ó·: èdóÕÇÃìKópÅAè’ìÀîªíËÇ»Ç«
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
