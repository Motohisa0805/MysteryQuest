using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class EventIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "Event_Idle";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        PlayableAnimationFunction mAnimationFunction;

        DamageChecker mImpactChecker;

        private PlayableInput mPlayableInput;

        private EquipmentController mEquipmentController;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            mPlayableInput.enabled = false;
            TPSCamera.CameraType = TPSCamera.Type.Fixed;
            mAnimationFunction.Animator.SetFloat("idleToRun", 0.0f);
            PlayerUIManager.Instance.ActionButtonController.AllDisableButton();
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mAnimationFunction.Animator.SetFloat("idleToRun", 0.0f);
        }

        public override void Execute_FixedUpdate(float time)
        {
            Vector3 eventInput = Vector3.zero;
            mController.InputVelocity(eventInput);
            mController.Movement.Move(mController.StatusManager.PlayerStatusData.MaxSpeed, mController.StatusManager.PlayerStatusData.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            TPSCamera.CameraType = TPSCamera.Type.Free;
            mPlayableInput.enabled = true;
            mOnComplete?.Invoke();
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }
}
