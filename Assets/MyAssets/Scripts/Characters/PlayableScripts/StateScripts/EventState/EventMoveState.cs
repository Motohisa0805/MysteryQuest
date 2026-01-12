using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class EventMoveState : StateBase<string>
    {
        public static readonly string mStateKey = "Event_Move";
        public override string Key => mStateKey;
        PlayableChracterController mController;

        PlayableAnimationFunction mAnimationFunction;

        DamageChecker mImpactChecker;

        private PlayableInput mPlayableInput;

        private EquipmentController mEquipmentController;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(EventIdleState.mStateKey)) { re.Add(new IsEventMoveToIdleTransition(actor, StateChanger, EventIdleState.mStateKey)); }
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
            if (mEquipmentController.IsBattleMode)
            {
                mAnimationFunction.SetModeBlend(1);
            }
            else
            {
                mAnimationFunction.SetModeBlend(0);
            }
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateIdleToRunAnimation();
            mAnimationFunction.SpritDushClear();
            PlayerStatusManager.Instance.RecoverySP(mPlayableInput.Sprit);
        }

        public override void Execute_FixedUpdate(float time)
        {
            Vector3 eventInput = Vector3.zero;
            if (EventManager.Instance)
            {
                eventInput = EventManager.Instance.EventMoveTargetPosition.transform.position - mController.transform.position;
            }
            mController.InputVelocity(eventInput.normalized * 0.5f);
            mController.Movement.Move(mController.StatusProperty.MaxSpeed, mController.StatusProperty.Acceleration);
            base.Execute_FixedUpdate(time);
            mController.FreeRotate();
        }

        public override void Exit()
        {
            base.Exit();
            TPSCamera.CameraType = TPSCamera.Type.Free;
            mPlayableInput.enabled = true;
            OnComplete();
        }

        public override void CollisionEnter(GameObject thisObject, Collision collision)
        {
            base.CollisionEnter(thisObject, collision);
            mImpactChecker.ApplyDamagePower(collision);
        }
    }

}
