using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class PushStartState : StateBase<string>
    {
        public static readonly string mStateKey = "PushStart";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private PropsObjectChecker mPropsChecker;

        private Animator mAnimator;//アニメーター

        [SerializeField]
        private float mPushPower;

        [SerializeField]
        private float mBasePushSpeed;

        [SerializeField]
        private float mMinSpeed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(PushingState.mStateKey)) { re.Add(new IsPushingTransition(actor, StateChanger, PushingState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsPushEndTransition(actor, StateChanger, MoveState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPropsChecker = actor.GetComponent<PropsObjectChecker>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            // ターゲットへの方向ベクトルを計算する
            Vector3 dir = mPropsChecker.LargeObject.transform.position - mController.gameObject.transform.position;
            dir.y = 0; // キャラクターのY軸回転のみを制御したい場合
            // 1. `Quaternion.LookRotation` を使って、その方向を向くための新しい回転を生成する
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            // 2. 生成した回転を transform.rotation に代入して適用する
            mController.gameObject.transform.rotation = targetRotation;

            mAnimator.SetInteger("pushState", 0);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            float objectMass = mPropsChecker.LargeObjectMass;

            float dampingFactor = mPushPower / objectMass;

            float finalSpeed = mBasePushSpeed * dampingFactor;

            // 速度に上限と下限を設定
            finalSpeed = Mathf.Clamp(finalSpeed, mMinSpeed, mBasePushSpeed);

            mController.Movement.PushObjectMove(finalSpeed);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("pushState", -1);
        }
    }


    [Serializable]
    public class PushingState : StateBase<string>
    {
        public static readonly string mStateKey = "Pushing";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private PropsObjectChecker mPropsChecker;

        private Animator mAnimator;//アニメーター

        [SerializeField]
        private float mPushPower;

        [SerializeField]
        private float mBasePushSpeed;

        [SerializeField]
        private float mMinSpeed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsPushEndTransition(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(PushEndState.mStateKey)) { re.Add(new IsPushEndStartTransition(actor, StateChanger, PushEndState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mPropsChecker = actor.GetComponent<PropsObjectChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("pushState", 1);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            //mPropsChecker.UpdatePushObjectCheck();
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            float objectMass = mPropsChecker.LargeObjectMass;

            float dampingFactor = mPushPower / objectMass;

            float finalSpeed = mBasePushSpeed * dampingFactor;

            // 速度に上限と下限を設定
            finalSpeed = Mathf.Clamp(finalSpeed, mMinSpeed, mBasePushSpeed);

            mController.Movement.PushObjectMove(finalSpeed);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("pushState", -1);
        }
    }

    [Serializable]
    public class PushEndState : StateBase<string>
    {
        public static readonly string mStateKey = "PushEnd";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PropsObjectChecker mPropsChecker;

        private Animator mAnimator;//アニメーター

        [SerializeField]
        private float mPushPower;

        [SerializeField]
        private float mBasePushSpeed;

        [SerializeField]
        private float mMinSpeed;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType4(actor, StateChanger, IdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mPropsChecker = actor.GetComponent<PropsObjectChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("pushState", 2);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            //mPropsChecker.UpdatePushObjectCheck();
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            float objectMass = mPropsChecker.LargeObjectMass;

            float dampingFactor = mPushPower / objectMass;

            float finalSpeed = mBasePushSpeed * dampingFactor;

            // 速度に上限と下限を設定
            finalSpeed = Mathf.Clamp(finalSpeed, mMinSpeed, mBasePushSpeed);

            mController.Movement.PushObjectMove(finalSpeed);
            base.Execute_FixedUpdate(time);
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("pushState", -1);
        }
    }
}
