using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class Word
    {
        public readonly List<LetterTile> Tiles;
        private int _wordPointsMultiplierValue = 1;


        public Word(List<LetterTile> tiles, bool isHorizontal, int multiplierValue = 1)
        {
            Tiles = tiles;
            _wordPointsMultiplierValue = multiplierValue;
            IsHorizontal = isHorizontal;
        }

        public bool IsHorizontal { get; private set; }
        public Vector2Int GetWordStartCoordinates => Tiles[0].TileCoordinates;
        public Vector2Int GetWordEndCoordinates => Tiles[^1].TileCoordinates;

        public string StringWord
        {
            get
            {
                string word = string.Empty;
                foreach (var tile in Tiles) word += tile.LetterString;
                return word;
            }
        }

        public int WordPoint
        {
            get
            {
                var points = 0;
                foreach (var tile in Tiles) points += tile.Points;
                return points * _wordPointsMultiplierValue;
            }
        }
    }
}