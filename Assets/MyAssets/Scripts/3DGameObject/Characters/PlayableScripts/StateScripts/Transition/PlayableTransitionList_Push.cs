using UnityEngine;

namespace MyAssets
{
    //押し始め遷移フラグ
    public class IsPushStartTransition : StateTransitionBase
    {
        readonly private PropsObjectChecker         mChecker;
        readonly private PlayableChracterController mController;
        public IsPushStartTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return !mController.StatusManager.IsStaminaCoolDown && mChecker.LargeObject != null && mChecker.PushEnabled;
        }
    }
    //押し中遷移フラグ
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
    //押し終了開始遷移フラグ
    public class IsPushEndStartTransition : StateTransitionBase
    {
        readonly private PlayableInput              mInput;
        readonly private PropsObjectChecker         mChecker;
        readonly private PlayableChracterController mController;
        public IsPushEndStartTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return (mChecker.LargeObject != null && !mChecker.PushEnabled) || mInput.InputMove.magnitude <= 0.0f || mController.StatusManager.IsStaminaCoolDown;
        }
    }
    //押し終了遷移フラグ
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