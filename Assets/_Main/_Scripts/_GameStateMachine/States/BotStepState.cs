using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public class BotStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;
        private readonly GameData _gameData;


        private const int MaxTiles = 7;
        private int _remainedTiles;
        private int _remainedPoints;

        public BotStepState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
            _fieldFacade.OnDecreaseRemainingTiles.AddListener(UpdateRemainingTiles);
            _fieldFacade.OnDecreaseRemainingPoints.AddListener(UpdateRemainingPoints);
        }

        public void Enter()
        {
            //TODO: Добавить проверку на то сколько осталось плиток в пуле 
            _remainedTiles = MaxTiles;
            _remainedPoints = 8;
        }

        public void Exit()
        {
        }

        public async void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                while (true)
                    if (await _fieldFacade.CheckAndPlaceWord(_remainedTiles,_remainedPoints) == false || _remainedTiles <= 0)
                        break;

                _stateSwitcher.SwitchState<PlayerStepState>();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                while (true)
                    if (await _fieldFacade.CheckAndPlaceWordFromLetter(_remainedTiles,_remainedPoints) == false || _remainedTiles <= 0)
                        break;

                _stateSwitcher.SwitchState<PlayerStepState>();
            }
        }

        private void UpdateRemainingTiles(int remainingTilesNow) => _remainedTiles = remainingTilesNow;
        private void UpdateRemainingPoints(int remainingPointsNow) => _remainedPoints = remainingPointsNow;
    }
}