using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class JumpUpState : StateBase<string>
    {
        public static readonly string mStateKey = "Jump";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private Animator mAnimator;//アニメーター
        private ImpactChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransitionType2(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(JumpingState.mStateKey)) { re.Add(new IsJumpLoopTransition(actor, StateChanger, JumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 0);
            Vector3 vel = mController.Rigidbody.linearVelocity;
            vel.y = 0;
            if (vel.magnitude > 0.0f)
            {
                mController.Movement.Jump(mController.StatusProperty.MoveJumpPower);
            }
            else
            {
                mController.Movement.Jump(mController.StatusProperty.IdleJumpPower);
            }
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);

        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, 5);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("jumpState", -1);
        }
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            if (collision.collider.GetComponent<ChemistryObject>() != null)
            {
                mImpactChecker.ApplyImpactPower(collision);
            }
        }

    }

    [System.Serializable]
    public class JumpingState : StateBase<string>
    {
        public static readonly string mStateKey = "Jumping";
        public override string Key => mStateKey;

        PlayableChracterController mController;

        private Animator mAnimator;//アニメーター

        private ImpactChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransition(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 1);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("jumpState", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            if (collision.collider.GetComponent<ChemistryObject>() != null)
            {
                mImpactChecker.ApplyImpactPower(collision);
            }
        }
    }

    [System.Serializable]
    public class JumpDownState : StateBase<string>
    {
        public static readonly string mStateKey = "JumpDown";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private Animator mAnimator;//アニメーター

        private ImpactChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType3(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType2(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(SmallImpactPlayerState.mStateKey)) { re.Add(new IsSmallImpactTransition(actor, StateChanger, SmallImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 2);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("jumpState", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            if (collision.collider.GetComponent<ChemistryObject>() != null)
            {
                mImpactChecker.ApplyImpactPower(collision);
            }
        }
    }
}
