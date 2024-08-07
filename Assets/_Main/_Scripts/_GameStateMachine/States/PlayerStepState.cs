using System;
using System.Collections.Generic;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts._GameStateMachine.States
{
    public class PlayerStepState : IState
    {
        private readonly GameField _gameField;
        private readonly LettersPool _lettersPool;

        public PlayerStepState(GameField gameField, LettersPool lettersPool)
        {
            _gameField = gameField;
            _lettersPool = lettersPool;
            _gameField.Init();
        }

        public void Enter()
        {
            _gameField.CreateRandomWord();
            SetNewLettersInPanel();
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _gameField.CheckingGridForCorrectnessWords();
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