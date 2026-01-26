using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class JumpUpState : StateBase<string>
    {
        public static readonly string mStateKey = "Jump";
        public override string Key => mStateKey;
        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;
        private TargetSearch mTargetSearch;

        private Animator mAnimator;//アニメーター
        private DamageChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(SecondJumpUpState.mStateKey)) { re.Add(new IsSecondJumpUpTransition(actor, StateChanger, SecondJumpUpState.mStateKey)); }
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransitionType2(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(JumpingState.mStateKey)) { re.Add(new IsJumpLoopTransition(actor, StateChanger, JumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 0);
            Vector3 vel = mController.Rigidbody.linearVelocity;
            vel.y = 0;
            if (vel.magnitude > 0.0f)
            {
                mController.Movement.Jump(mController.StatusManager.PlayerStatusData.MoveJumpPower);
            }
            else
            {
                mController.Movement.Jump(mController.StatusManager.PlayerStatusData.IdleJumpPower);
            }
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayOneShot3D("JumpUp", mController.transform.position);
            }
            PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Up, "ジャンプ");
        }

        public override void Execute_Update(float time)
        {
            mController.Movement.ClimbCheck();
            base.Execute_Update(time);
            mController.StatusManager.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            //mController.Movement.Gravity();
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusManager.PlayerStatusData.MaxSpeed, 5);
            base.Execute_FixedUpdate(time);
            mController.BodyRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("jumpState", -1);
        }
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }

    }

    [System.Serializable]
    public class SecondJumpUpState : StateBase<string>
    {
        public static readonly string mStateKey = "SecondJump";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private Animator mAnimator;//アニメーター
        private DamageChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransitionType2(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(JumpingState.mStateKey)) { re.Add(new IsJumpLoopTransition(actor, StateChanger, JumpingState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 1);
            Vector3 vel = mController.Rigidbody.linearVelocity;
            mController.Movement.Jump(mController.StatusManager.PlayerStatusData.IdleJumpPower);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayOneShot3D("JumpUp", mController.transform.position);
            }
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            mController.Movement.ClimbCheck();
            base.Execute_Update(time);
            mController.StatusManager.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusManager.PlayerStatusData.MaxSpeed, mController.StatusManager.PlayerStatusData.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.BodyRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("jumpState", -1);
        }
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }

    }

    [System.Serializable]
    public class JumpingState : StateBase<string>
    {
        public static readonly string mStateKey = "Jumping";
        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableAnimationFunction mPlayableAnimationFunction;

        private DamageChecker mImpactChecker;

        private TargetSearch mTargetSearch;

        private PlayableInput mPlayableInput;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            //if (StateChanger.IsContain(SecondJumpUpState.mStateKey)) { re.Add(new IsSecondJumpUpTransition(actor, StateChanger, SecondJumpUpState.mStateKey)); }
            if (StateChanger.IsContain(JumpDownState.mStateKey)) { re.Add(new IsJumpDownTransition(actor, StateChanger, JumpDownState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mPlayableAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
        }

        public override void Enter()
        {
            base.Enter();
            mPlayableAnimationFunction.Animator.SetInteger("jumpState", 2);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            mController.Movement.ClimbCheck();
            base.Execute_Update(time);
            mController.StatusManager.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusManager.PlayerStatusData.MaxSpeed, mController.StatusManager.PlayerStatusData.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.BodyRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mPlayableAnimationFunction.Animator.SetInteger("jumpState", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

    [System.Serializable]
    public class JumpDownState : StateBase<string>
    {
        public static readonly string mStateKey = "JumpDown";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        private Animator mAnimator;//アニメーター

        private DamageChecker mImpactChecker;

        private TargetSearch mTargetSearch;

        private PlayableInput mPlayableInput;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsIdleTransitionType3(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MoveState.mStateKey)) { re.Add(new IsMoveTransitionType2(actor, StateChanger, MoveState.mStateKey)); }
            if (StateChanger.IsContain(ClimbJumpingState.mStateKey)) { re.Add(new IsClimbJumpingTransition(actor, StateChanger, ClimbJumpingState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimator.SetInteger("jumpState", 3);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            mController.Movement.ClimbCheck();
            base.Execute_Update(time);
            mController.StatusManager.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusManager.PlayerStatusData.MaxSpeed, mController.StatusManager.PlayerStatusData.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.BodyRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimator.SetInteger("jumpState", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
