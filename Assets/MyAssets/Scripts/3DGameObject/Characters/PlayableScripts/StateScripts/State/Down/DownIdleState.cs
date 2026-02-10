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

        private PlayableAnimationFunction mAnimationFunction;

        public override List<IStateTransition<string>> CreateTransitionList(GameObject actor)
        {
            List<IStateTransition<string>> re = new List<IStateTransition<string>>();
            if (StateChanger.IsContain(WakeUpState.mStateKey)) { re.Add(new IsMoveTransition(actor, StateChanger, WakeUpState.mStateKey)); }
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
            mAnimationFunction.Animator.SetInteger("downState", 0);
        }

        public override void Execute_Update(float time)
        {
            base.Execute_Update(time);
            mAnimationFunction.UpdateLayerWeight();
            mAnimationFunction.UpdateModeBlend();
            mAnimationFunction.UpdateIdleToRunAnimation();
        }
    }
}
