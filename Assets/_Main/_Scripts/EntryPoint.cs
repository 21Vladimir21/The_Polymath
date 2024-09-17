using System.Collections.Generic;
using _Main._Scripts._GameStateMachine;
using _Main._Scripts.BotLogic;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic;
using _Main._Scripts.LetterPooLogic;
using _Main._Scripts.Services;
using _Main._Scripts.Services.FaderService;
using _Main._Scripts.UI;
using _Main._Scripts.UI.InfoPanel;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main._Scripts
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private DragAndDrop dragAndDrop;

        [SerializeField] private PlayingField playingField;
        [SerializeField] private NewLettersPanel newLettersPanel;
        [SerializeField] private Transform lettersParent;

        [SerializeField] private LettersPoolConfig lettersPoolConfig;
        [SerializeField] private SortingDictionary dictionary;

        [SerializeField] private LettersDataHolder lettersDataHolder;
        [SerializeField] private BotComplexityHolder botComplexityHolder;
        [SerializeField] private InfosHolder infosHolder;

        [SerializeField] private UIViewsHolder uiViewsHolder;
        [SerializeField] private FadeService fadeService;

        private GameStateMachine _gameStateMachine;
        private LettersPool _lettersPool;


        private void Awake()
        {
            var uiLocator = new UILocator(uiViewsHolder);
            ServiceLocator.Instance.TryAddService(uiLocator);
            ServiceLocator.Instance.TryAddService(fadeService);

            _lettersPool = new LettersPool(lettersPoolConfig, lettersParent);
            var infoHandler = new InfoPanelHandler(infosHolder);
            _gameStateMachine = new GameStateMachine(playingField, newLettersPanel, _lettersPool, dictionary,
                dragAndDrop, lettersDataHolder, botComplexityHolder,infoHandler);
        }

        private void Update()
        {
            _gameStateMachine.Update();
        }
    }
}