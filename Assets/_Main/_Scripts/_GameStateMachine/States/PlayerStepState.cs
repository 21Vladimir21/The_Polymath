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
        private SwapTilesHandler _swapTilesHandler;

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
            var inGameView = locator.GetViewByType<InGameView>();

            _swapTilesHandler = new(lettersPool, _newLettersPanel, inGameView.SwapTilesPanelView);
            _swapTilesHandler.OnSwapped += EndStep;

            inGameView.CheckWordsButton.onClick.AddListener(ValidateNewWords);
            inGameView.ReturnLettersToPanelButton.onClick.AddListener(ReturnLettersToPanel);
            inGameView.EndStepButton.onClick.AddListener(EndStep);
            inGameView.MixTilesButton.onClick.AddListener(newLettersPanel.MixTheTiles);
        }

        public void Enter()
        {
            _dragAndDrop.CanDrag = true;
            _newLettersPanel.SetNewLettersInPanel();
        }

        public void Exit()
        {
            int points = 0;
            _wordValidationHandler.CheckingGridForCorrectnessWords(ref points, true);
            Debug.Log($"Очки за ход: {points}");

            _newLettersPanel.ReturnNotRightTilesToPanel();
            _fieldFacade.ClearNotRightTiles();
            _dragAndDrop.CanDrag = false;
            _fieldFacade.UpdateFieldWords();
            _newLettersPanel.SetNewLettersInPanel();

            _gameData.PlayerPoints += points;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _fieldFacade.CreateRandomStartWord();
            }
        }

        private void EndStep()
        {
            if (_gameData.HasBeenRequiredPoints)
                _stateSwitcher.SwitchState<ResultState>();
            else
                _stateSwitcher.SwitchState<BotStepState>();
        }

        private void ReturnLettersToPanel()
        {
            _newLettersPanel.ReturnAllTilesIntoCells();
            _fieldFacade.ClearMovableTiles();
        }

        private void ValidateNewWords()
        {
            int points = 0;
            _wordValidationHandler.CheckingGridForCorrectnessWords(ref points, false);
            Debug.Log($"Очки: {points}");
        }
    }
}