using UnityEngine;

namespace MyAssets
{
    public class IsIdleTransition : StateTransitionBase
    {

        readonly PlayableInput mInput;
        public IsIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f;
    }

    public class IsIdleTransitionType2 : StateTransitionBase
    {
        //readonly IMoveInputProvider input;

        readonly PlayableInput mInput;
        public IsIdleTransitionType2(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f;
    }

    public class IsIdleTransitionType3 : StateTransitionBase
    {
        //readonly IMoveInputProvider input;
        readonly Animator mAnimator;
        public IsIdleTransitionType3(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("falling To Landing") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsMoveTransition : StateTransitionBase
    {

        readonly PlayableInput mInput;
        public IsMoveTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.InputMove.magnitude > 0.1f;
    }
    public class IsMoveTransitionType2 : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsMoveTransitionType2(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition() => mInput.InputMove.magnitude > 0.1f && mController.Grounded &&
                                               mAnimator.GetCurrentAnimatorStateInfo(0).IsName("falling To Landing");
    }

    public class IsSpritDushTransition : StateTransitionBase
    {

        readonly PlayableInput mInput;
        public IsSpritDushTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.Sprit;
    }

    public class IsNotSpritDushTransition : StateTransitionBase
    {

        readonly PlayableInput mInput;
        public IsNotSpritDushTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => !mInput.Sprit;
    }
    public class IsJumpUpTransition : StateTransitionBase
    {

        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
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
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
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
            return IsAnimationName() && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f&&
                !mController.Grounded;
        }
    }

    public class IsJumpDownTransition : StateTransitionBase
    {
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
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
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
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
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsJumpDownTransitionType3(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude > 0.0f &&mAnimator.GetCurrentAnimatorStateInfo(0).IsName("falling To Landing") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                   mController.Grounded;
        }
    }


    public class IsLandingToFallTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsLandingToFallTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return !mInput.InputJump && !mController.Grounded;
        }
    }

    public class IsFallingToLandTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsFallingToLandTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mController.Grounded;
        }
    }

    /*
     * しゃがみ関係の遷移フラグ
     */

    public class IsStandingToCrouchTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsStandingToCrouchTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputCrouch;
        }
    }

    public class IsCrouchIdleTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsCrouchIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("standing To Crouched") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsCrouch_IdleToWalkTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsCrouch_IdleToWalkTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude > 0.1f;
        }
    }

    public class IsCrouch_WalkToIdleTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsCrouch_WalkToIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude < 0.1f;
        }
    }

    public class IsCrouchToStandingTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsCrouchToStandingTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputCrouch;
        }
    }

    public class IsCrouchToStandingToIdleTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly PlayableChracterController mController;
        readonly Animator mAnimator;
        public IsCrouchToStandingToIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mController = actor.GetComponent<PlayableChracterController>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("crouched To Standing") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsIdleToToLiftTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly TakedObjectChecker mChecker;
        public IsIdleToToLiftTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<TakedObjectChecker>();
        }
        public override bool IsTransition()
        {
            return mChecker.TakedObject != null && mInput.Interact;
        }
    }

    public class IsToLiftToToLiftIdleTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly TakedObjectChecker mChecker;
        readonly Animator mAnimator;
        public IsToLiftToToLiftIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<TakedObjectChecker>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("to Lift") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsToLiftIdleToToLiftRunTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly TakedObjectChecker mChecker;
        readonly Animator mAnimator;
        public IsToLiftIdleToToLiftRunTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<TakedObjectChecker>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude > 0.1f;
        }
    }

    public class IsToLiftRunToToLiftIdleTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly TakedObjectChecker mChecker;
        readonly Animator mAnimator;
        public IsToLiftRunToToLiftIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<TakedObjectChecker>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude < 0.1f;
        }
    }
    //TODO : 持ち上げを解除クラスは後々変更
    public class IsReleaseLiftTransition : StateTransitionBase
    {
        readonly PlayableInput mInput;
        readonly TakedObjectChecker mChecker;
        readonly Animator mAnimator;
        public IsReleaseLiftTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mChecker = actor.GetComponent<TakedObjectChecker>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mChecker.TakedObject != null && mInput.Sprit;
        }
    }
}