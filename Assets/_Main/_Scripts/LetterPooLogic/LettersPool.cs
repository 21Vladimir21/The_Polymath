using System;
using System.Collections.Generic;
using _Main._Scripts.GameLogic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Main._Scripts.LetterPooLogic
{
    public class LettersPool
    {
        private readonly LettersPoolConfig _config;
        private readonly Transform _lettersParent;
        private readonly List<LetterTile> _spawnedTiles = new();

        public LettersPool(LettersPoolConfig config, Transform lettersParent)
        {
            _config = config;
            _lettersParent = lettersParent;
            SpawnLetters();
        }

        private void SpawnLetters()
        {
            foreach (var poolDate in _config.LetterPoolDates)
            {
                for (int i = 0; i < poolDate.Count; i++)
                {
                    var tile = Object.Instantiate(poolDate.TilePrefab, _lettersParent);
                    tile.gameObject.SetActive(false);
                    _spawnedTiles.Add(tile);
                }
            }
        }

        public LetterTile GetTileFromChar(char letter)
        {
            var enumLetter = Enum.Parse<Letters>(letter.ToString().ToUpper());
            return GetTile(enumLetter);
        }

        public LetterTile GetTile(Letters letter)
        {
            foreach (var tile in _spawnedTiles)
                if (tile.Letter == letter && tile.gameObject.activeSelf == false)
                {
                    tile.gameObject.SetActive(true);
                    return tile;
                }

            return null;
        }

        public void ReturnTile(LetterTile tile) => tile.gameObject.SetActive(false);
    }
}