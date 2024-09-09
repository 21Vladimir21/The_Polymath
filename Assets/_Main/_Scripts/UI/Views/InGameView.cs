using _Main._Scripts.GameLogic.SwapTilesLogic;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.UI.Views
{
    public class InGameView : AbstractView
    {
        [field: SerializeField] public Button CheckWordsButton { get; private set; }
        [field: SerializeField] public Button EndStepButton { get; private set; }
        [field: SerializeField] public Button ReturnLettersToPanelButton { get; private set; }
        [field: SerializeField] public Button MixTilesButton { get; private set; }
        [field: SerializeField] public SwapTilesPanelView SwapTilesPanelView { get; private set; }
        
        
    }
}