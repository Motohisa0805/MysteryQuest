using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class FallState : StateBase<string>
    {
        public static readonly string mStateKey = "Fall";
        public override string Key => mStateKey;

        PlayableChracterController mController;

        private CapsuleColliderController mColliderController;

        Animator mAnimator;

        private ImpactChecker mImpactChecker;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransition(actor, StateChanger, JumpDownState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 1);
            mAnimator.SetInteger("crouchState", -1);
            mAnimator.SetInteger("to Lift", -1);
            mAnimator.SetInteger("pushState", -1);
            TPSCamera.CameraType = TPSCamera.Type.Free;
            mColliderController.ResetCollider();
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
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            if (collision.collider.GetComponent<ChemistryObject>() != null)
            {
            }
                mImpactChecker.ApplyImpactPower(collision);
        }
    }
}
