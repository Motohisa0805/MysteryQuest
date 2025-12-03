using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class MoveState : StateBase<string>
    {
        public static readonly string mStateKey = "Run";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        PropsObjectChecker mPropsChecker;

        PlayableAnimationFunction mAnimationFunction;

        [SerializeField]
        private float mAcceleration; //加速度
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(SpritDushState.mStateKey)) { re.Add(new IsSpritDushTransition(actor, StateChanger, SpritDushState.mStateKey)); }
            if (StateChanger.IsContain(StandingToCrouchState.mStateKey)) { re.Add(new IsStandingToCrouchTransition(actor, StateChanger, StandingToCrouchState.mStateKey)); }
            if (StateChanger.IsContain(PushStartState.mStateKey)) { re.Add(new IsPushStartTransition(actor, StateChanger, PushStartState.mStateKey)); }
            if (StateChanger.IsContain(JumpUpState.mStateKey)) { re.Add(new IsJumpUpTransition(actor, StateChanger, JumpUpState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpState.mStateKey)) { re.Add(new IsClimbJumpTransition(actor, StateChanger, ClimbJumpState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPropsChecker = actor.GetComponent<PropsObjectChecker>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            //mPropsChecker.UpdatePushObjectCheck();
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateIdleToRunAnimation();
            mAnimationFunction.SpritDushClear();
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
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
}
