using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.GameFieldLogic
{
    public class GameField : MonoBehaviour
    {
        [field: SerializeField] public NewLettersPanel NewLettersPanel { get; private set; }
        [field: SerializeField] public FieldChecker FieldChecker { get; private set; }

        [field: SerializeField] private GameFieldCell[] cells;
        [SerializeField] private List<LetterTile> letterTiles;

        private SortingDictionary _dictionary;

        public GameFieldCell[,] Grid { get; private set; }
        public WordCreator WordCreator{ get; private set; }

        private List<Word> _createdWords = new();
        private List<string> _allCreatedWords = new();
        private LettersPool _lettersPool;

        public void Init(SortingDictionary dictionary, LettersPool lettersPool)
        {
            _lettersPool = lettersPool;
            _dictionary = dictionary;
            InitializeGrid();

            WordCreator = new WordCreator(Grid);
            FieldChecker = new FieldChecker(Grid);
        }

        private void InitializeGrid()
        {
            var maxGridLength = 15;
            Grid = new GameFieldCell [maxGridLength, maxGridLength];
            for (int i = 0; i < maxGridLength; i++)
            {
                for (int j = 0; j < maxGridLength; j++)
                {
                    Grid[i, j] = cells[i * maxGridLength + j];
                }
            }
        }

        //TODO: тестовая штука , надо доработать и возможно убрать отсюда когда будет сделана инициализация игры 
        public void CreateRandomWord()
        {
            var randomWord = _dictionary.GetRandomWordWithLength(3);
            var parsedWord = randomWord.ToCharArray();

            var testStartIndexes = new Vector2[]
            {
                new(6, 7),
                new(7, 7),
                new(8, 7)
            };

            for (int i = 0; i < testStartIndexes.Length; i++)
            {
                var letter = parsedWord[i].ToString().ToUpper();
                var enumLetter = Enum.Parse<Letters>(letter);
                var tile = _lettersPool.GetTile(enumLetter);
                Grid[(int)testStartIndexes[i].x, (int)testStartIndexes[i].y].AddTile(tile);
                tile.SetOnField();
            }

            _allCreatedWords.Add(randomWord);


            //TODO: Надо бы убрать , но часто юзаю для тестов :)
            // var enumLetter2 = Enum.Parse<Letters>("Е");
            // var enumLetter3 = Enum.Parse<Letters>("П");
            // var enumLetter4 = Enum.Parse<Letters>("Ы");
            // var tile2 = _lettersPool.GetTile(enumLetter2);
            // // Grid[6,12].AddTile(tile2);
            // var tile4 = _lettersPool.GetTile(enumLetter3);
            // var tile3 = _lettersPool.GetTile(enumLetter4);
            // Grid[12,7].AddTile(tile2);
            // Grid[13,7].AddTile(tile3);
            // Grid[14,7].AddTile(tile4);
            // tile2.SetOnField();
            // FieldChecker.CheckFreeCellsFromStartCell(6,7);

            // FieldChecker.FindNeedsCellsForOpponent();
        }


        public void EndStep()
        {
            var pointPerStep = CalculatePointsPointPerStep();
            _allCreatedWords.AddRange(_createdWords.Select(x => x.StringWord));
            Debug.Log($"Points per step - {pointPerStep}");
        }

        private int CalculatePointsPointPerStep()
        {
            var pointPerStep = 0;
            foreach (var word in _createdWords) pointPerStep += word.WordPoint;
            return pointPerStep;
        }

        //TODO:Все что ниже вынести в отдельный класс
        public void CheckingGridForCorrectnessWords()
        {
            var startWordsKeyValuePairs = FieldChecker.FindStartCellsIndexes();
            _createdWords.Clear();

            List<Word> words = new();
            foreach (var keyValuePair in startWordsKeyValuePairs)
                words.AddRange(WordCreator.CreateAWords(keyValuePair.Key, keyValuePair.Value));

            foreach (var word in words)
            {
                if (IsWordValid(word)) //TODO: сделать проверку на повторение с уже созданными словами 
                {
                    MarkTilesAsPartOfWord(word.Tiles);
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
            foreach (var createdWord in _allCreatedWords)
                if (string.Equals(createdWord, word.StringWord))
                    isNewWordInField = false;

            bool wordFits = word.Tiles.Any(tile => tile.InRightWord);
            bool wordExists = !string.IsNullOrEmpty(_dictionary.TryFoundWord(word.StringWord).Word);
            return isNewWordInField && wordExists && wordFits;
        }

        private void MarkTilesAsPartOfWord(List<LetterTile> currentWordTiles) //TODO: в FC
        {
            foreach (var tile in currentWordTiles)
            {
                tile.SetInRightWord();
            }
        }


        private void ShowPossibleWords()
        {
            var startWordsKeyValuePairs = FieldChecker.FindStartCellsIndexes();
        }
    }
}