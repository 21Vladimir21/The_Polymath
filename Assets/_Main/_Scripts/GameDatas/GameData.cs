using System.Collections.Generic;
using _Main._Scripts.GameFieldLogic;

namespace _Main._Scripts.GameDatas
{
    public class GameData
    {
        public List<Word> CreatedWords { get; private set; } = new();
        public int PlayerPoints;
        public int PCPoints;

        public void AddNewWords(List<Word> words)
        {
           CreatedWords.Clear();
            CreatedWords.AddRange(words);
        }
    }
}