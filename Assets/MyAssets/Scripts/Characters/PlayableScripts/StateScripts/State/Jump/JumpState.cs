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

        [SerializeField, Tooltip("ジャンプ力")]
        private float mJumpPower;//ジャンプ力


        [SerializeField, Tooltip("アニメーション")]
        private Animator mAnimator;//アニメーター

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransitionType2(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(JumpingState.mStateKey)) { re.Add(new IsJumpLoopTransition(actor, StateChanger, JumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 0);
            mController.Movement.Jump(mJumpPower);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);

        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            mController.InputVelocity();
            mController.Movement.Move(mController.MaxSpeed, 5);
            base.Execute_FixedUpdate(time);
            mController.RotateBody();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    [System.Serializable]
    public class JumpingState : StateBase<string>
    {
        public static readonly string mStateKey = "Jumping";
        public override string Key => mStateKey;
        PlayableChracterController mController;


        [SerializeField, Tooltip("アニメーション")]
        private Animator mAnimator;//アニメーター

        [SerializeField]
        private float mAcceleration; //加速度

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransition(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
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
            //mController.Movement.Gravity();
            mController.InputVelocity();
            mController.Movement.Move(mController.MaxSpeed, mAcceleration);
            base.Execute_FixedUpdate(time);
            mController.RotateBody();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    [System.Serializable]
    public class JumpDownState : StateBase<string>
    {
        public static readonly string mStateKey = "JumpDown";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private Rigidbody mRigidbody;//リジッドボディ

        private Animator mAnimator;//アニメーター

        [SerializeField]
        private float mAcceleration; //加速度

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType3(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType2(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mRigidbody = actor.GetComponent<Rigidbody>();
            mAnimator = actor.GetComponentInChildren<Animator>();
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
            //mController.Movement.Gravity();
            mController.InputVelocity();
            mController.Movement.Move(mController.MaxSpeed, mAcceleration);
            base.Execute_FixedUpdate(time);
            mController.RotateBody();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("jumpState", -1);
        }
    }
}
