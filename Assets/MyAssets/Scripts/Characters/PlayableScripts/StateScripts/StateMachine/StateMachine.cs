using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    //ステートを管理するクラス
    public class StateMachine<TKey> : IStateChanger<TKey>
    {
        //状態をDictionaryで管理
        private Dictionary<string, StateBase<TKey>> mStateDictionary;
        //現在の状態
        private StateBase<TKey> mCurrentState;
        public StateBase<TKey> CurrentState => mCurrentState;

        public event Action<StateBase<TKey>> OnStateChanged;

        public void Setup(StateBase<TKey>[] states, IEqualityComparer<TKey> comparer = null)
        {
            mStateDictionary = new Dictionary<string, StateBase<TKey>>();
            foreach (var state in states)
            {
                state.StateChanger = this;
                mStateDictionary.Add(state.Key, state);
            }
        }
        //Updateで使う関数
        public void Update(float time) => mCurrentState?.Execute_Update(time);
        //FixedUpdateで使う関数
        public void FixedUpdate(float time) => mCurrentState?.Execute_FixedUpdate(time);
        //LateUpdateで使う関数
        public void LateUpdate(float time) => mCurrentState?.Execute_LateUpdate(time);
        //Animationの更新が終わったLateUpdateで使う関数
        //public void DoAnimatorIKUpdate() => currentState?.DoAnimatorIKUpdate();
        //当たり判定の当たり初めに使う関数
        public void TriggerEnter(GameObject thisObject, Collider collider) => mCurrentState?.TriggerEnter(thisObject, collider);
        //当たり判定の当たり続けている時に使う関数
        public void TriggerStay(GameObject thisObject, Collider collider) => mCurrentState?.TriggerStay(thisObject, collider);
        //当たり判定の当たり終わり時に使う関数
        public void TriggerExit(GameObject thisObject, Collider collider) => mCurrentState?.TriggerExit(thisObject, collider);

        public void CollisionEnter(GameObject thisObject, Collision collision) => mCurrentState?.CollisionEnter(thisObject, collision);
        //当たり判定の当たり続けている時に使う関数
        public void CollisionStay(GameObject thisObject, Collision collision) => mCurrentState?.CollisionStay(thisObject, collision);
        //当たり判定の当たり終わり時に使う関数
        public void CollisionExit(GameObject thisObject, Collision collision) => mCurrentState?.CollisionExit(thisObject, collision);

        //状態を遷移するための条件を追加する関数
        public bool IsContain(string key) => mStateDictionary.ContainsKey(key);
        //TODO : Stateの変更
        //状態を変更する時の関数
        public bool ChangeState(string key)
        {
            if (!mStateDictionary.TryGetValue(key, out StateBase<TKey> state))
            {
                return false;
            }
            mCurrentState?.Exit();
            mCurrentState = state;
            mCurrentState.Enter();

            OnStateChanged?.Invoke(mCurrentState);
            return true;
        }
        //オブジェクトが削除される時に使う
        public void Dispose()
        {
            mStateDictionary = null;
            mCurrentState = null;
            OnStateChanged = null;
        }
    }
}
