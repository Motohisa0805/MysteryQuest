using UnityEngine;

namespace MyAssets
{
    public class IsTakingOutTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        public IsTakingOutTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition() => mInput.Attack && mAnimator.GetFloat("modeBlend") <= 0;
    }

    public class IsStorageTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        public IsStorageTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }
        public override bool IsTransition() => mInput.Sprit && mAnimator.GetFloat("modeBlend") >= 1;
    }
}