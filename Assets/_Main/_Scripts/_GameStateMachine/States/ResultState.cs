using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public class ResultState : IState
    {
        private readonly IStateSwitcher _switcher;
        private readonly CurrentGameData _gameData;
        private readonly NewLettersPanel _newLettersPanel;

        public ResultState(IStateSwitcher switcher, CurrentGameData gameData,NewLettersPanel newLettersPanel)
        {
            _switcher = switcher;
            _gameData = gameData;
            _newLettersPanel = newLettersPanel;
        }

        public void Enter()
        {
            _newLettersPanel.ReturnAllTilesToPool();
            if (_gameData.PCPoints > _gameData.PlayerPoints)
            {
                Debug.Log($"Победил бот");
            }
            else
            {
                Debug.Log($"Победил кожанный");
            }

            _switcher.SwitchState<EntryState>();
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}