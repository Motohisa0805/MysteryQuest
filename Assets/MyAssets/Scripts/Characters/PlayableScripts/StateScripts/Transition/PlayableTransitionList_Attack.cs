using UnityEngine;

namespace MyAssets
{
    public class IsReadyFirstAttackTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        readonly private EquipmentController mEquipmentController;
        public IsReadyFirstAttackTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
            mEquipmentController = actor.GetComponent<EquipmentController>();
        }
        public override bool IsTransition() => mInput.Attack && mEquipmentController.IsBattleMode;
    }

    public class IsFirstAttackTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsFirstAttackTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("ready First Attack") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
        public override bool IsTransition() => IsAnimationEnd();
    }

    public class IsFirstAttackTransition2 : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        readonly float mDuration = 0.5f;
        public IsFirstAttackTransition2(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("second Attack") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > mDuration;
        }
        public override bool IsTransition() => mInput.Attack && IsAnimationEnd();
    }

    public class IsSecondAttackTransition : StateTransitionBase
    {

        readonly private PlayableInput mInput;
        readonly private Animator mAnimator;
        readonly float mDuration = 0.5f;
        public IsSecondAttackTransition(GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mInput = actor.GetComponent<PlayableInput>();
            mAnimator = actor.GetComponentInChildren<Animator>();
        }

        private bool IsAnimationEnd()
        {
            return mAnimator.GetCurrentAnimatorStateInfo(0).IsName("first Attack") && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= mDuration;
        }
        public override bool IsTransition() => mInput.Attack && IsAnimationEnd();
    }
}