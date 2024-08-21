using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;


namespace _Main._Scripts._GameStateMachine.States
{
    public class EntryState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;

        public EntryState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
        }

        public void Enter()
        {
            _fieldFacade.CreateRandomStartWord();
            _stateSwitcher.SwitchState<PlayerStepState>();
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}