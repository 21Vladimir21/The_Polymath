using System.Collections.Generic;
using System.Linq;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;

namespace _Main._Scripts._GameStateMachine
{
    public class GameStateMachine : IStateSwitcher
    {
        private readonly GameField _gameField;
        private readonly LettersPool _lettersPool;
        private readonly List<IState> _states;
        private IState _currentState;


        public GameStateMachine(GameField gameField, LettersPool lettersPool)
        {
            _gameField = gameField;
            _lettersPool = lettersPool;

            _states = new List<IState>
            {
                new PlayerStepState(gameField, lettersPool)
            };

            _currentState = _states[0];
            _currentState.Enter();
        }

        public void SwitchState<TState>() where TState : IState
        {
            var state = _states.FirstOrDefault(state => state is TState);

            _currentState.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public void Update() => _currentState.Update();
    }
}