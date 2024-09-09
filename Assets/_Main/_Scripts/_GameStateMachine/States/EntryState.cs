using _Main._Scripts.BotLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using _Main._Scripts.Services;
using _Main._Scripts.UI;
using _Main._Scripts.UI.Views;


namespace _Main._Scripts._GameStateMachine.States
{
    public class EntryState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;
        private readonly ChooseComplexityView _complexityView;

        public EntryState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade, CurrentGameData currentGameData)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
            var uiLocator = ServiceLocator.Instance.GetServiceByType<UILocator>();
            _complexityView = uiLocator.GetViewByType<ChooseComplexityView>();
            
            _complexityView.StartGameButton.onClick.AddListener(StartGame);

            var complexityHandler = new ChooseComplexityHandler(_complexityView.ComplexityButtons, currentGameData);
            complexityHandler.SetDefaultComplexity();
        }

        public void Enter()
        {
            _complexityView.Open();
        }

        public void Exit()
        {
            _complexityView.Close();
        }

        public void Update()
        {
        }

        private void StartGame()
        {
            _fieldFacade.ClearField();
            _fieldFacade.CreateRandomStartWord();
            _stateSwitcher.SwitchState<PlayerStepState>();
        }
    }
}