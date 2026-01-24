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

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private PlayableChracterController mController;

        private  DamageChecker mImpactChecker;

        [SerializeField]
        private float mCrouchHeight;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(CrouchIdleState.mStateKey)) { re.Add(new IsCrouchIdleTransition(actor, StateChanger, CrouchIdleState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mPlayableInput = actor.GetComponentInChildren<PlayableInput>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();

            mAnimationFunction.Animator.SetInteger("crouchState", 0);

            float standingHeight = mColliderController.CapsuleCollider.height;
            float crouchHeight = mCrouchHeight;
            float crouchCenter_Y = mColliderController.CapsuleCollider.center.y - (standingHeight - crouchHeight) / 2;
            mColliderController.SetHeight(crouchHeight);
            Vector3 c = mColliderController.CapsuleCollider.center;
            mColliderController.SetCenter(new Vector3(c.x, crouchCenter_Y, c.z));
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateCrouchAnimation();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.CrouchMaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("crouchState", -1);
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

    [System.Serializable]
    public class CrouchIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "CrouchIdle";

        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private DamageChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(CrouchToStandingState.mStateKey)) { re.Add(new IsCrouchToStandingTransition(actor, StateChanger, CrouchToStandingState.mStateKey)); }
            if (StateChanger.IsContain(CrouchWalkState.mStateKey)) { re.Add(new IsCrouch_IdleToWalkTransition(actor, StateChanger, CrouchWalkState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("crouchState", 1);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateCrouchAnimation();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.CrouchMaxSpeed, mController.StatusProperty.Acceleration);
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
    public class CrouchWalkState : StateBase<string>
    {
        public static readonly string mStateKey = "CrouchWalk";

        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private DamageChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(CrouchIdleState.mStateKey)) { re.Add(new IsCrouch_WalkToIdleTransition(actor, StateChanger, CrouchIdleState.mStateKey)); }
            if (StateChanger.IsContain(CrouchToStandingState.mStateKey)) { re.Add(new IsCrouchToStandingTransition(actor, StateChanger, CrouchToStandingState.mStateKey)); }
            if (StateChanger.IsContain(FallState.mStateKey)) { re.Add(new IsLandingToFallTransition(actor, StateChanger, FallState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mPlayableInput = actor.GetComponentInChildren<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateCrouchAnimation();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.CrouchMaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }
        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

    [System.Serializable]
    public class CrouchToStandingState : StateBase<string>
    {
        public static readonly string mStateKey = "CrouchToStanding";

        public override string Key => mStateKey;

        private PlayableChracterController mController;

        private PlayableInput mPlayableInput;

        private PlayableAnimationFunction mAnimationFunction;

        private CapsuleColliderController mColliderController;

        private DamageChecker mImpactChecker;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsCrouchToStandingToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            if (StateChanger.IsContain(MediumImpactPlayerState.mStateKey)) { re.Add(new IsMediumImpactTransition(actor, StateChanger, MediumImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(BigImpactPlayerState.mStateKey)) { re.Add(new IsImpactTransition(actor, StateChanger, BigImpactPlayerState.mStateKey)); }
            if (StateChanger.IsContain(DeathPlayerState.mStateKey)) { re.Add(new IsDeathStateTransition(actor, StateChanger, DeathPlayerState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mPlayableInput = actor.GetComponentInChildren<PlayableInput>();
            mColliderController = actor.GetComponentInChildren<CapsuleColliderController>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("crouchState", 2);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Left);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Up);
            PlayerUIManager.Instance.ActionButtonController.DisableButton((int)ActionButtonController.ActionButtonTag.Down);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            // Idle状態の特定の処理をここに追加できます
            // 例: アニメーションの更新など
            mAnimationFunction.UpdateCrouchAnimation();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.InputVelocity();
            mController.Movement.Move(mController.StatusProperty.CrouchMaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("crouchState", -1);

            mColliderController.ResetCollider();
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
