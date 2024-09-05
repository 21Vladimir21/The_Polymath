using System.Collections.Generic;
using _Main._Scripts.BotLogic;
using _Main._Scripts.GameLogic;

namespace _Main._Scripts.GameDatas
{
    public class CurrentGameData
    {
        public BotComplexity Complexity { get; private set; }
        public List<Word> CreatedWords { get; private set; } = new();
        public int PlayerPoints;
        public int PCPoints;

        private const int PointsToWin = 200;

        public bool HasBeenRequiredPoints => PlayerPoints >= PointsToWin || PCPoints >= PointsToWin;

        public void AddNewWords(List<Word> words)
        {
            CreatedWords.Clear();
            CreatedWords.AddRange(words);
        }

        public void SetComplexity(BotComplexity botComplexity) => Complexity = botComplexity;
    }
}