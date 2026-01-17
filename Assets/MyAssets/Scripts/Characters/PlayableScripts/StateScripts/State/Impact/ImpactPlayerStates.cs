using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class MediumImpactPlayerState : StateBase<string>
    {
        public static readonly string mStateKey = "MediumImpact";
        public override string Key => mStateKey;
        private PlayableAnimationFunction mAnimationFunction;
        private PlayableInput mPlayableInput;
        private DamageChecker mImpactChecker;
        private PlayableChracterController mPlayableChracterController;
        private PropsObjectChecker mChecker;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsMediumImpactToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsMediumImpactToIdle2Transition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponentInChildren<DamageChecker>();
            mPlayableChracterController = actor.GetComponent<PlayableChracterController>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("to Lift", -1);
            mAnimationFunction.Animator.SetInteger("crouchState", -1);
            TPSCamera.CameraType = TPSCamera.Type.Free;
            if (mPlayableChracterController.Grounded)
            {
                mAnimationFunction.Animator.SetInteger("impact State", 0);
            }
            else
            {
                mAnimationFunction.Animator.SetInteger("impact State", -2);
            }
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);

            mChecker.SetReleaseTakedObject();
            PlayerStatusManager.Instance.ChangeHP(-mImpactChecker.GetCalculatedDamage());
            SoundManager.Instance.PlayOneShot3D(1003, mImpactChecker.transform.position);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("impact State", -1);
            mImpactChecker.ClearImpactPower();
        }
    }

    [Serializable]
    public class BigImpactPlayerState : StateBase<string>
    {
        public static readonly string mStateKey = "BigImpact";
        public override string Key => mStateKey;
        private PlayableInput mPlayableInput;
        private PlayableAnimationFunction mPlayableAnimationFunction;
        private RagdollController mRagdollController;
        private CapsuleColliderController mCapsuleColliderController;

        private DamageChecker mImpactChecker;

        private PropsObjectChecker mChecker;

        private Timer mTimer = new Timer();
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(StandingUpState.mStateKey)) { re.Add(new IsImpactToStandingUpTransition(actor, StateChanger, StandingUpState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        private void ImpactPowerReset()
        {
            mImpactChecker.ClearImpactPower();
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mPlayableAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mRagdollController = actor.GetComponentInChildren<RagdollController>();
            mCapsuleColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mTimer.OnEnd += ImpactPowerReset;
        }

        public override void Enter()
        {
            base.Enter();
            mPlayableAnimationFunction.Animator.SetInteger("to Lift", -1);
            mPlayableAnimationFunction.Animator.SetInteger("crouchState", -1);
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
            SoundManager.Instance.PlayOneShot3D(1003, mImpactChecker.transform.position);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mTimer.Update(time);
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
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
        
        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

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
            mPlayableInput = actor.GetComponentInChildren<PlayableInput>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mMovement = actor.GetComponent<Movement>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("standing Up State", 0);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
            mMovement.Stop();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("standing Up State", -1);
        }
    }
    [Serializable]
    public class DeathPlayerState : StateBase<string>
    {
        public static readonly string mStateKey = "Death";
        public override string Key => mStateKey;
        private PlayableInput mPlayableInput;
        private PlayableAnimationFunction mPlayableAnimationFunction;
        private RagdollController mRagdollController;
        private CapsuleColliderController mCapsuleColliderController;

        private DamageChecker mImpactChecker;

        private PropsObjectChecker mChecker;

        private Timer mTimer = new Timer();
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            
            return re;
        }

        private void ImpactPowerReset()
        {
            mImpactChecker.ClearImpactPower();
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mPlayableInput = actor.GetComponentInChildren<PlayableInput>();
            mPlayableAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mRagdollController = actor.GetComponentInChildren<RagdollController>();
            mCapsuleColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mTimer.OnEnd += ImpactPowerReset;
        }

        public override void Enter()
        {
            base.Enter();
            mPlayableAnimationFunction.Animator.SetInteger("to Lift", -1);
            mPlayableAnimationFunction.Animator.SetInteger("crouchState", -1);
            mPlayableAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            mPlayableAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);
            mPlayableAnimationFunction.SetAnimatorEnabled(false);

            TPSCamera.CameraType = TPSCamera.Type.Free;
            mRagdollController.SetEnabledRagdoll(true);
            mCapsuleColliderController.SetRagdollModeCollider();

            mTimer.Start(1.0f);
            mChecker.SetReleaseTakedObject();

            PlayerStatusManager.Instance.ChangeHP(-mImpactChecker.GetCalculatedDamage());
            SoundManager.Instance.PlayOneShot3D(1003, mImpactChecker.transform.position);
            if(EventManager.Instance)
            {
                EventManager.Instance.DeathEvent().Forget();
            }
            else
            {
                Debug.LogWarning("Not Find EventManager" + mStateKey + "State");
            }
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mTimer.Update(time);
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
        }
    }
}
