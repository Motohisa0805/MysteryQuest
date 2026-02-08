using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    [Serializable]
    public class DownIdleState : StateBase<string>
    {
        public static readonly string mStateKey = "DownIdle";
        public override string Key => mStateKey;

        PlayableChracterController mController;

        PlayableAnimationFunction mAnimationFunction;

        private Movement mMovement;

        DamageChecker mImpactChecker;

        private EquipmentController mEquipmentController;

        private PlayableInput mPlayableInput;

        private TargetSearch mTargetSearch;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(WakeUpState.mStateKey)) { re.Add(new IsMoveTransition(actor, StateChanger, WakeUpState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
            mMovement = actor.GetComponent<Movement>();
            mImpactChecker = actor.GetComponent<DamageChecker>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
            mPlayableInput = actor.GetComponent<PlayableInput>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("downState", 0);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mAnimationFunction.UpdateLayerWeight();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateIdleToRunAnimation();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
