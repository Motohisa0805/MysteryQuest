using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ÉWÉÉÉìÉvíÜÇ…äRÇìoÇÈéûÇÃìÆçÏ
    [System.Serializable]
    public class ClimbJumpingState : StateBase<string>
    {
        public static readonly string       mStateKey = "ClimbJumping";
        public override string              Key => mStateKey;

        private PlayableChracterController  mController;

        private PlayableInput               mPlayableInput;

        private PlayableAnimationFunction   mAnimationFunction;

        private DamageChecker               mImpactChecker;

        [SerializeField]
        private float                       mClimbJumpingTime;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType5(actor, StateChanger, IdleState.mStateKey)); }
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
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("jumpState", -1);
            mAnimationFunction.Animator.SetInteger("climbState", 0);
            mController.Movement.ClimbJumpingTimer.Start(mClimbJumpingTime);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mController.StatusManager.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.Movement.Climb();
            base.Execute_FixedUpdate(time);
            //mController.BodyRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("climbState", -1);
            mController.Movement.MovementCompensator.ClearStepFunc(false);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
