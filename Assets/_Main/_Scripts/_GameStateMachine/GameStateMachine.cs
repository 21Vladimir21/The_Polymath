using System.Collections.Generic;
using System.Linq;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.BotLogic;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
using _Main._Scripts.LetterPooLogic;

namespace _Main._Scripts._GameStateMachine
{
    public class GameStateMachine : IStateSwitcher
    {
        private readonly List<IState> _states;
        private IState _currentState;


        public GameStateMachine(PlayingField playingField, NewLettersPanel newLettersPanel, LettersPool lettersPool,
            SortingDictionary dictionary, DragAndDrop dragAndDrop, LettersDataHolder lettersDataHolder,
            BotComplexitySettings[] settingsArray)
        {
            var gameData = new CurrentGameData();
            var fieldFacade = new FieldFacade(playingField, gameData, dictionary, lettersPool, lettersDataHolder);

            _states = new List<IState>
            {
                new EntryState(this, fieldFacade,gameData),
                new PlayerStepState(this, newLettersPanel, lettersPool, dictionary, dragAndDrop, gameData, fieldFacade),
                new BotStepState(this, fieldFacade, settingsArray, gameData),
                new ResultState(this,gameData,newLettersPanel),
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