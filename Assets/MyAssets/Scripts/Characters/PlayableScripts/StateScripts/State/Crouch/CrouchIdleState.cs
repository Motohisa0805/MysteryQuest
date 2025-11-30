using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class StandingToCrouchState : StateBase<string>
    {
        public static readonly string mStateKey = "StandingToCrouch";

        public override string Key => mStateKey;


        private CapsuleColliderController mColliderController;

        private Animator mAnimator;

        private PlayableAnimationFunction mAnimationFunction;


        [SerializeField]
        private float mCrouchHeight;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(CrouchIdleState.mStateKey)) { re.Add(new IsCrouchIdleTransition(actor, StateChanger, CrouchIdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
        }

        public override void Enter()
        {
            base.Enter();

            mAnimator.SetInteger("crouchState", 0);

            float standingHeight = mColliderController.CapsuleCollider.height;
            float crouchHeight = mCrouchHeight;
            float crouchCenter_Y = mColliderController.CapsuleCollider.center.y + (standingHeight - crouchHeight) / 2;
            mColliderController.SetHeight(crouchHeight);
            Vector3 c = mColliderController.CapsuleCollider.center;
            mColliderController.SetCenter(new Vector3(c.x, -crouchCenter_Y, c.z));
        }

    }

    [System.Serializable]
    public class CrouchIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "CrouchIdle";

        public override string Key => mStateKey;


        private Animator mAnimator;

        private PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(CrouchToStandingState.mStateKey)) { re.Add(new IsCrouchToStandingTransition(actor, StateChanger, CrouchToStandingState.mStateKey)); }
            if (StateChanger.IsContain(CrouchWalkState.mStateKey)) { re.Add(new IsCrouch_IdleToWalkTransition(actor, StateChanger, CrouchWalkState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mAnimator = actor.GetComponentInChildren<Animator>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("crouchState", 1);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateCrouchAnimation();
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.Movement.Move(mController.CrouchMaxSpeed,0);
            base.Execute_FixedUpdate(time);
        }
    }

    [System.Serializable]
    public class CrouchWalkState : StateBase<string>
    {
        public static readonly string mStateKey = "CrouchWalk";

        public override string Key => mStateKey;


        private Animator mAnimator;

        private PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        [SerializeField]
        private float mAcceleration; //加速度

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(CrouchIdleState.mStateKey)) { re.Add(new IsCrouch_WalkToIdleTransition(actor, StateChanger, CrouchIdleState.mStateKey)); }
            if (StateChanger.IsContain(CrouchToStandingState.mStateKey)) { re.Add(new IsCrouchToStandingTransition(actor, StateChanger, CrouchToStandingState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mAnimator = actor.GetComponentInChildren<Animator>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
        }

        public override void Enter()
        {
            base.Enter();

        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateCrouchAnimation();
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            mController.Movement.Move(mController.CrouchMaxSpeed, mAcceleration);
            base.Execute_FixedUpdate(time);
            mController.RotateBody();
        }

    }

    [System.Serializable]
    public class CrouchToStandingState : StateBase<string>
    {
        public static readonly string mStateKey = "CrouchToStanding";

        public override string Key => mStateKey;


        private Animator mAnimator;

        private CapsuleColliderController mColliderController;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsCrouchToStandingToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mAnimator = actor.GetComponentInChildren<Animator>();
            mColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("crouchState", 2);
        }


        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("crouchState", -1);

            mColliderController.ResetCollider();
        }
    }
}
