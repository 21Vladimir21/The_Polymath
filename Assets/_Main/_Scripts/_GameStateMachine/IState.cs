namespace _Main._Scripts._GameStateMachine
{
    public interface IState
    {
        void Enter();
        void Exit();

        void Update();
    }
}