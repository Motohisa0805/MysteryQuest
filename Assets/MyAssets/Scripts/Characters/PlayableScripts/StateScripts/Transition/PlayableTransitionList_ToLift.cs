using UnityEngine;

namespace MyAssets
{
    public class IsIdleToToLiftTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        readonly private PropsObjectChecker mChecker;
        public IsIdleToToLiftTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }
        public override bool IsTransition()
        {
            return mChecker.TakedObject != null && mInput.Interact;
        }
    }

    public class IsToLiftToToLiftIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        readonly private PropsObjectChecker mChecker;
        public IsToLiftToToLiftIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("to Lift") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                   mChecker.HasTakedObject;
        }
    }

    public class IsFiledToLiftToIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        readonly private PropsObjectChecker mChecker;
        public IsFiledToLiftToIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("to Lift") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                   !mChecker.HasTakedObject;
        }
    }

    public class IsToLiftIdleToToLiftRunTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        public IsToLiftIdleToToLiftRunTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude > 0.1f;
        }
    }

    public class IsToLiftRunToToLiftIdleTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        public IsToLiftRunToToLiftIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude < 0.1f;
        }
    }

    //TODO : 持ち上げを解除クラスは後々変更
    public class IsReleaseLiftTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        readonly private PropsObjectChecker mChecker;
        readonly private PlayableChracterController mController;
        public IsReleaseLiftTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<PropsObjectChecker>();
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return mChecker.TakedObject != null && mInput.Sprit || !mInput.InputJump && !mController.Grounded && mController.FallTimer.IsEnd(); ;
        }
    }
}