using System.Collections.Generic;
using _Main._Scripts.GameLogic.LettersLogic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Main._Scripts.GameLogic
{
    public class Word
    {
        public  List<LetterTile> Tiles;
        private int _wordPointsMultiplierValue = 1;


        public Word(List<LetterTile> tiles, bool isHorizontal, int multiplierValue = 1)
        {
            Tiles = tiles;
            _wordPointsMultiplierValue = multiplierValue;
            IsHorizontal = isHorizontal;
            foreach (var tile in tiles)
            {
                tile.OnSwapped.RemoveAllListeners();
                tile.OnSwapped.AddListener(SwapTile);
            }
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

        public void MarkTilesInWord()
        {
            foreach (var tile in Tiles) tile.SetInWord();
        }
        
        private void SwapTile(LetterTile tile,LetterTile newTile)
        {
            Debug.Log($"Swap tile {tile.name} to {newTile.name}");
            var indexOf = Tiles.IndexOf(tile);
            Tiles.RemoveAt(indexOf);
            Tiles.Insert(indexOf,newTile);
            newTile.SetInWord();
            tile.OnSwapped.RemoveAllListeners();
        }
    }
}