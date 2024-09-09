using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _Main._Scripts.DictionaryLogic
{
    [CreateAssetMenu(fileName = "SortingDictionary", menuName = "Create new sorting dictionary", order = 0)]
    public class SortingDictionary : ScriptableObject
    {
        [SerializeField] private TextAsset words;

        [SerializeField] private List<WordLengthDictionaryHolder> dictionaryHolders = new();

        public async UniTask<List<string>> GetWordsFromWordPart(string word)
        {
            List<string> foundedWords = new();
            var wordLength = word.Length;
            foreach (var holder in dictionaryHolders)
            {
                if (holder.WordLength > wordLength)
                    foreach (var letterWordHolder in holder.FirstLetterWordHolders)
                    foreach (var dictionaryWord in letterWordHolder.Words)
                        if (dictionaryWord.Contains(word, StringComparison.OrdinalIgnoreCase))
                            foundedWords.Add(dictionaryWord);

                await UniTask.Yield();
            }

            return foundedWords;
        }

        public async UniTask<List<string>> GetWordsWithLetterAtPosition(string letter, int indexInWord = 0)
        {
            List<string> foundedWords = new();
            foreach (var holder in dictionaryHolders)
            {
                if (holder.WordLength <= indexInWord) continue;
                foreach (var letterWordHolder in holder.FirstLetterWordHolders)
                {
                    foreach (var dictionaryWord in letterWordHolder.Words)
                        if (string.Equals(dictionaryWord[indexInWord].ToString(),
                                letter, StringComparison.OrdinalIgnoreCase))
                        {
                            foundedWords.Add(dictionaryWord);
                        }
                }

                await UniTask.Yield();
            }

            return foundedWords;
        }

        public string TryFoundWord(string word)
        {
            var charArray = word.ToCharArray();


            foreach (var holder in dictionaryHolders)
            {
                if (holder.WordLength != charArray.Length) continue;
                foreach (var wordHolder in holder.FirstLetterWordHolders)
                {
                    if (wordHolder.FirstLetter.ToString() != charArray[0].ToString().ToUpper()) continue;
                    foreach (var holderWord in wordHolder.Words)
                    {
                        if (!string.Equals(word, holderWord,
                                StringComparison.OrdinalIgnoreCase)) continue;
                        return holderWord;
                    }

                    return default;
                }
            }

            return default;
        }

        public string GetRandomWordWithLength(int length)
        {
            foreach (var holder in dictionaryHolders)
            {
                if (holder.WordLength != length) continue;

                while (true)
                {
                    var randomFirstLetter = Random.Range(0, holder.FirstLetterWordHolders.Count);
                    if (holder.FirstLetterWordHolders[randomFirstLetter].Words.Count <= 0)
                        continue;
                    var randomWordIndex =
                        Random.Range(0, holder.FirstLetterWordHolders[randomFirstLetter].Words.Count);
                    var randomWord = holder.FirstLetterWordHolders[randomFirstLetter].Words[randomWordIndex];
                    return randomWord;
                }
            }

            return default;
        }

#if UNITY_EDITOR

        [ContextMenu("Fill dictionaries with new words")]
        private void FillDictionaries()
        {
            DeserializeData data = JsonConvert.DeserializeObject<DeserializeData>(words.text);

            var hashSet = data.Words;

            foreach (var word in hashSet)
            {
                var trimWord = word.Trim();
                
                var toUpperFirstLetterWord = CapitalizeFirstLetter(trimWord);
                var wordLength = toUpperFirstLetterWord.ToCharArray().Length;
                var firstLetter = toUpperFirstLetterWord.ToCharArray()[0].ToString();


                foreach (var holder in dictionaryHolders)
                {
                    if (holder.WordLength != wordLength) continue;
                    foreach (var wordHolder in holder.FirstLetterWordHolders)
                    {
                        if (wordHolder.FirstLetter.ToString() != firstLetter) continue;
                            wordHolder.TryAddWord(toUpperFirstLetterWord);
                    }
                }
            }
        }

        [ContextMenu("Create new dictionary holders")]
        private void CrateNewDictionaryHolders()
        {
            const int maxWordLength = 15;

            for (int i = 2; i <= maxWordLength; i++)
            {
                bool holderFound = false;
                foreach (var holder in dictionaryHolders)
                {
                    if (holder.WordLength == i)
                    {
                        holderFound = true;
                        break;
                    }
                }

                if (holderFound == false) dictionaryHolders.Add(new WordLengthDictionaryHolder(i));
            }
        }

        private static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }

    public class DeserializeData
    {
        public List<string> Words { get; set; }
    }
#endif
}