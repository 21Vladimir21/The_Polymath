using System.Collections.Generic;

namespace _Main._Scripts.GameFieldLogic
{
    public class Word
    {
        public readonly List<LetterTile> Tiles;

        public Word(List<LetterTile> tiles) => Tiles = tiles;

        public string StringWord
        {
            get
            {
                string word = string.Empty;
                foreach (var tile in Tiles) word += tile.Letter;
                return word;
            }
        }
        
    }
}