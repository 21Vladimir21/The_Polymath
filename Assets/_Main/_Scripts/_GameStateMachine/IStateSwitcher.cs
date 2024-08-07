namespace _Main._Scripts._GameStateMachine
{
    public interface IStateSwitcher
    {
        void SwitchState<TState>() where TState : IState;
    }
}