using UnityEngine;

namespace MyAssets
{
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

    public class IsMoveTransitionType5 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        public IsMoveTransitionType5(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }


        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("first Attack") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f;
        }
        public override bool IsTransition() => mInput.InputMove.magnitude > 0.1f && !mInput.Attack && IsAnimationEnd();
    }

    public class IsMoveTransitionType6 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        readonly float mDuration = 0.5f;
        public IsMoveTransitionType6(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }


        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("second Attack") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f;
        }
        public override bool IsTransition() => mInput.InputMove.magnitude > 0.1f && !mInput.Attack && IsAnimationEnd();
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

    public class IsMoveTransitionType4 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        public IsMoveTransitionType4(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
        }
        public override bool IsTransition() => mInput.InputMove.magnitude > 0.1f;
    }
}