using System;
using System.Collections.Generic;
using _Main._Scripts._GameStateMachine;
using _Main._Scripts.BotLogic;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic;
using _Main._Scripts.GameLogic.SwapTilesLogic;
using _Main._Scripts.LetterPooLogic;
using _Main._Scripts.Services;
using _Main._Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main._Scripts
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private DragAndDrop dragAndDrop;

        [FormerlySerializedAs("gameField")] [SerializeField]
        private PlayingField playingField;

        [SerializeField] private Transform lettersParent;
        [SerializeField] private NewLettersPanel newLettersPanel;
        [SerializeField] private SortingDictionary dictionary;
        [SerializeField] private LettersDataHolder _lettersDataHolder;
        [SerializeField] private List<BotComplexitySettings> _botComplexitySettings;
        [SerializeField] private UIViewsHolder uiViewsHolder;

        [SerializeField] private LettersPoolConfig _lettersPoolConfig;


        private GameStateMachine _gameStateMachine;
        private LettersPool _lettersPool;


        private void Awake()
        {
            var uiLocator = new UILocator(uiViewsHolder);
            ServiceLocator.Instance.TryAddService(uiLocator);
            
            var savesService = ServiceLocator.Instance.GetServiceByType<SavesService>();
            var saves = savesService.Saves;
           

            _lettersPool = new LettersPool(_lettersPoolConfig, lettersParent);
            _gameStateMachine = new GameStateMachine(playingField, newLettersPanel, _lettersPool, dictionary,
                dragAndDrop, _lettersDataHolder, _botComplexitySettings.ToArray());
        }

        private void Update()
        {
            _gameStateMachine.Update();
        }

#if UNITY_EDITOR

        private const int MaxComplexitiesCount = 3;
        private void OnValidate()
        {
            if (_botComplexitySettings.Count > MaxComplexitiesCount)
            {
                _botComplexitySettings.RemoveRange(MaxComplexitiesCount,
                    _botComplexitySettings.Count - MaxComplexitiesCount);
            }
        }
#endif
    }
}