using UnityEngine;

namespace MyAssets
{
    /*
     * ‚µ‚á‚ª‚ÝŠÖŒW‚Ì‘JˆÚƒtƒ‰ƒO
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
}