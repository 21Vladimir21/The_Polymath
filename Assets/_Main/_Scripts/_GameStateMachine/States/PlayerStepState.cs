using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using _Main._Scripts.GameLogic.SwapTilesLogic;
using _Main._Scripts.LetterPooLogic;
using _Main._Scripts.Services;
using _Main._Scripts.UI;
using _Main._Scripts.UI.Views;
using UnityEngine;
using Random = UnityEngine.Random;

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

            _newLettersPanel.Initialize(lettersPool,fieldFacade);
            _wordValidationHandler = new WordValidationHandler(dictionary, fieldFacade, gameData);

            dragAndDrop.OnSwappedTiles += _newLettersPanel.ReturnTileToFreeCell;

            var locator = ServiceLocator.Instance.GetServiceByType<UILocator>();
            _validationWordsView = locator.GetViewByType<ValidationWordsView>();
            _inGameView = locator.GetViewByType<InGameView>();

            SwapTilesHandler swapTilesHandler = new(lettersPool, _newLettersPanel,_fieldFacade, _inGameView.SwapTilesPanelView);
            swapTilesHandler.OnSwapped += EndStep;

            _inGameView.CheckWordsButton.onClick.AddListener(ValidateNewWords);
            _inGameView.ReturnLettersToPanelButton.onClick.AddListener(ReturnLettersToPanel);
            _inGameView.EndStepButton.onClick.AddListener(EndStep);
            _inGameView.MixTilesButton.onClick.AddListener(newLettersPanel.MixTheTiles);
        }

        public void Enter()
        {
            _dragAndDrop.CanDrag = true;
            _newLettersPanel.SetNewLettersInPanel();
            _inGameView.ShowPlayerPanel();
            _inGameView.UpdatePoints(_gameData.PlayerPoints,_gameData.PCPoints);
            
        }

        public void Exit()
        {
            _newLettersPanel.ReturnAllTilesIntoCells(_fieldFacade.GetCellsFromNotRightTiles());
            _fieldFacade.ClearNotRightTiles();
            _dragAndDrop.CanDrag = false;
            _fieldFacade.UpdateFieldWords();
            _newLettersPanel.SetNewLettersInPanel();
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