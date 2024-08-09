using System.Collections.Generic;
using System.Linq;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;

namespace _Main._Scripts._GameStateMachine
{
    public class GameStateMachine : IStateSwitcher
    {
        private readonly GameField _gameField;
        private readonly LettersPool _lettersPool;
        private readonly SortingDictionary _dictionary;
        private readonly DragAndDrop _dragAndDrop;
        private readonly List<IState> _states;
        private IState _currentState;
        private readonly FieldChecker _fieldChecker;
        private readonly WordCreator _wordCreator;


        public GameStateMachine(GameField gameField, LettersPool lettersPool, SortingDictionary dictionary,
            DragAndDrop dragAndDrop)
        {
            _gameField = gameField;
            _lettersPool = lettersPool;
            _dictionary = dictionary;
            _dragAndDrop = dragAndDrop;
            _gameField.Init(dictionary, _lettersPool);
            _fieldChecker = _gameField.FieldChecker;
            _wordCreator = _gameField.WordCreator;
            _states = new List<IState>
            {
                new PlayerStepState(this, gameField, lettersPool, dictionary, dragAndDrop),
                new OpponentStep(this, _fieldChecker,_wordCreator, gameField,dictionary,_lettersPool),
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