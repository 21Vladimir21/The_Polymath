using System.Collections.Generic;
using _Main._Scripts.BotLogic;
using _Main._Scripts.GameLogic;
using _Main._Scripts.Services;
using _Main._Scripts.Services.Saves;

namespace _Main._Scripts.GameDatas
{
    public class CurrentGameData
    {
        public BotComplexity Complexity { get; private set; }
        public List<Word> CreatedWords { get; private set; } = new();

        public int PlayerPoints
        {
            get => _saves.PlayerPoints;
            set => _saves.PlayerPoints = value;
        }

        public int PCPoints
        {
            get => _saves.BotPoints;
            set => _saves.BotPoints = value;
        }

        private readonly Saves _saves;

        private const int PointsToWin = 200;

        public CurrentGameData()
        {
            var savesService = ServiceLocator.Instance.GetServiceByType<SavesService>();
            _saves = savesService.Saves;

            if (_saves.HasStartGame)
            {
                PlayerPoints = _saves.PlayerPoints;
                PCPoints = _saves.BotPoints;
            }
        }

        public bool HasBeenRequiredPoints => PlayerPoints >= PointsToWin || PCPoints >= PointsToWin;

        public void AddNewWords(List<Word> words)
        {
            CreatedWords.Clear();
            CreatedWords.AddRange(words);
        }

        public void SetComplexity(BotComplexity botComplexity) => Complexity = botComplexity;

        public void ClearData()
        {
            CreatedWords.Clear();
            PlayerPoints = 0;
            PCPoints = 0;
        }
    }
}