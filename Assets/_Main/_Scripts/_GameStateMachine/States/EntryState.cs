using _Main._Scripts.BotLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using _Main._Scripts.Services;
using _Main._Scripts.Services.FaderService;
using _Main._Scripts.Services.Saves;
using _Main._Scripts.UI;
using _Main._Scripts.UI.Views;
using Unity.VisualScripting;


namespace _Main._Scripts._GameStateMachine.States
{
    public class EntryState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;
        private readonly ChooseComplexityView _complexityView;
        private readonly CurrentGameData _currentGameData;
        private readonly NewLettersPanel _newLettersPanel;
        private readonly Saves _saves;
        private readonly FadeService _fadeService;

        public EntryState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade, CurrentGameData currentGameData,
            NewLettersPanel newLettersPanel)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
            _currentGameData = currentGameData;
            _newLettersPanel = newLettersPanel;
            var uiLocator = ServiceLocator.Instance.GetServiceByType<UILocator>();
            _complexityView = uiLocator.GetViewByType<ChooseComplexityView>();

            var savesService = ServiceLocator.Instance.GetServiceByType<SavesService>();
            _saves = savesService.Saves;

            _fadeService = ServiceLocator.Instance.GetServiceByType<FadeService>();

            _complexityView.StartGameButton.onClick.AddListener(StartNewGame);

            var complexityHandler = new ChooseComplexityHandler(_complexityView.ComplexityButtons, _currentGameData);
            complexityHandler.SetDefaultComplexity();
        }

        public void Enter()
        {
            if (_saves.HasStartGame)
            {
                _newLettersPanel.SetLettersFromSave();
                _fieldFacade.LoadGridFromSave();
                _fadeService.EnableFade(() => _stateSwitcher.SwitchState<PlayerStepState>());

            }
            else
            {
                _currentGameData.ClearData();
                _complexityView.Open();
            }
        }

        public void Exit()
        {
            _complexityView.Close();
        }

        public void Update()
        {
        }

        private void StartNewGame()
        {
            _fieldFacade.ClearField();
            _fieldFacade.CreateRandomStartWord();
            _newLettersPanel.SetNewLettersInPanel();
            _fadeService.EnableFade(() => _stateSwitcher.SwitchState<PlayerStepState>());
            _saves.HasStartGame = true;
        }
    }
}