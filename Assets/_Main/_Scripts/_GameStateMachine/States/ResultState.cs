using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.Services;
using _Main._Scripts.Services.Saves;
using _Main._Scripts.UI;
using _Main._Scripts.UI.Views;

namespace _Main._Scripts._GameStateMachine.States
{
    public class ResultState : IState
    {
        private readonly IStateSwitcher _switcher;
        private readonly CurrentGameData _gameData;
        private readonly NewLettersPanel _newLettersPanel;
        private ResultView _resultView;
        private readonly Saves _saves;

        public ResultState(IStateSwitcher switcher, CurrentGameData gameData, NewLettersPanel newLettersPanel)
        {
            _switcher = switcher;
            _gameData = gameData;
            _newLettersPanel = newLettersPanel;
            var savesService = ServiceLocator.Instance.GetServiceByType<SavesService>();
            _saves = savesService.Saves;

            var locator = ServiceLocator.Instance.GetServiceByType<UILocator>();
            _resultView = locator.GetViewByType<ResultView>();
            _resultView.MenuButton.onClick.AddListener(()=>_switcher.SwitchState<EntryState>());
        }

        public void Enter()
        {
            _saves.HasStartGame = false;
            _newLettersPanel.ReturnAllTilesToPool();
            _resultView.SetResult(_gameData.PlayerPoints,_gameData.BotPoints);
            _resultView.Open();
        }

        public void Exit()
        {
            _resultView.Close();
        }

        public void Update()
        {
        }
    }
}