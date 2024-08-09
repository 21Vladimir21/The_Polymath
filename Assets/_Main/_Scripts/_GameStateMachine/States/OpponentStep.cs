using System;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts._GameStateMachine.States
{
    public class OpponentStep : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldChecker _fieldChecker;
        private readonly WordCreator _wordCreator;
        private readonly GameField _gameField;
        private readonly SortingDictionary _dictionary;
        private readonly LettersPool _lettersPool;

        public OpponentStep(IStateSwitcher stateSwitcher, FieldChecker fieldChecker, WordCreator wordCreator,
            GameField gameField,
            SortingDictionary dictionary, LettersPool lettersPool)
        {
            _stateSwitcher = stateSwitcher;
            _fieldChecker = fieldChecker;
            _wordCreator = wordCreator;
            _gameField = gameField;
            _dictionary = dictionary;
            _lettersPool = lettersPool;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                NewMethod();
            }
        }

        private void NewMethod()
        {
            var space = _fieldChecker.FindNeedsCellsForOpponent();


            var firstCell = _gameField.Grid[(int)space.Coordinates.x, (int)space.Coordinates.y];
            if (space.Equals(default(FreeSpaceInfo)))
            {
                return;
            }
            var randomWordLength = Random.Range(2, space.Length);
            var words = _dictionary.GetWordsFromFirstLetterAndLength(firstCell.CurrentTile.Letter, randomWordLength);
            if (words.Count <= 0)
            {
                NewMethod();
                return;
            }

            var word = words[Random.Range(0,words.Count)];
            var charArray = word.ToList();

            var startIndex = 0;
            if (space.Direction == CheckingFieldDirection.Horizontal)
                startIndex = (int)space.Coordinates.y + 1;
            else if (space.Direction == CheckingFieldDirection.Vertical)
                startIndex = (int)space.Coordinates.x + 1;

            for (int i = 1; i < charArray.Count; i++)
            {
                var letter = charArray[i].ToString().ToUpper();
                var enumLetter = Enum.Parse<Letters>(letter);
                var tile = _lettersPool.GetTile(enumLetter);
                if (space.Direction == CheckingFieldDirection.Horizontal)
                    _gameField.Grid[(int)space.Coordinates.x, startIndex].AddTile(tile);
                else if (space.Direction == CheckingFieldDirection.Vertical)
                    _gameField.Grid[startIndex, (int)space.Coordinates.y].AddTile(tile);
                tile.SetOnField();
                startIndex++;
            }
        }
    }
}