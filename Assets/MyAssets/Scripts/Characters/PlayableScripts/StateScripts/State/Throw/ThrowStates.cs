using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ó\îıìÆçÏ
    [Serializable]
    public class ThrowStartState : StateBase<string>
    {
        public static readonly string mStateKey = "ThrowStart";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private Movement mMovement;

        private PropsObjectChecker mChecker;

        private DamageChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ThrowIdleState.mStateKey)) { re.Add(new IsThrowIdleTransition(actor, StateChanger, ThrowIdleState.mStateKey)); }
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
            mMovement = actor.GetComponent<Movement>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("to Lift", 2);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);
            TPSCamera.CameraType = TPSCamera.Type.ShoulderView;
            PlayerUIManager.Instance.ThrowCircle.gameObject.SetActive(true);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectThrowDirection(PlayerStatusManager.Instance.PlayerStatusData.ThrowPower);
            mChecker.UpdateTakedObjectPosition();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
            mController.ShoulderViewRotate();
            mMovement.Stop();
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
    [Serializable]
    public class ThrowIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "ThrowIdle";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        private PlayableInput mPlayableInput;

        private PropsObjectChecker mChecker;

        private Movement mMovement;

        private DamageChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ThrowingState.mStateKey)) { re.Add(new IsThrowingTransition(actor, StateChanger, ThrowingState.mStateKey)); }
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
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mPlayableInput = actor.GetComponentInChildren<PlayableInput>();
            mMovement = actor.GetComponent<Movement>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("to Lift", 3);
        }

        public override void Execute_Update(float time)
        {
            mChecker.UpdateTakedObjectThrowDirection(PlayerStatusManager.Instance.PlayerStatusData.ThrowPower);
            mChecker.UpdateTakedObjectPosition();
            base.Execute_Update(time);
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
            mController.ShoulderViewRotate();
            mMovement.Stop();
        }

        public override void Exit()
        {
            base.Exit();
            PlayerUIManager.Instance.ThrowCircle.gameObject.SetActive(false);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
    [Serializable]
    public class ThrowingState : StateBase<string>
    {
        public static readonly string mStateKey = "Throwing";
        public override string Key => mStateKey;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PropsObjectChecker mChecker;

        private Movement mMovement;

        private DamageChecker mImpactChecker;

        private bool mThrowed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsThrowingToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mMovement = actor.GetComponent<Movement>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("to Lift", 4);

            mThrowed = false;
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectPosition();
            bool flag = mAnimationFunction.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f && mAnimationFunction.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && mAnimationFunction.Animator.GetCurrentAnimatorStateInfo(0).IsName("throwing");
            if (!mThrowed && flag)
            {
                mChecker.Throw(PlayerStatusManager.Instance.PlayerStatusData.ThrowPower);
                mThrowed = true;
            }
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
            mAnimationFunction.Animator.SetInteger("to Lift", -1);
            mThrowed = false;
            TPSCamera.CameraType = TPSCamera.Type.Free;
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

}