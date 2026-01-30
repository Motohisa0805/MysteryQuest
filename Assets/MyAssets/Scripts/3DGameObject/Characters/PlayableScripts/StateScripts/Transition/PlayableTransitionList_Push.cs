using UnityEngine;

namespace MyAssets
{
    public class IsPushStartTransition : StateTransitionBase
    {
        readonly private PropsObjectChecker mChecker;
        public IsPushStartTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }
        public override bool IsTransition()
        {
            return mChecker.LargeObject != null && mChecker.PushEnabled;
        }
    }

    public class IsPushingTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsPushingTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("push Start") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsPushEndStartTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        readonly private PropsObjectChecker mChecker;
        public IsPushEndStartTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }
        public override bool IsTransition()
        {
            return (mChecker.LargeObject != null && !mChecker.PushEnabled) || mInput.InputMove.magnitude <= 0.0f;
        }
    }

    public class IsPushEndTransition : StateTransitionBase
    {
        readonly private PropsObjectChecker mChecker;
        public IsPushEndTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }
        public override bool IsTransition()
        {
            return mChecker.CheckPushReleaseCondition() || !mChecker.PushEnabled;
        }
    }
}