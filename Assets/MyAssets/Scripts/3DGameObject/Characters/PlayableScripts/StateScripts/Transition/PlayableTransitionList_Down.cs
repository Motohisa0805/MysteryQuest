using UnityEngine;

namespace MyAssets
{
    public class IsWakeUpToIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsWakeUpToIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("standing Up") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
        public override bool IsTransition() => IsAnimationEnd();
    }
}