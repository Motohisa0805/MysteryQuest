using UnityEngine;

namespace MyAssets
{
    public class IsThrowStartTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        readonly private PropsObjectChecker mChecker;
        public IsThrowStartTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }
        public override bool IsTransition()
        {
            return mInput.InputThrow && mChecker.TakedObject != null;
        }
    }

    //ìäÇ∞ÇÈìÆçÏä÷òAÇÃëJà⁄ÉNÉâÉX
    public class IsThrowIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsThrowIdleTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("throw Start") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsThrowingTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        public IsThrowingTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponentInChildren<PlayableInput>();
        }
        public override bool IsTransition()
        {
            return !mInput.InputThrow;
        }
    }

    public class IsThrowingToIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsThrowingToIdleTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("throwing") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }
}