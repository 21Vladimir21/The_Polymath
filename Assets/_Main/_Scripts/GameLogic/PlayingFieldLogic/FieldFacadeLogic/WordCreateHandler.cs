using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.LettersLogic;
using _Main._Scripts.LetterPooLogic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic
{
    public class WordCreateHandler
    {
        private readonly PlayingFieldCell[,] _grid;
        private readonly SortingDictionary _dictionary;
        private readonly LettersPool _lettersPool;
        private readonly UnityEvent<int> _onDecreaseRemainingTiles;
        private readonly UnityEvent<int> _onDecreaseRemainingPoints;
        private readonly LettersDataHolder _lettersDataHolder;
        private readonly CurrentGameData _gameData;

        public WordCreateHandler(PlayingFieldCell[,] grid, SortingDictionary dictionary,
            LettersPool lettersPool, UnityEvent<int> onDecreaseRemainingTiles,
            UnityEvent<int> onDecreaseRemainingPoints, LettersDataHolder lettersDataHolder, CurrentGameData gameData)
        {
            _grid = grid;
            _dictionary = dictionary;
            _lettersPool = lettersPool;
            _onDecreaseRemainingTiles = onDecreaseRemainingTiles;
            _onDecreaseRemainingPoints = onDecreaseRemainingPoints;
            _lettersDataHolder = lettersDataHolder;
            _gameData = gameData;
        }

        public async UniTask<bool> CanPlaceWord(LetterFreeSpaceInfo space, int remainedTiles, int remainedPoints)
        {
            HashSet<int> usedLetterPositions = new();

            while (usedLetterPositions.Count < space.FreeCellsFromBeginningLetter)
            {
                var randomLetterPositionInWord = Random.Range(0, space.FreeCellsFromBeginningLetter);
                if (!usedLetterPositions.Add(randomLetterPositionInWord)) continue;

                var words = await _dictionary.GetWordsWithLetterAtPosition(space.LetterTile.LetterString,
                    randomLetterPositionInWord);

                foreach (var candidateWord in words)
                {
                    var wordNotFound = true;
                    foreach (var word in _gameData.CreatedWords)
                        if (string.Equals(word.StringWord, candidateWord.Word, StringComparison.OrdinalIgnoreCase))
                            wordNotFound = false;

                    if (wordNotFound == false) continue;

                    var modifiedWord = MaskLetterAtPosition(candidateWord.Word, randomLetterPositionInWord);

                    var remainingTilesNow = IsEnoughTiles(remainedTiles, modifiedWord);
                    if (remainingTilesNow < 0) continue;

                    if (!CanFitWordInAvailableCells(modifiedWord, space.FreeCellsFromBeginningLetter,
                            space.FreeCellsFromEndLetter)) continue;

                    if (!IsWordPointsWithLimits(candidateWord.Word, ref remainedPoints))
                        continue;

                    var letterTiles = TilesFounded(modifiedWord);
                    if (letterTiles == null) continue;

                    PlaceLettersOnGrid(modifiedWord, space.IsHorizontal, space.LetterTile.TileCoordinates, letterTiles);
                    _onDecreaseRemainingTiles?.Invoke(remainingTilesNow);
                    _onDecreaseRemainingPoints?.Invoke(remainedPoints);

                    UpdatePointsWithNewWord(modifiedWord, candidateWord.Word, space.IsHorizontal,
                        space.LetterTile.TileCoordinates);
                    Debug.Log(
                        $"Начальная буква {space.LetterTile.LetterString} , Модифицированное слово {modifiedWord}, поставленное слово {candidateWord.Word}");
                    return true;
                }
            }

            return false;
        }

        public async UniTask<bool> CanPlaceWord(WordFreeCellsInfo wordFreeCellsInfo, int remainedTiles,
            int remainedPoints)
        {
            var words = await _dictionary.GetWordsFromWordPart(wordFreeCellsInfo.Word.StringWord);

            foreach (var candidateWord in words)
            {
                var modifiedWord = MaskMatchingLetters(wordFreeCellsInfo.Word.StringWord, candidateWord.Word);

                var remainingTilesNow = IsEnoughTiles(remainedTiles, modifiedWord);
                if (remainingTilesNow < 0) continue;

                if (!CanFitWordInAvailableCells(modifiedWord, wordFreeCellsInfo.FreeCellsFromBeginningWord,
                        wordFreeCellsInfo.FreeCellsFromEndWord)) continue;
                if (!IsWordPointsWithLimits(candidateWord.Word, ref remainedPoints))
                    continue;

                var letterTiles = TilesFounded(modifiedWord);
                if (letterTiles == null) continue;

                PlaceLettersOnGrid(modifiedWord, wordFreeCellsInfo, letterTiles);
                _onDecreaseRemainingTiles?.Invoke(remainingTilesNow);
                _onDecreaseRemainingPoints?.Invoke(remainedPoints);

                UpdatePointsWithNewWord(modifiedWord, candidateWord.Word, wordFreeCellsInfo.Word.IsHorizontal,
                    wordFreeCellsInfo.StartWordCoords);

                Debug.Log(
                    $"Начальное слово {wordFreeCellsInfo.Word.StringWord} , Модифицированное слово {modifiedWord}, поставленное слово {candidateWord.Word}");

                return true;
            }

            return false;
        }

        private int IsEnoughTiles(int remainedTiles, string modifiedWord)
        {
            int needPutTiles = 0;
            foreach (var letter in modifiedWord)
            {
                if (letter.Equals('-'))
                    continue;
                needPutTiles++;
            }

            return remainedTiles - needPutTiles;
        }

        private void PlaceLettersOnGrid(string modifiedWord,
            WordFreeCellsInfo wordFreeCellsInfo, List<LetterTile> tiles) =>
            PlaceLettersOnGrid(modifiedWord, wordFreeCellsInfo.Word.IsHorizontal,
                wordFreeCellsInfo.Word.GetWordStartCoordinates, tiles);

        private void PlaceLettersOnGrid(string modifiedWord, bool isHorizontal, Vector2Int startCoords,
            List<LetterTile> tiles)
        {
            var startLetterIndex = modifiedWord.IndexOf('-');

            for (int i = 0; i < modifiedWord.Length; i++)
            {
                if (modifiedWord[i].Equals('-')) continue;

                var newIndex = (isHorizontal ? startCoords.y : startCoords.x) + (i - startLetterIndex);


                foreach (var tile in tiles)
                {
                    if (!string.Equals(tile.LetterString, modifiedWord[i].ToString(),
                            StringComparison.OrdinalIgnoreCase)) continue;

                    Vector2Int coords;
                    if (isHorizontal)
                        coords = new Vector2Int(startCoords.x, newIndex);
                    else
                        coords = new Vector2Int(newIndex, startCoords.y);

                    _grid[coords.x, coords.y].AddTile(tile);
                    tile.SetInWord();

                    tiles.Remove(tile);
                    break;
                }
            }
        }

        private string MaskMatchingLetters(string word, string wordForModified)
        {
            var maskWordPart = new string('-', word.Length);
            return wordForModified.Replace(word, maskWordPart, StringComparison.OrdinalIgnoreCase);
        }

        private string MaskLetterAtPosition(string word, int index) => word.Remove(index, 1).Insert(index, "-");

        private bool CanFitWordInAvailableCells(string dictionaryWord, int freeCellsFromBeginning, int freeCellsFromEnd)
        {
            var startLetterIndex =
                dictionaryWord.IndexOf('-');
            var endLetterIndex =
                dictionaryWord.LastIndexOf('-');


            int lettersToBeginning = 0;
            int lettersToEnd = 0;

            for (var index = 0; index < dictionaryWord.Length; index++)
            {
                if (index < startLetterIndex)
                    lettersToBeginning++;
                if (index > endLetterIndex)
                    lettersToEnd++;
            }

            if (lettersToBeginning > freeCellsFromBeginning ||
                lettersToEnd > freeCellsFromEnd)
                return false;

            return true;
        }

        private List<LetterTile> TilesFounded(string word)
        {
            List<LetterTile> foundedTiles = new();
            var charArray = word.ToList();
            for (int j = 0; j < charArray.Count; j++)
            {
                var letter = charArray[j];
                if (letter.Equals('-')) continue;
                var tile = _lettersPool.GetTileFromChar(letter);
                if (tile) foundedTiles.Add(tile);
                else
                {
                    foreach (var foundedTile in foundedTiles)
                        _lettersPool.ReturnTile(foundedTile);
                    return null;
                }
            }

            return foundedTiles;
        }

        private bool IsWordPointsWithLimits(string newWord, ref int maxPoints)
        {
            int wordPoints = 0;
            foreach (var letter in newWord)
            {
                int letterPoints = 0;
                if (_lettersDataHolder.TryGetLetterPoints(letter, ref letterPoints))
                {
                    wordPoints += letterPoints;
                }
            }

            if (wordPoints > maxPoints) return false;

            maxPoints = wordPoints;
            return true;
        }


        private void UpdatePointsWithNewWord(string modifiedWord, string newWord, bool isHorizontal,
            Vector2Int coordinates)
        {
            var wordPoints = GetWordPoints(modifiedWord, newWord, isHorizontal,
                coordinates);
            _gameData.PCPoints += wordPoints;
        }

        private int GetWordPoints(string modifiedWord, string newWord, bool isHorizontal, Vector2Int startCoords)
        {
            var startLetterIndex = modifiedWord.IndexOf('-');

            var newIndex = (isHorizontal ? startCoords.y : startCoords.x) - startLetterIndex;
            var points = 0;
            var wordMultiplication = 1;

            for (int i = 0; i < newWord.Length; i++)
            {
                if (isHorizontal)
                {
                    var cell = _grid[startCoords.x, newIndex + i];
                    CellMultiplication(newWord, cell, ref wordMultiplication, i, ref points);
                }
                else
                {
                    var cell = _grid[newIndex + i, startCoords.y];
                    CellMultiplication(newWord, cell, ref wordMultiplication, i, ref points);
                }
            }


            points *= wordMultiplication;
            Debug.Log($"Слово {newWord} имеет {points} очков ");
            return points;
        }

        private void CellMultiplication(string newWord, PlayingFieldCell cell, ref int wordMultiplication, int i,
            ref int points)
        {
            var cellMultiplication = 1;
            if (cell.IsWordMultiplication)
            {
                if (cell.WasUsed == false)
                {
                    wordMultiplication = cell.MultiplicationBonus;
                    cell.WasUsed = true;
                }
            }
            else
            {
                if (cell.WasUsed == false)
                {
                    cellMultiplication = cell.MultiplicationBonus;
                    cell.WasUsed = true;
                }
            }


            int letterPoints = 0;
            if (_lettersDataHolder.TryGetLetterPoints(newWord[i], ref letterPoints))
                points += letterPoints * cellMultiplication;
        }
    }
}