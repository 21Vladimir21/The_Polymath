using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using _Main._Scripts.GameLogic.SwapTilesLogic;
using _Main._Scripts.LetterPooLogic;
using _Main._Scripts.Services;
using _Main._Scripts.Services.FaderService;
using _Main._Scripts.Services.Saves;
using _Main._Scripts.UI;
using _Main._Scripts.UI.Views;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public class PlayerStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly NewLettersPanel _newLettersPanel;
        private readonly DragAndDrop _dragAndDrop;
        private readonly CurrentGameData _gameData;
        private readonly FieldFacade _fieldFacade;
        private readonly WordValidationHandler _wordValidationHandler;
        private readonly ValidationWordsView _validationWordsView;
        private readonly InGameView _inGameView;
        private readonly Saves _saves;
        private readonly FadeService _fadeService;

        public PlayerStepState(IStateSwitcher stateSwitcher, NewLettersPanel newLettersPanel,
            LettersPool lettersPool, SortingDictionary dictionary, DragAndDrop dragAndDrop,
            CurrentGameData gameData,
            FieldFacade fieldFacade)
        {
            _stateSwitcher = stateSwitcher;
            _newLettersPanel = newLettersPanel;
            _dragAndDrop = dragAndDrop;
            _gameData = gameData;
            _fieldFacade = fieldFacade;

            _newLettersPanel.Initialize(lettersPool);
            _wordValidationHandler = new WordValidationHandler(dictionary, fieldFacade, gameData);

            dragAndDrop.OnSwappedTiles += _newLettersPanel.ReturnTileToFreeCell;

            var locator = ServiceLocator.Instance.GetServiceByType<UILocator>();
            _validationWordsView = locator.GetViewByType<ValidationWordsView>();
            _inGameView = locator.GetViewByType<InGameView>();
            var savesService = ServiceLocator.Instance.GetServiceByType<SavesService>();
            _saves = savesService.Saves;

            _fadeService = ServiceLocator.Instance.GetServiceByType<FadeService>();

            SwapTilesHandler swapTilesHandler =
                new(lettersPool, _newLettersPanel, _fieldFacade, _inGameView.SwapTilesPanelView);
            swapTilesHandler.OnSwapped += EndStep;

            _dragAndDrop.OnFieldChanged += _fieldFacade.SaveGrid;
            _dragAndDrop.OnFieldChanged += _newLettersPanel.SaveLetters;

            _inGameView.CheckWordsButton.onClick.AddListener(ValidateNewWords);
            _inGameView.ReturnLettersToPanelButton.onClick.AddListener(ReturnLettersToPanel);
            _inGameView.EndStepButton.onClick.AddListener(EndStep);
            _inGameView.MixTilesButton.onClick.AddListener(newLettersPanel.MixTheTiles);
            _inGameView.MenuButton.onClick.AddListener(ToSelectComplexity);
        }

        public void Enter()
        {
            _dragAndDrop.CanDrag = true;
            _inGameView.ShowPlayerPanel();
            _inGameView.UpdatePoints(_gameData.PlayerPoints, _gameData.PCPoints);
            _inGameView.SetInteractableButtons(true);
        }

        public void Exit()
        {
            _inGameView.SetInteractableButtons(false);
            _newLettersPanel.ReturnAllTilesIntoCells(_fieldFacade.GetCellsFromNotRightTiles());
            _fieldFacade.ClearNotRightTiles();
            _fieldFacade.UpdateFieldWords();
            _newLettersPanel.SetNewLettersInPanel();
            _dragAndDrop.CanDrag = false;
        }


        public void Update()
        {
#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.R))
            {
                _fieldFacade.CreateRandomStartWord();
            }
#endif
        }

        private void EndStep()
        {
            int points = 0;
            _wordValidationHandler.CheckingGridForCorrectnessWords(ref points, true, out var _);
            Debug.Log($"Очки за ход: {points}");
            _gameData.PlayerPoints += points;

            if (_gameData.HasBeenRequiredPoints)
                _stateSwitcher.SwitchState<ResultState>();
            else
                _stateSwitcher.SwitchState<BotStepState>();
        }

        private void ToSelectComplexity()
        {
            _saves.HasStartGame = false;
            _fadeService.EnableFade(() => _stateSwitcher.SwitchState<EntryState>());
        }

        private void ReturnLettersToPanel()
        {
            _newLettersPanel.ReturnAllTilesIntoCells(_fieldFacade.GetCellsFromMovableTiles());
            _fieldFacade.ClearMovableTiles();
        }

        private void ValidateNewWords()
        {
            int points = 0;
            _wordValidationHandler.CheckingGridForCorrectnessWords(ref points, false, out var words);
            _validationWordsView.SetResultText(words, points, () => _validationWordsView.Open());
        }
    }
}