using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class ToLiftState : StateBase<string>
    {
        public static readonly string mStateKey = "To Lift";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        PropsObjectChecker mChecker;

        private Animator mAnimator;//アニメーター

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ToLiftIdleState.mStateKey)) { re.Add(new IsToLiftToToLiftIdleTransition(actor, StateChanger, ToLiftIdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();

            mChecker = actor.GetComponent<PropsObjectChecker>();

            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("to Lift", 0);
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
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.Movement.Move(mController.MaxSpeed, 0);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    [System.Serializable]
    public class ToLiftIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "To Lift Idle";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        PropsObjectChecker mChecker;

        Animator mAnimator;

        [SerializeField]
        private float mAcceleration; //加速度

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ToLiftRunState.mStateKey)) { re.Add(new IsToLiftIdleToToLiftRunTransition(actor, StateChanger, ToLiftRunState.mStateKey)); }
            if (StateChanger.IsContain(ReleaseLiftState.mStateKey)) { re.Add(new IsReleaseLiftTransition(actor, StateChanger, ReleaseLiftState.mStateKey)); }
            if (StateChanger.IsContain(ThrowStartState.mStateKey)) { re.Add(new IsThrowStartTransition(actor, StateChanger, ThrowStartState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();

            mChecker = actor.GetComponent<PropsObjectChecker>();

            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("to Lift", 1);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectPosition();
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateToLiftIdleToToLiftRunAnimation();
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            mController.Movement.Move(mController.MaxSpeed, mAcceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    [System.Serializable]
    public class ToLiftRunState : StateBase<string>
    {
        public static readonly string mStateKey = "To Lift Run";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        PropsObjectChecker mChecker;

        [SerializeField]
        private float mAcceleration; //加速度

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(ToLiftIdleState.mStateKey)) { re.Add(new IsToLiftRunToToLiftIdleTransition(actor, StateChanger, ToLiftIdleState.mStateKey)); }
            if (StateChanger.IsContain(ReleaseLiftState.mStateKey)) { re.Add(new IsReleaseLiftTransition(actor, StateChanger, ReleaseLiftState.mStateKey)); }
            if (StateChanger.IsContain(ThrowStartState.mStateKey)) { re.Add(new IsThrowStartTransition(actor, StateChanger, ThrowStartState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();

            mChecker = actor.GetComponent<PropsObjectChecker>();
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mChecker.UpdateTakedObjectPosition();
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateToLiftIdleToToLiftRunAnimation();
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.MaxSpeed, mAcceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    [System.Serializable]
    public class ReleaseLiftState : StateBase<string>
    {
        public static readonly string mStateKey = "ReleaseLift";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private PlayableAnimationFunction mAnimationFunction;

        PropsObjectChecker mChecker;

        Animator mAnimator;

        [SerializeField]
        private float mAcceleration; //加速度

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransition(actor, StateChanger, MoveState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();

            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();

            mChecker = actor.GetComponent<PropsObjectChecker>();

            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            mChecker.SetReleaseTakedObject();
            mAnimator.SetInteger("to Lift", -1);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateIdleToRunAnimation();
        }

        public override void Execute_FixedUpdate(float time)
        {
            // Idle状態の特定の物理処理をここに追加できます
            // 例: 重力の適用、衝突判定など
            mController.InputVelocity();
            mController.Movement.Move(mController.MaxSpeed, mAcceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
