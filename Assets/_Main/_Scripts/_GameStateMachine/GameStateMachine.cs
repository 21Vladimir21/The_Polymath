using System.Collections.Generic;
using System.Linq;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;

namespace _Main._Scripts._GameStateMachine
{
    public class GameStateMachine : IStateSwitcher
    {
        private readonly List<IState> _states;
        private IState _currentState;


        public GameStateMachine(PlayingField playingField, NewLettersPanel newLettersPanel, LettersPool lettersPool,
            SortingDictionary dictionary, DragAndDrop dragAndDrop)
        {
            var gameData = new GameData();
            var fieldController = new FieldController(playingField, gameData,dictionary,lettersPool);

            _states = new List<IState>
            {
                new InitState(this, dictionary, playingField, lettersPool, gameData),
                new PlayerStepState(this, playingField, newLettersPanel, lettersPool, dictionary, dragAndDrop,
                    gameData,fieldController),
                new PCStepState(this, fieldController),
                new MainMenuState()
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