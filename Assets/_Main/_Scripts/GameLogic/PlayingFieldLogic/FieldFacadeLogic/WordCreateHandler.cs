using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.DictionaryLogic;
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

        public WordCreateHandler(PlayingFieldCell[,] grid, SortingDictionary dictionary,
            LettersPool lettersPool, UnityEvent<int> onDecreaseRemainingTiles,
            UnityEvent<int> onDecreaseRemainingPoints, LettersDataHolder lettersDataHolder)
        {
            _grid = grid;
            _dictionary = dictionary;
            _lettersPool = lettersPool;
            _onDecreaseRemainingTiles = onDecreaseRemainingTiles;
            _onDecreaseRemainingPoints = onDecreaseRemainingPoints;
            _lettersDataHolder = lettersDataHolder;
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
                    var modifiedWord = MaskLetterAtPosition(candidateWord.Word, randomLetterPositionInWord);

                    var remainingTilesNow = IsEnoughTiles(remainedTiles, modifiedWord);
                    if (remainingTilesNow < 0) continue;

                    if (!CanFitWordInAvailableCells(modifiedWord, space.FreeCellsFromBeginningLetter,
                            space.FreeCellsFromEndLetter)) continue;
                    
                    if (!IsPointsWithinLimits(modifiedWord, ref remainedPoints, candidateWord.Word,
                            space.IsHorizontal, space.LetterTile.TileCoordinates)) continue;

                    var letterTiles = TilesFounded(modifiedWord);
                    if (letterTiles == null) continue;

                    PlaceLettersOnGrid(modifiedWord, space.IsHorizontal, space.LetterTile.TileCoordinates, letterTiles);
                    _onDecreaseRemainingTiles?.Invoke(remainingTilesNow);
                    _onDecreaseRemainingPoints?.Invoke(remainedPoints);
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
                if (!IsPointsWithinLimits(modifiedWord, ref remainedPoints, candidateWord.Word,
                        wordFreeCellsInfo.Word.IsHorizontal, wordFreeCellsInfo.StartWordCoords)) continue;

                var letterTiles = TilesFounded(modifiedWord);
                if (letterTiles == null) continue;

                PlaceLettersOnGrid(modifiedWord, wordFreeCellsInfo, letterTiles);
                _onDecreaseRemainingTiles?.Invoke(remainingTilesNow);
                _onDecreaseRemainingPoints?.Invoke(remainedPoints);

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

                    if (isHorizontal)
                        _grid[startCoords.x, newIndex].AddTile(tile);
                    else
                        _grid[newIndex, startCoords.y].AddTile(tile);

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

        private bool IsPointsWithinLimits(string modifiedWord, ref int maxPoints, string newWord, bool isHorizontal,
            Vector2Int startCoords)
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
                    WordMultiplication(newWord, cell, ref wordMultiplication, i, ref points);
                }
                else
                {
                    var cell = _grid[newIndex + i, startCoords.y];
                    WordMultiplication(newWord, cell, ref wordMultiplication, i, ref points);
                }
            }

            if (points <= maxPoints)
            {
                points *= wordMultiplication;
                maxPoints -= points;
                return true;
            }

            Debug.Log($"Слово {newWord} имеет {points * wordMultiplication} очков ");
            return false;
        }

        private void WordMultiplication(string newWord, PlayingFieldCell cell, ref int wordMultiplication, int i,
            ref int points)
        {
            if (cell.IsWordMultiplication)
            {
                wordMultiplication = cell.MultiplicationBonus;
            }
            else
            {
                var cellMultiplication = cell.MultiplicationBonus;
                int letterPoints = 0;
                if (_lettersDataHolder.TryGetLetterPoints(newWord[i], ref letterPoints))
                    points += letterPoints * cellMultiplication;
            }
        }
    }
}