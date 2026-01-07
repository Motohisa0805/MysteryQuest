using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //予備動作
    [Serializable]
    public class ThrowStartState : StateBase<string>
    {
        public static readonly string mStateKey = "ThrowStart";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        private Animator mAnimator;//アニメーター

        private Movement mMovement;

        private PropsObjectChecker mChecker;

        private ImpactChecker mImpactChecker;

        [SerializeField]
        private float mThrowPower;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ThrowIdleState.mStateKey)) { re.Add(new IsThrowIdleTransition(actor, StateChanger, ThrowIdleState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mMovement = actor.GetComponent<Movement>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("to Lift", 2);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);
            TPSCamera.CameraType = TPSCamera.Type.ShoulderView;
            PlayerUIManager.Instance.ThrowCircle.gameObject.SetActive(true);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectThrowDirection(mThrowPower);
            mChecker.UpdateTakedObjectPosition();
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
            mImpactChecker.ApplyImpactPower(collision);
        }
    }
    [Serializable]
    public class ThrowIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "ThrowIdle";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PropsObjectChecker mChecker;

        private Animator mAnimator;//アニメーター

        private Movement mMovement;

        private ImpactChecker mImpactChecker;

        [SerializeField]
        private float mThrowPower;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ThrowingState.mStateKey)) { re.Add(new IsThrowingTransition(actor, StateChanger, ThrowingState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mMovement = actor.GetComponent<Movement>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("to Lift", 3);
        }

        public override void Execute_Update(float time)
        {
            mChecker.UpdateTakedObjectThrowDirection(mThrowPower);
            mChecker.UpdateTakedObjectPosition();
            base.Execute_Update(time);
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
            mImpactChecker.ApplyImpactPower(collision);
        }
    }
    [Serializable]
    public class ThrowingState : StateBase<string>
    {
        public static readonly string mStateKey = "Throwing";
        public override string Key => mStateKey;

        private PropsObjectChecker mChecker;

        private Animator mAnimator;//アニメーター

        private Movement mMovement;

        private ImpactChecker mImpactChecker;

        [SerializeField]
        private float mThrowPower;

        private bool mThrowed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsThrowingToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mMovement = actor.GetComponent<Movement>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("to Lift", 4);

            mThrowed = false;
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectPosition();
            bool flag = mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && mAnimator.GetCurrentAnimatorStateInfo(0).IsName("throwing");
            if (!mThrowed && flag)
            {
                mChecker.Throw(mThrowPower);
                mThrowed = true;
            }
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
            mMovement.Stop();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("to Lift", -1);
            mThrowed = false;
            TPSCamera.CameraType = TPSCamera.Type.Free;
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyImpactPower(collision);
        }
    }

}