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

    //•ÏX—\’è
    public class IsIdleTransitionType7 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsIdleTransitionType7(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f;
    }

    public class IsIdleTransitionType8 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        public IsIdleTransitionType8(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }


        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("first Attack") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f && !mInput.Attack && IsAnimationEnd();
    }
    public class IsIdleTransitionType9 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        public IsIdleTransitionType9(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }


        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("second Attack") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f;
        }
        public override bool IsTransition() => mInput.InputMove.magnitude < 0.1f && !mInput.Attack && IsAnimationEnd();
    }
}
