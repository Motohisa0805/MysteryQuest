namespace MyAssets
{
    public interface IStateTransition<TKey>
    {
        bool IsTransition();
        void Transition();
    }
}
