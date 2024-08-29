using _Main._Scripts.BotLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;


namespace _Main._Scripts._GameStateMachine.States
{
    public class EntryState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;
        private readonly CurrentGameData _currentGameData;

        public EntryState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade, CurrentGameData currentGameData)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
            _currentGameData = currentGameData;
        }

        public void Enter()
        {
            _fieldFacade.ClearField();
            _fieldFacade.CreateRandomStartWord();
            _currentGameData.SetComplexity(BotComplexity.Easy);
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