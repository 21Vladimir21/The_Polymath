using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameLogic
{
    [CreateAssetMenu(fileName = "LettersDataHolder", menuName = "NewLettersDataHolder", order = 0)]
    public class LettersDataHolder : ScriptableObject
    {
        [field: SerializeField] public List<LetterData> Datas { get; private set; }

        public bool TryGetLetterPoints(char letter, ref int points)
        {
            try
            {
                var enumLetter = Enum.Parse<Letters>(letter.ToString(), true);
                return TryGetLetterPoints(enumLetter, ref points);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return false;
            }
        }

        public bool TryGetLetterPoints(Letters letter, ref int points)
        {
            foreach (var data in Datas)
            {
                if (data.Letter.Equals(letter))
                {
                    points = data.Points;
                    return true;
                }
            }

            return false;
        }
    }
}