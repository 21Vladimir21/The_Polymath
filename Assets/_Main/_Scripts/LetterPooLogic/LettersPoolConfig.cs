using System;
using System.Collections.Generic;
using _Main._Scripts.GameFieldLogic;
using UnityEngine;

namespace _Main._Scripts.LetterPooLogic
{
    [CreateAssetMenu(fileName = "LettersPoolConfig", menuName = "Configs/Create New Letters Pool Config", order = 0)]
    public class LettersPoolConfig : ScriptableObject
    {
        [field: SerializeField] public List<LetterPoolData> LetterPoolDates { get; private set; }
        [SerializeField] private List<LetterTile> tilePrefabs;


        [ContextMenu("UpdateLetters")]
        private void UpdateLettersDataWithMissingLevels()
        {
            foreach (var letter in Enum.GetValues(typeof(Letters)))
            {
                bool levelFound = false;

                foreach (var data in LetterPoolDates)
                {
                    if (data.Letter == (Letters)letter)
                    {
                        levelFound = true;
                        break;
                    }
                }

                if (!levelFound)
                {
                    LetterPoolData newData = new LetterPoolData();
                    newData.Letter = (Letters)letter;
                    newData.name = letter.ToString();
                    foreach (var prefab in tilePrefabs)
                        if (prefab.Letter == (Letters)letter)
                            newData.TilePrefab = prefab;
                    LetterPoolDates.Add(newData);
                }
            }
        }
    }


    [Serializable]
    public class LetterPoolData
    {
        [HideInInspector] public string name;
        [field: SerializeField] public Letters Letter { get; set; }
        [field: SerializeField] public LetterTile TilePrefab { get; set; }
        [field: Range(1, 10), SerializeField] public int Count { get; private set; } = 10;
    }
}