using _Main._Scripts.BotLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public class BotStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;
        private readonly BotComplexitySettings[] _complexitySettings;
        private readonly CurrentGameData _gameData;

        private const int MaxTiles = 7;
        private int _remainedTiles;
        private int _remainedPoints;
        private BotComplexitySettings _currentComplexity;

        public BotStepState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade,
            BotComplexitySettings[] complexitySettings, CurrentGameData gameData)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
            _complexitySettings = complexitySettings;
            _gameData = gameData;
            _fieldFacade.OnDecreaseRemainingTiles.AddListener(UpdateRemainingTiles);
            _fieldFacade.OnDecreaseRemainingPoints.AddListener(UpdateRemainingPoints);
        }

        public void Enter()
        {
            SetComplexity();
            SetRemainedPoints();
            SetRemainedTiles();


            //TODO: Добавить проверку на то сколько осталось плиток в пуле 
        }

        public void Exit()
        {
            Debug.Log($"Очки бота: {_gameData.PCPoints}");
        }

        public async void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                while (true)
                    if (await _fieldFacade.CheckAndPlaceWord(_remainedTiles, _remainedPoints) == false ||
                        _remainedTiles <= 0)
                        break;

                _stateSwitcher.SwitchState<PlayerStepState>();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                while (true)
                    if (await _fieldFacade.CheckAndPlaceWordFromLetter(_remainedTiles, _remainedPoints) == false ||
                        _remainedTiles <= 0)
                        break;

                _stateSwitcher.SwitchState<PlayerStepState>();
            }
        }

        private void SetComplexity(bool setForce = false)
        {
            if (_currentComplexity == null || _currentComplexity.Complexity != _gameData.Complexity || setForce)
            {
                foreach (var setting in _complexitySettings)
                    if (setting.Complexity.Equals(_gameData.Complexity))
                    {
                        _currentComplexity = setting;
                        Debug.Log($"Установленная сложность {_gameData.Complexity.ToString()}");
                    }
            }
        }

        private void SetRemainedPoints()
        {
            var randomChance = Random.Range(0, 101);
            foreach (var pointsChances in _currentComplexity.Chances)
            {
                if (randomChance >= pointsChances.MinChanceValue && randomChance < pointsChances.MaxChanceValue)
                    _remainedPoints = pointsChances.Points;
            }
        }

        private void SetRemainedTiles()
        {
            var value = Random.Range(0, 101);
            if (value <= _currentComplexity.ChanceUseAllTiles)
                _remainedTiles = MaxTiles;
            else
                _remainedTiles = _currentComplexity.MaxTilesPerStep;
        }

        private void UpdateRemainingTiles(int remainingTilesNow) => _remainedTiles = remainingTilesNow;

        private void UpdateRemainingPoints(int remainingPointsNow) => _remainedPoints = remainingPointsNow;
    }
}