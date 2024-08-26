using System;
using System.Collections.Generic;
using _Main._Scripts.GameLogic;
using UnityEngine;

namespace _Main._Scripts.DictionaryLogic
{
    [Serializable]
    public class WordLengthDictionaryHolder
    {
        [HideInInspector] public string name;

        public WordLengthDictionaryHolder(int wordLength)
        {
            name = $"Dictionary with words {wordLength} letters length ";
            WordLength = wordLength;
            var lettersArray = Enum.GetValues(typeof(Letters));

            for (int i = 0; i < lettersArray.Length - 1; i++)
            {
                Letters letter = (Letters)lettersArray.GetValue(i);
                if (letter is Letters.Ь or Letters.Ъ or Letters.Ы 
                    // or Letters.JackPot
                    ) continue;

                FirstLetterWordHolders.Add(new FirstLetterWordHolder(letter));
            }
        }

        [field: SerializeField] public int WordLength { get; private set; }

        [field: SerializeField] public List<FirstLetterWordHolder> FirstLetterWordHolders { get; private set; } = new();

        public bool TryAddNewWord(DictionaryWord word)
        {
            var charArray = word.Word.ToCharArray();
            if (charArray.Length < WordLength)
            {
                foreach (var wordHolder in FirstLetterWordHolders)
                {
                    if (wordHolder.TryAddWord(word)) return true;

                    return false;
                }
            }

            return false;
        }
    }
}