using _Main._Scripts.GameDatas;
using _Main._Scripts.GameFieldLogic;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public class PCStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly FieldController _fieldController;
        private readonly GameData _gameData;

        public PCStepState(IStateSwitcher stateSwitcher, FieldController fieldController)
        {
            _stateSwitcher = stateSwitcher;
            _fieldController = fieldController;
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
                _fieldController.CheckAndPlaceWord();
                _stateSwitcher.SwitchState<PlayerStepState>();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _fieldController.CreateWordFromLetter();
                _stateSwitcher.SwitchState<PlayerStepState>();
            }
        }
    }
}