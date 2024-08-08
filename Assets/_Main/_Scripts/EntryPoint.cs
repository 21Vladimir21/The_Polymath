using _Main._Scripts._GameStateMachine;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;

namespace _Main._Scripts
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameField gameField;
        [SerializeField] private Transform lettersParent;
        [SerializeField] private SortingDictionary dictionary;
        
        
        private GameStateMachine _gameStateMachine;
        private LettersPool _lettersPool;


        [SerializeField] private LettersPoolConfig _lettersPoolConfig;
        
        
        private void Awake()
        {
            _lettersPool = new LettersPool(_lettersPoolConfig, lettersParent);
            _gameStateMachine = new GameStateMachine(gameField,_lettersPool,dictionary);


        }

        private void Update()
        {
            _gameStateMachine.Update();
        }
    }
}