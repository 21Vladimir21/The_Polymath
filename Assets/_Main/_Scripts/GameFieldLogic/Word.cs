using System.Collections.Generic;

namespace _Main._Scripts.GameFieldLogic
{
    public class Word
    {
        public readonly List<LetterTile> Tiles;
        private int _wordPointsMultiplierValue = 1;


        public Word(List<LetterTile> tiles,int multiplierValue)
        {
            Tiles = tiles;
            _wordPointsMultiplierValue = multiplierValue;
        }

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