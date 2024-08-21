using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public class PCStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldFacade _fieldFacade;
        private readonly GameData _gameData;

        public PCStepState(IStateSwitcher stateSwitcher, FieldFacade fieldFacade)
        {
            _stateSwitcher = stateSwitcher;
            _fieldFacade = fieldFacade;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                _fieldFacade.CheckAndPlaceWord();
                _stateSwitcher.SwitchState<PlayerStepState>();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _fieldFacade.CheckAndPlaceWordFromLetter();
                _stateSwitcher.SwitchState<PlayerStepState>();
            }
        }
    }
}