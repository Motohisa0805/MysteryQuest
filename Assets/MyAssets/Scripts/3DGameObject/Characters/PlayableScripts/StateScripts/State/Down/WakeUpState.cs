using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ãNÇ´è„Ç™ÇÈèÛë‘
    [Serializable]
    public class WakeUpState : StateBase<string>
    {
        public static readonly string                   mStateKey = "WakeUp";
        public override string                          Key => mStateKey;

        private PlayableAnimationFunction               mAnimationFunction;
        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(IdleState.mStateKey)) { re.Add(new IsWakeUpToIdleTransition(actor, StateChanger, IdleState.mStateKey)); }
            return re;
        }

        public override void Setup(GameObject actor)
        {
            base.Setup(actor);
            mAnimationFunction = actor.GetComponent<PlayableAnimationFunction>();
        }

        public override void Enter()
        {
            base.Enter();
            mAnimationFunction.Animator.SetInteger("downState", 1);
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
            mAnimationFunction.Animator.SetInteger("downState", -1);
            mOnComplete?.Invoke();
        }
    }
}
