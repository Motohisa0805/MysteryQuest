using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MyAssets
{
    //イベント移動ポイントクラス
    //注意：イベント移動ポイントはEventManagerでIDの小さい順にソートされる
    //そのため、同じIDのポイントが複数存在すると予期せぬ動作をする可能性がある
    //必ずIDはユニークな値を設定すること
    public class EventPoint : MonoBehaviour
    {
        //数値ID(数値が小さい程優先度順位が高い)
        [SerializeField]
        protected int mPointNo;
        public int PointNo => mPointNo;

        protected Action mOnComplete;

        virtual public async Task SetConfig() 
        {
            var utcs = new UniTaskCompletionSource();
            mOnComplete = () =>
            {
                utcs.TrySetResult();
            };
            await utcs.Task;
        }
    }
}
