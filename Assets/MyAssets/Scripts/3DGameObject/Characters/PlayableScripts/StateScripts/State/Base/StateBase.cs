using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ó‘Ô‚ÌŠî’êƒNƒ‰ƒX
    public abstract class StateBase<TKey>
    {

        public abstract string Key { get; }
        public override string ToString() => Key;
        public IStateChanger<TKey> StateChanger { protected get; set; }

        private List<IStateTransition<string>> mTransitionList = new List<IStateTransition<string>>();

        protected Action mOnComplete;

        public abstract List<IStateTransition<string>> CreateTransitionList(GameObject actor);

        public virtual List<ActionButtonInfo> GetActionButtons()
        {
            return new List<ActionButtonInfo>();
        }

        public virtual void Setup(GameObject actor)
        {
            mTransitionList = CreateTransitionList(actor);
        }

        public virtual void SetConfig(Action onComplete)
        {
            mOnComplete = onComplete;
        }

        public virtual void OnComplete()
        {
            mOnComplete?.Invoke();
        }

        public virtual void TransitionCheck()
        {
            foreach (var check in mTransitionList)
            {
                if (check.IsTransition())
                {
                    check.Transition();
                    break;
                }
            }
        }

        public virtual void Enter()
        {
            //ó‘Ô‚É“ü‚Á‚½‚Æ‚«‚Ìˆ—
        }

        public virtual void Execute_FixedUpdate(float time)
        {
            //ó‘Ô’†‚Ìˆ—
        }
        public virtual void Execute_Update(float time)
        {
            //ó‘Ô’†‚Ìˆ—
            TransitionCheck();
        }
        public virtual void Execute_LateUpdate(float time)
        {
            //ó‘Ô’†‚Ìˆ—
        }

        public virtual void Execute_IKAnimatorUpdate(float time)
        {
            //ó‘Ô’†‚Ìˆ—
        }

        public virtual void Exit()
        {
            //ó‘Ô‚©‚ço‚½‚Æ‚«‚Ìˆ—
        }

        public virtual void TriggerEnter(GameObject thisObject, Collider collider) { }

        public virtual void TriggerStay(GameObject thisObject, Collider collider) { }

        public virtual void TriggerExit(GameObject thisObject, Collider collider) { }

        public virtual void CollisionEnter(GameObject thisObject, Collision collision) { }

        public virtual void CollisionStay(GameObject thisObject, Collision collision) { }

        public virtual void CollisionExit(GameObject thisObject, Collision collision) { }
    }

    public abstract class StateTransitionBase : IStateTransition<string>
    {
        readonly IStateChanger<string> stateChanger;
        readonly string changeKey;
        protected StateTransitionBase(IStateChanger<string> stateChanger, string changeKey)
        {
            this.stateChanger = stateChanger;
            this.changeKey = changeKey;
        }
        public abstract bool IsTransition();
        public void Transition() => stateChanger.ChangeState(changeKey);
    }

    public interface IStateChanger<TKey>
    {
        bool IsContain(string key);
        bool ChangeState(string key);
    }
}
