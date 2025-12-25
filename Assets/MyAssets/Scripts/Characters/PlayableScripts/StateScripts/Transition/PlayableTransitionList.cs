using UnityEngine;

namespace MyAssets
{
    public class IsIdleTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f;
    }

    public class IsIdleTransitionType2 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsIdleTransitionType2(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f;
    }

    public class IsIdleTransitionType3 : StateTransitionBase
    {
        readonly private Animator mAnimator;
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

    public class IsIdleTransitionType4 : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsIdleTransitionType4(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("push End") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsIdleTransitionType5 : StateTransitionBase
    {
        readonly private Movement mMovement;
        readonly private PlayableChracterController mController;
        public IsIdleTransitionType5(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mMovement = actor.GetComponentInChildren<Movement>();
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return mMovement.ClimbJumpingTimer.IsEnd();
        }
    }

    public class IsIdleTransitionType6 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private TargetSearch mTargetSearch;
        public IsIdleTransitionType6(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f && !mInput.Focusing;
    }

    public class IsMoveTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsMoveTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.InputMove.magnitude > 0.1f;
    }
    public class IsMoveTransitionType2 : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        readonly private PlayableChracterController mController;
        readonly private Animator mAnimator;
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

    public class IsMoveTransitionType3 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private TargetSearch mTargetSearch;
        public IsMoveTransitionType3(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mTargetSearch = actor.GetComponent<TargetSearch>();
        }
        public override bool IsTransition() => mInput.InputMove.magnitude > 0.1f && !mInput.Focusing;
    }

    public class IsSpritDushTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsSpritDushTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.Sprit;
    }

    public class IsNotSpritDushTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsNotSpritDushTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => !mInput.Sprit;
    }

    public class IsFocusingMoveTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsFocusingMoveTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();

        }
        public override bool IsTransition() => mInput.Focusing;
    }

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
            return IsAnimationName() && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f&&
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
            return mInput.InputMove.magnitude > 0.0f &&mAnimator.GetCurrentAnimatorStateInfo(0).IsName("falling To Landing") &&
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

    /*
     * しゃがみ関係の遷移フラグ
     */

    public class IsStandingToCrouchTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        public IsStandingToCrouchTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition()
        {
            return mInput.InputCrouch;
        }
    }

    public class IsCrouchIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsCrouchIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
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
        readonly private PlayableInput mInput;
        public IsCrouch_IdleToWalkTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude > 0.1f;
        }
    }

    public class IsCrouch_WalkToIdleTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        public IsCrouch_WalkToIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition()
        {
            return mInput.InputMove.magnitude < 0.1f;
        }
    }

    public class IsCrouchToStandingTransition : StateTransitionBase
    {
        readonly private PlayableInput mInput;
        public IsCrouchToStandingTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition()
        {
            return mInput.InputCrouch;
        }
    }

    public class IsCrouchToStandingToIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsCrouchToStandingToIdleTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
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
            return (mChecker.LargeObject != null && !mChecker.PushEnabled)|| mInput.InputMove.magnitude <= 0.0f;
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

    public class IsClimbJumpingTransition : StateTransitionBase
    {
        readonly private Movement mMovement;
        public IsClimbJumpingTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mMovement = actor.GetComponent<Movement>();
        }
        public override bool IsTransition()
        {
            return mMovement.MovementCompensator.IsClimbJumping;
        }
    }

    public class IsClimbJumpTransition : StateTransitionBase
    {
        readonly private Movement mMovement;
        public IsClimbJumpTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mMovement = actor.GetComponent<Movement>();
        }
        public override bool IsTransition()
        {
            return mMovement.MovementCompensator.IsClimb;
        }
    }

    public class IsClimbTransition : StateTransitionBase
    {
        readonly private PlayableChracterController mController;
        public IsClimbTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return mController.Rigidbody.linearVelocity.y <= 0.0f;
        }
    }

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

    //投げる動作関連の遷移クラス
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

    public class IsSmallImpactTransition : StateTransitionBase
    {
        readonly private ImpactChecker mImpactChecker;
        public IsSmallImpactTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }
        public override bool IsTransition()
        {
            return mImpactChecker.IsEnabledSmallDamage;
        }
    }

    public class IsSmallImpactToIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsSmallImpactToIdleTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("small Impact") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

    public class IsSmallImpactToIdle2Transition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsSmallImpactToIdle2Transition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetInteger("impact State") <= -2;
        }
    }

    public class IsImpactTransition : StateTransitionBase
    {
        readonly private ImpactChecker mImpactChecker;
        public IsImpactTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }
        public override bool IsTransition()
        {
            return mImpactChecker.IsEnabledBigDamage;
        }
    }

    public class IsImpactToStandingUpTransition : StateTransitionBase
    {
        readonly private ImpactChecker mImpactChecker;
        public IsImpactToStandingUpTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<ImpactChecker>();
        }
        public override bool IsTransition()
        {
            return !mImpactChecker.IsEnabledDamage;
        }
    }

    public class IsStandingUpToIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsStandingUpToIdleTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("standing Up") &&
                   mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
    }

}