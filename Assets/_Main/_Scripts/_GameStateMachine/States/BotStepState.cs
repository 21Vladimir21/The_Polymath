using System;
using _Main._Scripts.BotLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using _Main._Scripts.Services;
using _Main._Scripts.UI;
using _Main._Scripts.UI.InfoPanel;
using _Main._Scripts.UI.Views;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts._GameStateMachine.States
{
    public class BotStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;
        private readonly BotComplexitySettings[] _complexitySettings;
        private readonly CurrentGameData _gameData;
        private readonly InfoPanelHandler _infoPanel;

        private BotComplexitySettings _currentComplexity;
        private readonly InGameView _inGameView;

        private const int MaxTiles = 7;
        private const int PointForAllTiles = 15;

        private int _remainedTiles;
        private int _remainedPoints;


        public BotStepState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade,
            BotComplexityHolder complexityHolder, CurrentGameData gameData, InfoPanelHandler infoPanel)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
            _complexitySettings = complexityHolder.BotComplexitySettings.ToArray();
            _gameData = gameData;
            _infoPanel = infoPanel;
            _fieldFacade.OnDecreaseRemainingTiles.AddListener(UpdateRemainingTiles);
            _fieldFacade.OnDecreaseRemainingPoints.AddListener(UpdateRemainingPoints);

            var locator = ServiceLocator.Instance.GetServiceByType<UILocator>();

            _inGameView = locator.GetViewByType<InGameView>();
        }

        public void Enter()
        {
            SetComplexity();
            SetRemainedPoints();
            SetRemainedTiles();
            _infoPanel.ShowPanelWithInfo(InfoKey.BotStep, PlaceWords);
            _inGameView.UpdatePoints(_gameData.PlayerPoints, _gameData.BotPoints);
            //TODO: Добавить проверку на то сколько осталось плиток в пуле 
        }

        public void Exit()
        {
            _gameData.BotPoints += _remainedTiles <= 0 ? PointForAllTiles : 0;
            _fieldFacade.SaveGrid();
        }


        public void Update()
        {
        }

        private void MoveTilesToCellsAndActivateCellShine(Action callback)
        {
            if (_fieldFacade.LastPlacedTileCells.Count <= 0)
            {
                callback?.Invoke();
                return;
            }

            foreach (var cell in _fieldFacade.LastPlacedTileCells)
                cell.AnimatedMoveTileToCell(() => cell.ActivateShine(callback));

            _fieldFacade.LastPlacedTileCells.Clear();
        }

        private async void PlaceWords()
        {
            var value = Random.Range(0, 2);
            if (value == 0)
            {
                while (true)
                    if (await _fieldFacade.CheckAndPlaceWord(_remainedTiles, _remainedPoints) == false ||
                        _remainedTiles <= 0)
                        break;
            }
            else
            {
                while (true)
                    if (await _fieldFacade.CheckAndPlaceWordFromLetter(_remainedTiles, _remainedPoints) == false ||
                        _remainedTiles <= 0)
                        break;
            }

            if (_gameData.GameStarted == false) return;
            if (_remainedTiles >= _currentComplexity.MaxTilesPerStep)
            {
                _infoPanel.ShowPanelWithKeyRange(InfoKey.BotSwapTiles, InfoKey.BotMissedStep, () =>
                {
                    MoveTilesToCellsAndActivateCellShine(() =>
                    {
                        if (_gameData.HasBeenRequiredPoints)
                            _stateSwitcher.SwitchState<ResultState>();
                        else
                            _stateSwitcher.SwitchState<PlayerStepState>();
                    });
                });
            }
            else
            {
                MoveTilesToCellsAndActivateCellShine(() =>
                {
                    if (_gameData.HasBeenRequiredPoints)
                        _stateSwitcher.SwitchState<ResultState>();
                    else
                        _stateSwitcher.SwitchState<PlayerStepState>();
                });
            }
        }

        private void SetComplexity(bool setForce = false)
        {
            if (_currentComplexity == null || _currentComplexity.Complexity != _gameData.Complexity || setForce)
            {
                foreach (var setting in _complexitySettings)
                    if (setting.Complexity.Equals(_gameData.Complexity))
                        _currentComplexity = setting;
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