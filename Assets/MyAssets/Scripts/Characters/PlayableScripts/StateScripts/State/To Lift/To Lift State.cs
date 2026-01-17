using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class ToLiftState : StateBase<string>
    {
        public static readonly string mStateKey = "To Lift";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PropsObjectChecker mChecker;

        private DamageChecker mImpactChecker;//当たり判定用チェッカー

        private PlayableAnimationFunction mAnimationFunction;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ToLiftIdleState.mStateKey)) { re.Add(new IsToLiftToToLiftIdleTransition(actor, StateChanger, ToLiftIdleState.mStateKey)); }
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsFiledToLiftToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mChecker = actor.GetComponent<PropsObjectChecker>();

            mPlayableInput = actor.GetComponent<PlayableInput>();

            mImpactChecker = actor.GetComponent<DamageChecker>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("to Lift", 0);
            // ターゲットへの方向ベクトルを計算する
            Vector3 dir = mChecker.TakedObject.transform.position - mController.gameObject.transform.position;
            dir.y = 0; // キャラクターのY軸回転のみを制御したい場合
            // 1. `Quaternion.LookRotation` を使って、その方向を向くための新しい回転を生成する
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            // 2. 生成した回転を transform.rotation に代入して適用する
            mController.gameObject.transform.rotation = targetRotation;
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.CheckTheDistanceHandsAndObject();
            mChecker.UpdateTakedObjectPosition();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, 0);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

    [System.Serializable]
    public class ToLiftIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "To Lift Idle";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PropsObjectChecker mChecker;

        private DamageChecker mImpactChecker;//当たり判定用チェッカー

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ToLiftRunState.mStateKey)) { re.Add(new IsToLiftIdleToToLiftRunTransition(actor, StateChanger, ToLiftRunState.mStateKey)); }
            if (StateChanger.IsContain(FocusingMoveState.mStateKey)) { re.Add(new IsFocusingMoveTransition(actor, StateChanger, FocusingMoveState.mStateKey)); }
            if (StateChanger.IsContain(ReleaseLiftState.mStateKey)) { re.Add(new IsReleaseLiftTransition(actor, StateChanger, ReleaseLiftState.mStateKey)); }
            if (StateChanger.IsContain(ThrowStartState.mStateKey)) { re.Add(new IsThrowStartTransition(actor, StateChanger, ThrowStartState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mPlayableInput = actor.GetComponent<PlayableInput>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();

            mChecker = actor.GetComponent<PropsObjectChecker>();

            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("to Lift", 1);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 1);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 1);
            mAnimationFunction.SetModeBlend(0);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectPosition();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateIdleToRunAnimation();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
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
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

    [System.Serializable]
    public class ToLiftRunState : StateBase<string>
    {
        public static readonly string mStateKey = "To Lift Run";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PropsObjectChecker mChecker;

        private DamageChecker mImpactChecker;//当たり判定用チェッカー

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ToLiftIdleState.mStateKey)) { re.Add(new IsToLiftRunToToLiftIdleTransition(actor, StateChanger, ToLiftIdleState.mStateKey)); }
            if (StateChanger.IsContain(FocusingMoveState.mStateKey)) { re.Add(new IsFocusingMoveTransition(actor, StateChanger, FocusingMoveState.mStateKey)); }
            if (StateChanger.IsContain(ReleaseLiftState.mStateKey)) { re.Add(new IsReleaseLiftTransition(actor, StateChanger, ReleaseLiftState.mStateKey)); }
            if (StateChanger.IsContain(ThrowStartState.mStateKey)) { re.Add(new IsThrowStartTransition(actor, StateChanger, ThrowStartState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mPlayableInput = actor.GetComponent<PlayableInput>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();

            mChecker = actor.GetComponent<PropsObjectChecker>();

            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.SetModeBlend(0);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectPosition();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateIdleToRunAnimation();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
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
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

    [System.Serializable]
    public class ReleaseLiftState : StateBase<string>
    {
        public static readonly string mStateKey = "ReleaseLift";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PropsObjectChecker mChecker;

        private Animator mAnimator;

        private DamageChecker mImpactChecker;//当たり判定用チェッカー

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransition(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mPlayableInput = actor.GetComponent<PlayableInput>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();

            mChecker = actor.GetComponent<PropsObjectChecker>();

            mAnimator = actor.GetComponentInChildren<Animator>();

            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mChecker.SetReleaseTakedObject();
            mAnimationFunction.StartUpdateAnimatorLayerWeight(1, 0);
            mAnimationFunction.StartUpdateAnimatorLayerWeight(2, 0);
            mAnimator.SetInteger("to Lift", -1);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateIdleToRunAnimation();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
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
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
