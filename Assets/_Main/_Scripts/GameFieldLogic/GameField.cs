using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.GameFieldLogic
{
    public class GameField : MonoBehaviour
    {
        [field: SerializeField] public NewLettersPanel NewLettersPanel { get; private set; }

        [field: SerializeField] private GameFieldCell[] cells;
        [SerializeField] private List<LetterTile> letterTiles;

        private SortingDictionary _dictionary;

        private GameFieldCell[,] _grid;
        private DragAndDrop _dragAndDrop;
        private FieldChecker _fieldChecker;
        private WordCreator _wordCreator;

        private List<string> _createdWords = new();

        public void Init(SortingDictionary dictionary)
        {
            _dictionary = dictionary;
            InitializeGrid();
            _fieldChecker = new FieldChecker(_grid);
            _wordCreator = new WordCreator(_grid);
        }

        private void InitializeGrid()
        {
            var maxGridLength = 15;
            _grid = new GameFieldCell [maxGridLength, maxGridLength];
            for (int i = 0; i < maxGridLength; i++)
            {
                for (int j = 0; j < maxGridLength; j++)
                {
                    _grid[i, j] = cells[i * maxGridLength + j];
                }
            }
        }

        //TODO: тестовая штука , надо доработать и возможно убрать отсюда когда будет сделана инициализация игры 
        public void CreateRandomWord()
        {
            var randomWord = _dictionary.GetRandomWordWithLength(3);
            var parsedWord = randomWord.ToCharArray();
            if (parsedWord.Length > 3)
            {
                CreateRandomWord();
                return;
            }

            var testStartIndexes = new Vector2[]
            {
                new(7, 6),
                new(7, 7),
                new(7, 8)
            };


            for (int i = 0; i < testStartIndexes.Length; i++)
            {
                var tilePrefab = letterTiles.FirstOrDefault(x =>
                    x.LetterString.Equals(parsedWord[i].ToString(), StringComparison.OrdinalIgnoreCase));
                var tile = Instantiate(tilePrefab, transform); //TODO:Сделать взятие букв из пула 
                _grid[(int)testStartIndexes[i].x, (int)testStartIndexes[i].y].AddTile(tile);
                tile.SetOnField();
            }
        }

        //TODO:Все что ниже вынести в отдельный класс
        public void CheckingGridForCorrectnessWords()
        {
            var startWordsKeyValuePairs = _fieldChecker.FindStartCellsIndexes();
            _createdWords.Clear();

            List<Word> words = new();
            foreach (var keyValuePair in startWordsKeyValuePairs)
                words.AddRange(_wordCreator.CreateAWords(keyValuePair.Key, keyValuePair.Value));

            foreach (var word in words)
            {
                if (IsWordValid(word)) //TODO: сделать проверку на повторение с уже созданными словами 
                {
                    MarkTilesAsPartOfWord(word.Tiles);
                    _createdWords.Add(word.StringWord);
                    //TODO:Хрень для дебага
                    Debug.Log(
                        $"Created word: |{word.StringWord}|, word points with multiplication - |{word.WordPoint}|");
                }
                else if (word.StringWord.Length > 1)
                    Debug.Log($"Word |{word.StringWord}| not found or set not right");
            }
        }

        private bool IsWordValid(Word word) //TODO: в FC
        {
            bool wordFits = word.Tiles.Any(tile => tile.InRightWord);
            return !string.IsNullOrEmpty(_dictionary.TryFoundWord(word.StringWord).Word) &&
                   wordFits;
        }

        private void MarkTilesAsPartOfWord(List<LetterTile> currentWordTiles) //TODO: в FC
        {
            foreach (var tile in currentWordTiles)
            {
                tile.SetInRightWord();
            }
        }
    }
}