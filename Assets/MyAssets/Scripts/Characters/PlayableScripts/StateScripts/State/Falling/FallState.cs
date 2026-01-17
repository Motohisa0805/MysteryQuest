using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class FallState : StateBase<string>
    {
        public static readonly string mStateKey = "Fall";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private CapsuleColliderController mColliderController;

        private DamageChecker mImpactChecker;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsFallBigImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransition(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("jumpState", 1);
            mAnimationFunction.Animator.SetInteger("crouchState", -1);
            mAnimationFunction.Animator.SetInteger("to Lift", -1);
            mAnimationFunction.Animator.SetInteger("pushState", -1);
            TPSCamera.CameraType = TPSCamera.Type.Free;
            mColliderController.ResetCollider();
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, 5);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("jumpState", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
