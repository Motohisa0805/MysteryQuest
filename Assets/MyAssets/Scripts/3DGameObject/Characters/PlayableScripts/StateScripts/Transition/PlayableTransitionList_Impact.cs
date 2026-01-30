using UnityEngine;

namespace MyAssets
{

    public class IsMediumImpactTransition : StateTransitionBase
    {
        readonly private DamageChecker mImpactChecker;
        public IsMediumImpactTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }
        public override bool IsTransition()
        {
            return mImpactChecker.IsEnabledSmallDamage;
        }
    }

    public class IsMediumImpactToIdleTransition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsMediumImpactToIdleTransition
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

    public class IsMediumImpactToIdle2Transition : StateTransitionBase
    {
        readonly private Animator mAnimator;
        public IsMediumImpactToIdle2Transition
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
        readonly private DamageChecker mImpactChecker;
        public IsImpactTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }
        public override bool IsTransition()
        {
            return mImpactChecker.IsEnabledBigDamage;
        }
    }
    public class IsFallBigImpactTransition : StateTransitionBase
    {
        readonly private DamageChecker mImpactChecker;
        public IsFallBigImpactTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }
        public override bool IsTransition()
        {
            return mImpactChecker.IsEnabledFallDamage;
        }
    }

    public class IsImpactToStandingUpTransition : StateTransitionBase
    {
        readonly private DamageChecker mImpactChecker;
        public IsImpactToStandingUpTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<DamageChecker>();
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

    public class IsDeathStateTransition : StateTransitionBase
    {
        readonly private PlayableChracterController mController;
        public IsDeathStateTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mController = actor.GetComponent<PlayableChracterController>();
        }
        public override bool IsTransition()
        {
            return mController.StatusManager.PlayerStatusData.CurrentHP <= 0.0f;
        }
    }

}