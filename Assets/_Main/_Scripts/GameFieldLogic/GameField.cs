using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.GameFieldLogic
{
    public class GameField : MonoBehaviour
    {
        [field: SerializeField] private GameFieldSell[] cells;

        [SerializeField] private List<LetterTile> letterTiles;

        private GameFieldSell[,] _grid;
        private DragAndDrop _dragAndDrop;
        private FieldChecker _fieldChecker;
        private WordCreator _wordCreator;

        private List<string> _words = new()
        {
            "кам", "мак", "скам", "хуй", "смак",
        };

        private List<string> _createdWords = new();

        private void Start()
        {
            InitializeGrid();
            _fieldChecker = new FieldChecker(_grid);
            _wordCreator = new WordCreator(_grid);
            CreateRandomWord();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CheckingGridForCorrectnessWords();
            }
        }

        private void InitializeGrid()
        {
            _grid = new GameFieldSell [5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _grid[i, j] = cells[i * 5 + j];
                }
            }
        }

        //TODO: тестовая штука , надо доработать и возможно убрать отсюда когда будет сделана инициализация игры 
        private void CreateRandomWord()
        {
            var randomWordIndex = Random.Range(0, _words.Count);
            var randomWord = _words[randomWordIndex];
            var parsedWord = randomWord.ToCharArray();
            if (parsedWord.Length > 3)
            {
                CreateRandomWord();
                return;
            }

            var testStartIndexes = new Vector2[]
            {
                new(2, 1),
                new(2, 2),
                new(2, 3)
            };


            for (int i = 0; i < testStartIndexes.Length; i++)
            {
                var tilePrefab = letterTiles.FirstOrDefault(x =>
                    x.Letter.Equals(parsedWord[i].ToString(), StringComparison.OrdinalIgnoreCase));
                var tile = Instantiate(tilePrefab, transform); //TODO:Сделать взятие букв из пула 
                _grid[(int)testStartIndexes[i].x, (int)testStartIndexes[i].y].AddTile(tile);
                tile.SetOnField();
            }
        }
        
        //TODO:Все что ниже вынести в отдельный класс
        private void CheckingGridForCorrectnessWords()
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
                    Debug.Log($"Created word: {word.StringWord}");
                }
                else if (word.StringWord.Length > 1) 
                    Debug.Log($"Word {word.StringWord} not found");
            }
        }

        private bool IsWordValid(Word word) //TODO: в FC
        {
            bool wordFits = word.Tiles.Any(tile => tile.InRightWord);
            return _words.Any(w => string.Equals(w, word.StringWord, StringComparison.OrdinalIgnoreCase)) &&
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
