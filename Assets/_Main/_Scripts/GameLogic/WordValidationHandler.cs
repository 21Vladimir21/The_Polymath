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
        private readonly GameData _gameData;
        private readonly List<Word> _createdWords = new();

        public WordValidationHandler(SortingDictionary dictionary,FieldFacade fieldFacade,GameData gameData)
        {
            _dictionary = dictionary;
            _fieldFacade = fieldFacade;
            _gameData = gameData;
        }


        public void CheckingGridForCorrectnessWords()
        {
            _createdWords.Clear();
            List<Word> words = _fieldFacade.GetWordsOnField();

            foreach (var word in words)
            {
                if (IsWordValid(word))
                {
                    _fieldFacade.MarkTilesAsPartOfRightWord(word.Tiles);
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

        private bool IsWordValid(Word word)
        {
            bool isNewWordInField = true;
            foreach (var createdWord in _gameData.CreatedWords)
                if (string.Equals(createdWord.StringWord, word.StringWord, StringComparison.OrdinalIgnoreCase))
                    isNewWordInField = false;

            bool wordFits = word.Tiles.Any(tile => tile.InRightWord);
            var foundWord = _dictionary.TryFoundWord(word.StringWord);
            bool wordExists = foundWord != null && !string.IsNullOrEmpty(foundWord.Word);

            return isNewWordInField && wordExists && wordFits;
        }

        private int CalculatePointsPointPerStep()
        {
            var pointPerStep = 0;
            foreach (var word in _createdWords) pointPerStep += word.WordPoint;
            return pointPerStep;
        }
    }
}