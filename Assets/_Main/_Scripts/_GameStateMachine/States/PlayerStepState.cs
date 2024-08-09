using System;
using System.Collections.Generic;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts._GameStateMachine.States
{
    public class PlayerStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly GameField _gameField;
        private readonly LettersPool _lettersPool;
        private readonly SortingDictionary _dictionary;
        private readonly DragAndDrop _dragAndDrop;

        public PlayerStepState(IStateSwitcher stateSwitcher,GameField gameField, LettersPool lettersPool,SortingDictionary dictionary,DragAndDrop dragAndDrop)
        {
            _stateSwitcher = stateSwitcher;
            _gameField = gameField;
            _lettersPool = lettersPool;
            _dictionary = dictionary;
            _dragAndDrop = dragAndDrop;
        
        }

        public void Enter()
        {
            _dragAndDrop.CanDrag = true;
            _gameField.CreateRandomWord();
            SetNewLettersInPanel();
        }

        public void Exit()
        {
            _dragAndDrop.CanDrag = false;
            SetNewLettersInPanel();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _gameField.CheckingGridForCorrectnessWords();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _stateSwitcher.SwitchState<OpponentStep>();
            }
            
        }

        private List<Letters> CreateRandomLettersList(int count)
        {
            List<Letters> randomLettersArray = new();
            var letters = Enum.GetValues(typeof(Letters));
            for (int i = 0; i <= count; i++)
            {
                var randomIndex = Random.Range(0, letters.Length);
                var randomLetter = (Letters)letters.GetValue(randomIndex);
                randomLettersArray.Add(randomLetter);
            }

            return randomLettersArray;
        }

        private void SetNewLettersInPanel()
        {
            var freeCells = _gameField.NewLettersPanel.GetFreeCells();
            var randomLetters = CreateRandomLettersList(freeCells.Count);
            for (int i = 0; i < freeCells.Count; i++)
            {
                var tile = _lettersPool.GetTile(randomLetters[i]);
                freeCells[i].AddTile(tile);
            }
        }
    }
}