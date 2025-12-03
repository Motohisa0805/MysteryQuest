using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ジャンプせず一番高い所に登る状態のジャンプ状態
    [System.Serializable]
    public class ClimbJumpState : StateBase<string>
    {
        public static readonly string mStateKey = "ClimbJump";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private Rigidbody mRigidbody;

        private Animator mAnimator;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ClimbState.mStateKey)) { re.Add(new IsClimbTransition(actor, StateChanger, ClimbState.mStateKey)); }
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
            mAnimator.SetInteger("jumpState", -1);
            mAnimator.SetInteger("climbState", 0);

            float h = mController.Movement.MovementCompensator.StepGoalPosition.y - mController.Movement.MovementCompensator.StepStartPosition.y;

            float g = Mathf.Abs(Physics.gravity.y) * 2.0f;
            float requiredVelocityY = Mathf.Sqrt(2 * g * h);

            // 必要な垂直速度を瞬間的に加える
            mRigidbody.AddForce(Vector3.up * requiredVelocityY, ForceMode.VelocityChange);
        }

        public override void Execute_FixedUpdate(float time)
        {
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mController.Movement.MovementCompensator.StepStartPosition = mController.transform.position;
        }
    }

    //ジャンプした後に登る状態のジャンプ状態
    [System.Serializable]
    public class ClimbState : StateBase<string>
    {
        public static readonly string mStateKey = "Climb";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private Rigidbody mRigidbody;

        private Animator mAnimator;

        [SerializeField]
        private float mClimbJumpingTime;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType5(actor, StateChanger, IdleState.mStateKey)); }
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
            mController.Movement.ClimbJumpingTimer.Start(mClimbJumpingTime);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.Movement.Climb();
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("climbState", -1);
            mController.Movement.MovementCompensator.ClearStepFunc(false);
        }
    }
}
