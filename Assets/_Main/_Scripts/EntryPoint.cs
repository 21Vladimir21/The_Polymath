using _Main._Scripts._GameStateMachine;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main._Scripts
{
    public class EntryPoint : MonoBehaviour
    {
        
        [SerializeField] private DragAndDrop dragAndDrop;
        [FormerlySerializedAs("gameField")] [SerializeField] private PlayingField playingField;
        [SerializeField] private Transform lettersParent;
        [SerializeField] private NewLettersPanel newLettersPanel;
        [SerializeField] private SortingDictionary dictionary;

        
        private GameStateMachine _gameStateMachine;
        private LettersPool _lettersPool;


        [SerializeField] private LettersPoolConfig _lettersPoolConfig;
        
        
        private void Awake()
        {
            _lettersPool = new LettersPool(_lettersPoolConfig, lettersParent);
            _gameStateMachine = new GameStateMachine(playingField,newLettersPanel,_lettersPool,dictionary,dragAndDrop);


        }

        private void Update()
        {
            _gameStateMachine.Update();
        }
    }
}