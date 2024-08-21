using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.GameLogic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _Main._Scripts.DictionaryLogic
{
    [CreateAssetMenu(fileName = "SortingDictionary", menuName = "Create new sorting dictionary", order = 0)]
    public class SortingDictionary : ScriptableObject
    {
        [SerializeField] private TextAsset words;

        [SerializeField] private List<WordLengthDictionaryHolder> dictionaryHolders = new();


        public List<string> GetWordsFromFirstLetterAndLength(Letters firstLetter, int length)
        {
            List<string> foundWords = new();
            foreach (var dictionaryHolder in dictionaryHolders)
            {
                if (dictionaryHolder.WordLength != length) continue;
                if (length > dictionaryHolder.WordLength) break;

                foreach (var wordHolder in dictionaryHolder.FirstLetterWordHolders)
                {
                    if (wordHolder.FirstLetter == firstLetter)
                    {
                        foundWords.AddRange(wordHolder.Words.Select(x => x.Word));
                        break;
                    }
                }
            }

            return foundWords.Count > 0 ? foundWords : null;
        }


        public string GetWordDescription(string word)
        {
            var dictionaryWord = TryFoundWord(word);
            return dictionaryWord.Description;
        }

        public async UniTask<List<DictionaryWord>> GetWordsFromWordPart(string word)
        {
            List<DictionaryWord> foundedWords = new();
            var wordLength = word.Length;
            foreach (var holder in dictionaryHolders)
            {
                
                if (holder.WordLength > wordLength)
                    foreach (var letterWordHolder in holder.FirstLetterWordHolders)
                    foreach (var dictionaryWord in letterWordHolder.Words)
                        if (dictionaryWord.Word.Contains(word, StringComparison.OrdinalIgnoreCase))
                            foundedWords.Add(dictionaryWord);
                
                await UniTask.Yield();
            }

            return foundedWords;
        }

        public async UniTask<List<DictionaryWord>> GetWordsWithLetterAtPosition(string letter, int indexInWord = 0)
        {
            List<DictionaryWord> foundedWords = new();
            foreach (var holder in dictionaryHolders)
            {
                if (holder.WordLength <= indexInWord) continue;
                foreach (var letterWordHolder in holder.FirstLetterWordHolders)
                {
                    foreach (var dictionaryWord in letterWordHolder.Words)
                        if (string.Equals(dictionaryWord.Word[indexInWord].ToString(),
                                letter, StringComparison.OrdinalIgnoreCase))
                        {
                            foundedWords.Add(dictionaryWord);
                        }
                }

                await UniTask.Yield();
            }

            return foundedWords;
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
                    return randomWord.Word;
                }
            }

            return default;
        }


        [ContextMenu("Fill dictionaries with new words")]
        private void FillDictionaries()
        {
            DeserializeData data = JsonConvert.DeserializeObject<DeserializeData>(words.text);

            var wordDictionary = data.Words;

            foreach (var valuePair in wordDictionary)
            {
                var trimWord = valuePair.Key.Trim();
                string newWord = trimWord;
                if (trimWord.Contains('-', StringComparison.OrdinalIgnoreCase))
                    continue;
                if (trimWord.Contains('ё', StringComparison.OrdinalIgnoreCase))
                    newWord = trimWord.Replace("ё", "е", StringComparison.OrdinalIgnoreCase);

                var toUpperFirstLetterWord = CapitalizeFirstLetter(newWord);
                var wordLength = toUpperFirstLetterWord.ToCharArray().Length;
                var firstLetter = toUpperFirstLetterWord.ToCharArray()[0].ToString();


                foreach (var holder in dictionaryHolders)
                {
                    if (holder.WordLength != wordLength) continue;
                    foreach (var wordHolder in holder.FirstLetterWordHolders)
                    {
                        if (wordHolder.FirstLetter.ToString() != firstLetter) continue;
                        bool wordFound = false;
                        foreach (var holderWord in wordHolder.Words)
                        {
                            if (toUpperFirstLetterWord != holderWord.Word) continue;
                            wordFound = true;
                            break;
                        }

                        if (wordFound == false)
                            wordHolder.TryAddWord(new DictionaryWord(toUpperFirstLetterWord, valuePair.Value));
                    }
                }
            }
        }

        public DictionaryWord TryFoundWord(string word)
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
                        if (!string.Equals(word, holderWord.Word,
                                StringComparison.OrdinalIgnoreCase)) continue;
                        return holderWord;
                    }

                    return default;
                }
            }

            return default;
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
        public Dictionary<string, string> Words { get; set; }
    }
}