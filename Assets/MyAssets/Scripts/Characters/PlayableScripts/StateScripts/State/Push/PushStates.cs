using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class PushStartState : StateBase<string>
    {
        public static readonly string mStateKey = "PushStart";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PropsObjectChecker mPropsChecker;

        private DamageChecker mImpactChecker;

        [SerializeField]
        private float mPushPower;

        [SerializeField]
        private float mBasePushSpeed;

        [SerializeField]
        private float mMinSpeed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(PushingState.mStateKey)) { re.Add(new IsPushingTransition(actor, StateChanger, PushingState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsPushEndTransition(actor, StateChanger, MoveState.mStateKey)); }
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
            mPropsChecker = actor.GetComponent<PropsObjectChecker>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();

            mAnimationFunction.Animator.SetInteger("pushState", 0);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            PlayerStatusManager.Instance.ChangeSP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            float objectMass = mPropsChecker.LargeObjectMass;

            float dampingFactor = mPushPower / objectMass;

            float finalSpeed = mBasePushSpeed * dampingFactor;

            // ë¨ìxÇ…è„å¿Ç∆â∫å¿Çê›íË
            finalSpeed = Mathf.Clamp(finalSpeed, mMinSpeed, mBasePushSpeed);

            mController.Movement.PushObjectMove(finalSpeed);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("pushState", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }


    [Serializable]
    public class PushingState : StateBase<string>
    {
        public static readonly string mStateKey = "Pushing";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PropsObjectChecker mPropsChecker;

        private DamageChecker mImpactChecker;

        [SerializeField]
        private float mPushPower;

        [SerializeField]
        private float mBasePushSpeed;

        [SerializeField]
        private float mMinSpeed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsPushEndTransition(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(PushEndState.mStateKey)) { re.Add(new IsPushEndStartTransition(actor, StateChanger, PushEndState.mStateKey)); }
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
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mPropsChecker = actor.GetComponent<PropsObjectChecker>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("pushState", 1);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            PlayerStatusManager.Instance.ChangeSP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            float objectMass = mPropsChecker.LargeObjectMass;

            float dampingFactor = mPushPower / objectMass;

            float finalSpeed = mBasePushSpeed * dampingFactor;

            // ë¨ìxÇ…è„å¿Ç∆â∫å¿Çê›íË
            finalSpeed = Mathf.Clamp(finalSpeed, mMinSpeed, mBasePushSpeed);

            mController.Movement.PushObjectMove(finalSpeed);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("pushState", -1);
        }
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

    [Serializable]
    public class PushEndState : StateBase<string>
    {
        public static readonly string mStateKey = "PushEnd";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PropsObjectChecker mPropsChecker;

        private DamageChecker mImpactChecker;

        [SerializeField]
        private float mPushPower;

        [SerializeField]
        private float mBasePushSpeed;

        [SerializeField]
        private float mMinSpeed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType4(actor, StateChanger, IdleState.mStateKey)); }
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
            mPropsChecker = actor.GetComponent<PropsObjectChecker>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("pushState", 2);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            PlayerStatusManager.Instance.ChangeSP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            float objectMass = mPropsChecker.LargeObjectMass;

            float dampingFactor = mPushPower / objectMass;

            float finalSpeed = mBasePushSpeed * dampingFactor;

            // ë¨ìxÇ…è„å¿Ç∆â∫å¿Çê›íË
            finalSpeed = Mathf.Clamp(finalSpeed, mMinSpeed, mBasePushSpeed);

            mController.Movement.PushObjectMove(finalSpeed);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("pushState", -1);
        }
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
