using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class SmallImpactPlayerState : StateBase<string>
    {
        public static readonly string mStateKey = "SmallImpact";
        public override string Key => mStateKey;
        private Animator mAnimator;
        private PlayableAnimationFunction mAnimationFunction;
        private ImpactChecker mImpactChecker;
        private PlayableChracterController mPlayableChracterController;
        private PropsObjectChecker mChecker;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsSmallImpactToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsSmallImpactToIdle2Transition(actor, StateChanger, IdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mAnimator = actor.GetComponentInChildren<Animator>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponentInChildren<ImpactChecker>();
            mPlayableChracterController = actor.GetComponent<PlayableChracterController>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("to Lift", -1);
            mAnimator.SetInteger("crouchState", -1);
            TPSCamera.CameraType = TPSCamera.Type.Free;
            if (mPlayableChracterController.Grounded)
            {
                mAnimator.SetInteger("impact State", 0);
            }
            else
            {
                mAnimator.SetInteger("impact State", -2);
            }
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);

            mChecker.SetReleaseTakedObject();
            PlayerStatusManager.Instance.ChangeHP(-mImpactChecker.GetCalculatedDamage());
            SoundManager.Instance.PlayOneShot3D(SoundList.SEType.Damage, mImpactChecker.transform);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("impact State", -1);
            mImpactChecker.ClearImpactPower();
        }
    }

    [Serializable]
    public class BigImpactPlayerState : StateBase<string>
    {
        public static readonly string mStateKey = "BigImpact";
        public override string Key => mStateKey;
        private Animator mAnimator;
        private PlayableAnimationFunction mPlayableAnimationFunction;
        private RagdollController mRagdollController;
        private CapsuleColliderController mCapsuleColliderController;

        private ImpactChecker mImpactChecker;

        private PropsObjectChecker mChecker;

        private Timer mTimer = new Timer();
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(StandingUpState.mStateKey)) { re.Add(new IsImpactToStandingUpTransition(actor, StateChanger, StandingUpState.mStateKey)); }
            return re;
        }

        private void ImpactPowerReset()
        {
            mImpactChecker.ClearImpactPower();
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mAnimator = actor.GetComponentInChildren<Animator>();
            mPlayableAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
            mRagdollController = actor.GetComponentInChildren<RagdollController>();
            mCapsuleColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mTimer.OnEnd += ImpactPowerReset;
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("to Lift", -1);
            mAnimator.SetInteger("crouchState", -1);
            mPlayableAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            mPlayableAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);
            mPlayableAnimationFunction.SetAnimatorEnabled(false);

            TPSCamera.CameraType = TPSCamera.Type.Free;
            mRagdollController.SetEnabledRagdoll(true);
            mCapsuleColliderController.SetRagdollModeCollider();

            mTimer.Start(1.0f);
            mChecker.SetReleaseTakedObject();

            //ƒeƒXƒg
            PlayerStatusManager.Instance.ChangeHP(-mImpactChecker.GetCalculatedDamage());
            SoundManager.Instance.PlayOneShot3D(SoundList.SEType.Damage, mImpactChecker.transform);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mTimer.Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mPlayableAnimationFunction.SetAnimatorEnabled(true);
            mRagdollController.SetEnabledRagdoll(false);
            mCapsuleColliderController.ResetCollider();
        }
    }
    [Serializable]
    public class StandingUpState : StateBase<string>
    {
        public static readonly string mStateKey = "StandingUp";
        public override string Key => mStateKey;
        private Animator mAnimator;
        private Movement mMovement;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsStandingUpToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mAnimator = actor.GetComponentInChildren<Animator>();
            mMovement = actor.GetComponent<Movement>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("standing Up State", 0);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
            mMovement.Stop();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("standing Up State", -1);
        }
    }
}
