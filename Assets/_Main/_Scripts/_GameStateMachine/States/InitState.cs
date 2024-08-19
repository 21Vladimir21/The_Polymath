using System;
using System.Collections.Generic;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public class InitState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly SortingDictionary _dictionary;
        private readonly PlayingField _playingField;
        private readonly LettersPool _lettersPool;
        private readonly GameData _gameData;

        public InitState(IStateSwitcher stateSwitcher, SortingDictionary dictionary, PlayingField playingField,
            LettersPool lettersPool,
            GameData gameData)
        {
            _stateSwitcher = stateSwitcher;
            _dictionary = dictionary;
            _playingField = playingField;
            _lettersPool = lettersPool;
            _gameData = gameData;
        }

        public void Enter()
        {
            CreateRandomStartWord();
            _stateSwitcher.SwitchState<PlayerStepState>();
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }

        private void CreateRandomStartWord()
        {
            var randomWord = _dictionary.GetRandomWordWithLength(3);
            var parsedWord = randomWord.ToCharArray();

            List<LetterTile> tiles = new();
            var testStartIndexes = new Vector2Int[]
            {
                new(7, 6),
                new(7, 7),
                new(7, 8)
            };
            for (int i = 0; i < testStartIndexes.Length; i++)
            {
                var letter = parsedWord[i].ToString().ToUpper();
                var enumLetter = Enum.Parse<Letters>(letter);
                var tile = _lettersPool.GetTile(enumLetter);
                tiles.Add(tile);
                _playingField.Grid[testStartIndexes[i].x, testStartIndexes[i].y].AddTile(tile);
                tile.SetOnField(testStartIndexes[i]);
            }

            _gameData.CreatedWords.Add(new Word(tiles,true));

            //TODO: Надо бы убрать , но часто юзаю для тестов :)
            var enumLetter2 = Enum.Parse<Letters>("Е");
            // var enumLetter3 = Enum.Parse<Letters>("П");
            // var enumLetter4 = Enum.Parse<Letters>("Ы");
            var tile2 = _lettersPool.GetTile(enumLetter2);
            // // Grid[6,12].AddTile(tile2);
            // var tile4 = _lettersPool.GetTile(enumLetter3);
            // var tile3 = _lettersPool.GetTile(enumLetter4);
            // _playingField.Grid[6,1].AddTile(tile2);
            // Grid[13,7].AddTile(tile3);
            // Grid[14,7].AddTile(tile4);
            // tile2.SetOnField();
            // FieldChecker.CheckFreeCellsFromStartCell(6,7);

            // FieldChecker.FindNeedsCellsForOpponent();
        }
    }
}