using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using _Main._Scripts.LetterPooLogic;
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
        }

        public void Enter()
        {
            _dragAndDrop.CanDrag = true;
            _newLettersPanel.SetNewLettersInPanel();
        }

        public void Exit()
        {
            _newLettersPanel.ReturnNotRightTilesToPanel();
            _fieldFacade.ClearNotRightTiles();
            _dragAndDrop.CanDrag = false;
            _fieldFacade.UpdateFieldWords();
            _newLettersPanel.SetNewLettersInPanel();

            int points = 0;
            _wordValidationHandler.CheckingGridForCorrectnessWords(ref points);
            Debug.Log($"Очки за ход: {points}");
            _gameData.PlayerPoints += points;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                int points = 0;
                _wordValidationHandler.CheckingGridForCorrectnessWords(ref points);
                Debug.Log($"Очки: {points}");
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_gameData.HasBeenRequiredPoints)
                    _stateSwitcher.SwitchState<ResultState>();
                else
                    _stateSwitcher.SwitchState<BotStepState>();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _newLettersPanel.ReturnAllTilesIntoCells();
                _fieldFacade.ClearMovableTiles();
            }
        }
    }
}