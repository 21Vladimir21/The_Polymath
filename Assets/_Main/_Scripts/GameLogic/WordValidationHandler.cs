using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using UnityEngine;

namespace _Main._Scripts.GameLogic
{
    public class WordValidationHandler
    {
        private readonly SortingDictionary _dictionary;
        private readonly FieldFacade _fieldFacade;
        private readonly CurrentGameData _currentGameData;
        private readonly List<Word> _createdWords = new();

        public WordValidationHandler(SortingDictionary dictionary, FieldFacade fieldFacade,
            CurrentGameData currentGameData)
        {
            _dictionary = dictionary;
            _fieldFacade = fieldFacade;
            _currentGameData = currentGameData;
        }


        public void CheckingGridForCorrectnessWords(ref int points, bool endStep)
        {
            _createdWords.Clear();
            List<Word> words = _fieldFacade.GetWordsOnField();

            foreach (var word in words)
            {
                if (IsWordValid(word) == ValidationResults.Success)
                {
                    _fieldFacade.MarkTilesAsPartOfRightWord(word.Tiles);
                    if (endStep)
                        word.MarkTilesInWord();
                    _createdWords.Add(word);
                    //TODO:Хрень для дебага
                    Debug.Log(
                        $"Created word: |{word.StringWord}|, word points with multiplication - |{word.WordPoint}|");
                }
                else if (IsWordValid(word) == ValidationResults.SetNotRight && word.StringWord.Length > 1)
                    Debug.Log($"Word |{word.StringWord}| set not right");
                else if (IsWordValid(word) == ValidationResults.NotFound && word.StringWord.Length > 1)
                    Debug.Log($"Word |{word.StringWord}| not found");
                else if (IsWordValid(word) == ValidationResults.WordWasPosted && word.StringWord.Length > 1)
                    Debug.Log($"Word |{word.StringWord}| was posed");
            }

            points = CalculatePointsPointPerStep();
        }

        private ValidationResults IsWordValid(Word word)
        {
            foreach (var createdWord in _currentGameData.CreatedWords)
                if (string.Equals(createdWord.StringWord, word.StringWord, StringComparison.OrdinalIgnoreCase))
                {
                    if (createdWord.GetWordStartCoordinates == word.GetWordStartCoordinates)
                        return ValidationResults.WordWasCreated;
                    return ValidationResults.WordWasPosted;
                }

            bool wordFits = word.Tiles.Any(tile => tile.InRightWord);
            if (wordFits == false) return ValidationResults.SetNotRight;


            var foundWord = _dictionary.TryFoundWord(word.StringWord);
            bool wordExists = foundWord != null && !string.IsNullOrEmpty(foundWord.Word);
            if (wordExists == false) return ValidationResults.NotFound;

            return ValidationResults.Success;
        }

        private int CalculatePointsPointPerStep()
        {
            var pointPerStep = 0;
            foreach (var word in _createdWords) pointPerStep += word.WordPoint;
            return pointPerStep;
        }
    }

    public enum ValidationResults
    {
        SetNotRight,
        NotFound,
        WordWasPosted,
        WordWasCreated,
        Success,
    }
}