using UnityEngine;

namespace MyAssets
{

    public class IsSmallImpactTransition : StateTransitionBase
    {
        readonly private DamageChecker mImpactChecker;
        public IsSmallImpactTransition
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
        readonly private DamageChecker mImpactChecker;
        public IsDeathStateTransition
            (GameObject actor, IStateChanger<string> stateChanger, string changeKey)
            : base(stateChanger, changeKey)
        {
            mImpactChecker = actor.GetComponent<DamageChecker>();
        }
        public override bool IsTransition()
        {
            return PlayerStatusManager.Instance.PlayerStatusData.CurrentHP <= 0.0f;
        }
    }

}