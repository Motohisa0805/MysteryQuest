using UnityEngine;

namespace MyAssets
{
    public class IsJumpUpTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private PlayableChracterController mController;
        readonly private Animator mAnimator;
        public IsJumpUpTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputJump && mController.Grounded && !mAnimator.GetCurrentAnimatorStateInfo(0).IsName("falling To Landing");
        }
    }

    public class IsJumpLoopTransition : StateTransitionBase
    {
        readonly private PlayableChracterController mController;
        readonly private Animator mAnimator;
        public IsJumpLoopTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        private bool IsAnimationName()
        {
            return (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumping Up") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("runJump"));
        }

        public override bool IsTransition()
        {
            return IsAnimationName() && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                !mController.Grounded;
        }
    }

    public class IsJumpDownTransition : StateTransitionBase
    {
        readonly private PlayableChracterController mController;
        readonly private Animator mAnimator;
        public IsJumpDownTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("falling Idle") &&
                mController.Grounded;
        }
    }

    public class IsJumpDownTransitionType2 : StateTransitionBase
    {
        readonly private PlayableChracterController mController;
        readonly private Animator mAnimator;
        public IsJumpDownTransitionType2(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        private bool IsAnimationName()
        {
            return (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumping Up") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("runJump"));
        }
        public override bool IsTransition()
        {
            return IsAnimationName() &&
                mController.Grounded;
        }
    }

    public class IsJumpDownTransitionType3 : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        readonly private PlayableChracterController mController;
        readonly private Animator mAnimator;
        public IsJumpDownTransitionType3(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude > 0.0f && mAnimator.GetCurrentAnimatorStateInfo(0).IsName("falling To Landing") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                   mController.Grounded;
        }
    }

    public class IsLandingToFallTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        readonly private PlayableChracterController mController;
        public IsLandingToFallTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return !mInput.InputJump && !mController.Grounded && mController.FallTimer.IsEnd();
        }
    }

    public class IsFallingToLandTransition : StateTransitionBase
    {
        readonly private PlayableChracterController mController;
        public IsFallingToLandTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return mController.Grounded;
        }
    }
}