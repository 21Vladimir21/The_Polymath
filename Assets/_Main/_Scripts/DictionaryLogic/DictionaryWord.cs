using System;
using UnityEngine;

namespace _Main._Scripts.DictionaryLogic
{
    [Serializable]
    public class DictionaryWord
    {
        [field: SerializeField] public string Word { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        public DictionaryWord(string word, string description)
        {
            Word = word;
            Description = description;
        }
    }
}