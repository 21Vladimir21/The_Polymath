using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.LetterPooLogic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic
{
    public class FieldFacade
    {
        public readonly UnityEvent<int> OnDecreaseRemainingTiles = new();
        public readonly UnityEvent<int> OnDecreaseRemainingPoints = new();

        private readonly PlayingField _playingField;
        private readonly FieldFreeSpaceHandler _fieldFreeSpaceHandler;
        private readonly WordCreateHandler _wordCreateHandler;
        private readonly WordSearchHandler _wordSearchHandler;
        private readonly CurrentGameData _currentGameData;
        private readonly SortingDictionary _dictionary;
        private readonly LettersPool _lettersPool;

        private PlayingFieldCell[,] _grid;
        private const int MaxGridLength = 15;

        public FieldFacade(PlayingField playingField, CurrentGameData currentGameData, SortingDictionary dictionary,
            LettersPool lettersPool, LettersDataHolder lettersDataHolder)
        {
            _playingField = playingField;
            _currentGameData = currentGameData;
            _dictionary = dictionary;
            _lettersPool = lettersPool;
            
            InitializeGrid();
            
            _fieldFreeSpaceHandler = new FieldFreeSpaceHandler(_grid, _playingField, _currentGameData);
            _wordCreateHandler = new WordCreateHandler(_grid, dictionary, lettersPool, OnDecreaseRemainingTiles,
                OnDecreaseRemainingPoints, lettersDataHolder, _currentGameData);
            _wordSearchHandler = new WordSearchHandler(_grid);
        }

        private void InitializeGrid()
        {
            _grid = new PlayingFieldCell [MaxGridLength, MaxGridLength];
            for (int i = 0; i < MaxGridLength; i++)
            for (int j = 0; j < MaxGridLength; j++)
            {
                _grid[i, j] = _playingField.Cells[i * MaxGridLength + j];
                _grid[i, j].SetCellCoords(i, j);
            }
        }


        public async UniTask<bool> CheckAndPlaceWord(int remainedTiles, int remainedPoints)
        {
            var freeSpaceInfos =
                _fieldFreeSpaceHandler.TryGetWordFreeSpaceInfo();
            if (freeSpaceInfos == null)
            {
                Debug.Log("Не нашлось нужных слов для составления нового слова!");
                return false;
            }

            foreach (var info in freeSpaceInfos)
            {
                if (!await _wordCreateHandler.CanPlaceWord(info, remainedTiles, remainedPoints)) continue;
                UpdateFieldWords();
                return true;
            }

            return false;
        }

        public async UniTask<bool> CheckAndPlaceWordFromLetter(int remainedTiles, int remainedPoints)
            //TODO:Наверное можно сделать generic класс и скоратить код до 1 метода 
        {
            var letterFreeSpaceInfos = _fieldFreeSpaceHandler.TryGetStartCells();

            if (letterFreeSpaceInfos == null)
            {
                Debug.Log("Для составления слова нет буквы на доске!");
                return false;
            }

            foreach (var space in letterFreeSpaceInfos)
                // TODO: Добавить рандомный выьор первой точки , что бы бот не ставил в предсказуемое место 
            {
                if (!await _wordCreateHandler.CanPlaceWord(space, remainedTiles, remainedPoints)) continue;
                UpdateFieldWords();
                return true;
            }

            return false;
        }

        public void ClearNotRightTiles()
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            for (int j = 0; j < _grid.GetLength(1); j++)
                if (_grid[i, j].CurrentTile != null && _grid[i, j].CurrentTile.InRightWord == false)
                    _grid[i, j].ClearTileData();
        }

        public void ClearMovableTiles()
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            for (int j = 0; j < _grid.GetLength(1); j++)
                if (_grid[i, j].CurrentTile != null && _grid[i, j].CurrentTile.CanMove)
                    _grid[i, j].ClearTileData();
        }

        public void UpdateFieldWords()
        {
            var words = GetWordsOnField();
            _currentGameData.AddNewWords(words);
        }

        public List<Word> GetWordsOnField()
        {
            var startWordsKeyValuePairs = _wordSearchHandler.FindStartCellsIndexes();

            List<Word> words = new();
            foreach (var keyValuePair in startWordsKeyValuePairs)
                words.AddRange(_wordSearchHandler.CreateAWords(keyValuePair.Key, keyValuePair.Value));
            return words;
        }

        public void MarkTilesAsPartOfRightWord(List<LetterTile> currentWordTiles)
        {
            foreach (var tile in currentWordTiles)
                tile.MarkInRightWord();
        }

        public void CreateRandomStartWord()
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
                _grid[testStartIndexes[i].x, testStartIndexes[i].y].AddTile(tile);
                tile.SetOnField(testStartIndexes[i]);
            }

            _currentGameData.CreatedWords.Add(new Word(tiles, true));

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