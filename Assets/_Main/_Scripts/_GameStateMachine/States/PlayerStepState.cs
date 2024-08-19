using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts._GameStateMachine.States
{
    public class PlayerStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly PlayingField _playingField;
        private readonly NewLettersPanel _newLettersPanel;
        private readonly LettersPool _lettersPool;
        private readonly SortingDictionary _dictionary;
        private readonly DragAndDrop _dragAndDrop;
        private readonly GameData _gameData;

        private readonly List<Word> _createdWords = new();

        public PlayerStepState(IStateSwitcher stateSwitcher, PlayingField playingField, NewLettersPanel newLettersPanel,
            LettersPool lettersPool, SortingDictionary dictionary, DragAndDrop dragAndDrop, GameData gameData)
        {
            _stateSwitcher = stateSwitcher;
            _playingField = playingField;
            _newLettersPanel = newLettersPanel;
            _lettersPool = lettersPool;
            _dictionary = dictionary;
            _dragAndDrop = dragAndDrop;
            _gameData = gameData;
        }

        public void Enter()
        {
            _dragAndDrop.CanDrag = true;
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
                CheckingGridForCorrectnessWords();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _stateSwitcher.SwitchState<PCStepState>();
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
            var freeCells = _newLettersPanel.GetFreeCells();
            var randomLetters = CreateRandomLettersList(freeCells.Count);
            for (int i = 0; i < freeCells.Count; i++)
            {
                var tile = _lettersPool.GetTile(randomLetters[i]);
                freeCells[i].AddTile(tile);
            }
        }

        private void CheckingGridForCorrectnessWords()
        {
            _createdWords.Clear();
            List<Word> words = _playingField.GetWordsOnField();
            
            foreach (var word in words)
            {
                if (IsWordValid(word))
                {
                    _playingField.MarkTilesAsPartOfRightWord(word.Tiles);
                    _createdWords.Add(word);
                    //TODO:Хрень для дебага
                    Debug.Log(
                        $"Created word: |{word.StringWord}|, word points with multiplication - |{word.WordPoint}|");
                }
                else if (word.StringWord.Length > 1)
                    Debug.Log($"Word |{word.StringWord}| not found or set not right");
            }

            var pointPerStep = CalculatePointsPointPerStep();
            Debug.Log($"Points per step - {pointPerStep}");
        }

        private bool IsWordValid(Word word) //TODO: в FC
        {
            bool isNewWordInField = true;
            foreach (var createdWord in _gameData.CreatedWords)
                if (string.Equals(createdWord.StringWord, word.StringWord, StringComparison.OrdinalIgnoreCase))
                    isNewWordInField = false;

            bool wordFits = word.Tiles.Any(tile => tile.InRightWord);
            bool wordExists = !string.IsNullOrEmpty(_dictionary.TryFoundWord(word.StringWord).Word);
            return isNewWordInField && wordExists && wordFits;
        }

        private int CalculatePointsPointPerStep()
        {
            var pointPerStep = 0;
            foreach (var word in _createdWords) pointPerStep += word.WordPoint;
            return pointPerStep;
        }
    }
}