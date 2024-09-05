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
        [field: SerializeField] public Button SwapTilesButton { get; private set; }
        [field: SerializeField] public Button RewardSwapTilesButton { get; private set; }
    }
}