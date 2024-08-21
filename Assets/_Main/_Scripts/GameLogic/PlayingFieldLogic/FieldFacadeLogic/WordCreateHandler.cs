using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.LetterPooLogic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic
{
    public class WordCreateHandler
    {
        private readonly PlayingFieldCell[,] _grid;
        private readonly SortingDictionary _dictionary;
        private readonly PlayingField _playingField;
        private readonly LettersPool _lettersPool;

        public WordCreateHandler(PlayingFieldCell[,] grid,SortingDictionary dictionary, PlayingField playingField, LettersPool lettersPool)
        {
            _grid = grid;
            _dictionary = dictionary;
            _playingField = playingField;
            _lettersPool = lettersPool;
        }

        public async UniTask<bool> CanPlaceWord(LetterFreeSpaceInfo space)
        {
            var randomLetterPositionInWord = Random.Range(0, space.FreeCellsFromBeginningLetter);
            var words = await _dictionary.GetWordsWithLetterAtPosition(space.LetterTile.LetterString,
                randomLetterPositionInWord);

            foreach (var candidateWord in words)
            {
                var modifiedWord = MaskLetterAtPosition(candidateWord.Word, randomLetterPositionInWord);
                if (!CanFitWordInAvailableCells(modifiedWord, space.FreeCellsFromBeginningLetter,
                        space.FreeCellsFromEndLetter)) continue;

                var letterTiles = TilesFounded(modifiedWord);
                if (letterTiles == null) continue;

                PlaceLettersOnGrid(modifiedWord, space.IsHorizontal, space.LetterTile.TileCoordinates, letterTiles);

                Debug.Log(
                    $"Начальная буква {space.LetterTile.LetterString} , Модифицированное слово {modifiedWord}, поставленное слово {candidateWord.Word}");
                return true;
            }

            return false;
        }

        public async UniTask<bool> CanPlaceWord(WordFreeCellsInfo wordFreeCellsInfo)
        {
            var words = await _dictionary.GetWordsFromWordPart(wordFreeCellsInfo.Word.StringWord);

            foreach (var candidateWord in words)
            {
                var modifiedWord = MaskMatchingLetters(wordFreeCellsInfo.Word.StringWord, candidateWord.Word);

                if (!CanFitWordInAvailableCells(modifiedWord, wordFreeCellsInfo.FreeCellsFromBeginningWord,
                        wordFreeCellsInfo.FreeCellsFromEndWord)) continue;

                var letterTiles = TilesFounded(modifiedWord);
                if (letterTiles == null) continue;

                PlaceLettersOnGrid(modifiedWord, wordFreeCellsInfo, letterTiles);

                Debug.Log(
                    $"Начальное слово {wordFreeCellsInfo.Word.StringWord} , Модифицированное слово {modifiedWord}, поставленное слово {candidateWord.Word}");

                return true;
            }

            return false;
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
    }
}