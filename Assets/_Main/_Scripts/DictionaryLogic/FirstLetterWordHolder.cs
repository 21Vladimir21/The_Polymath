using System;
using System.Collections.Generic;
using _Main._Scripts.GameLogic;
using UnityEngine;

namespace _Main._Scripts.DictionaryLogic
{
    [Serializable]
    public class FirstLetterWordHolder
    {
        [HideInInspector] public string name;

        public FirstLetterWordHolder(Letters firstLetter)
        {
            FirstLetter = firstLetter;
            name = $"Words with |{firstLetter}| first letter";
        }

        [field: SerializeField] public Letters FirstLetter { get; private set; }
        [field: SerializeField] public List<DictionaryWord> Words { get; private set; } = new();

        public bool TryAddWord(DictionaryWord word)
        {
            var charArray = word.Word.ToCharArray();
            if (charArray[0].ToString() == FirstLetter.ToString())
            {
                Words.Add(word);
                name = $"Words with |{FirstLetter.ToString()}| first letter. Words count - |{Words.Count}|";
                return true;
            }

            Debug.Log($"Word - {word}  have not needed first letter");
            return false;
        }
    }
}