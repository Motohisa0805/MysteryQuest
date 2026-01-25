using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [System.Serializable]
    public class OutOfBodyExperienceState : StateBase<string>
    {
        public static readonly string       mStateKey = "OutOfBodyExperience";
        public override string              Key => mStateKey;
        private PlayableChracterController  mController;

        private PlayableAnimationFunction   mAnimationFunction;

        private DamageChecker               mImpactChecker;

        private CapsuleColliderController   mColliderController;

        [SerializeField]
        private float mCrouchHeight;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsOutOfBodyExperienceToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
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
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mColliderController = actor.GetComponent<CapsuleColliderController>();
        }

        public override void Enter()
        {
            base.Enter();
            mController.Movement.Stop();
            mAnimationFunction.Animator.SetInteger("outOfBodyState", 0);
            mController.SoulPlayerController.EnableSoul();

            float standingHeight = mColliderController.CapsuleCollider.height;
            float crouchHeight = mCrouchHeight;
            float crouchCenter_Y = mColliderController.CapsuleCollider.center.y - (standingHeight - crouchHeight) / 2;
            mColliderController.SetHeight(crouchHeight);
            Vector3 c = mColliderController.CapsuleCollider.center;
            mColliderController.SetCenter(new Vector3(c.x, crouchCenter_Y, c.z));

            mController.CharacterColorController.SetAllColors(Color.gray);

            PlayerUIManager.Instance.ActionButtonController.AllDisableButton();
            PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Right, "取る");
            PlayerUIManager.Instance.ActionButtonController.ActiveButton((int)ActionButtonController.ActionButtonTag.Down, "解除");

            TPSCamera.CameraType = TPSCamera.Type.ShoulderView;

            SoundManager.Instance.PlayOneShot2D("EnabledSkil_Playerl");
        }

        public override void Execute_Update(float time)
        {
            mController.SoulPlayerController.TakeObjectment.ObjectCheck();
            mController.SoulPlayerController.TakeObjectment.TakeObjectInput();
            mController.SoulPlayerController.TakeObjectment.InputTakeOffObject();
            base.Execute_Update(time);
        }

        public override void Execute_FixedUpdate(float time)
        {
            mController.SoulPlayerController.TakeObjectment.UpdateTakeObject();
            mController.SoulPlayerController.InputVelocity();
            mController.SoulPlayerController.FloatingMovement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.SoulPlayerController.SyncRotationWithCamera();
        }

        public override void Exit()
        {
            base.Exit();
            mAnimationFunction.Animator.SetInteger("outOfBodyState", -1);
            //幽体離脱モードの後処理
            mController.SoulPlayerController.DisableSoul();
            //コライダーの後処理
            mColliderController.ResetCollider();
            //カラーの後処理
            mController.CharacterColorController.ResetAllColors();
            //オブジェクト取得関連の後処理
            mController.SoulPlayerController.TakeObjectment.TakeOffObject();
            mController.SoulPlayerController.TakeObjectment.ClearFocus();
            //プレイヤーUI周りの後処理
            PlayerUIManager.Instance.ActionButtonController.AllDisableButton();

            TPSCamera.CameraType = TPSCamera.Type.Free;
            SoundManager.Instance.PlayOneShot2D("DisableSkil_Playerl");
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
