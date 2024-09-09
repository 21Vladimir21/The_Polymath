using System.Collections.Generic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.GameLogic.SwapTilesLogic
{
    public class SwapTilesPanelView : MonoBehaviour
    {
        [field: SerializeField] public List<TileCell> Cells { get; private set; }
        [field: SerializeField] public GameObject Panel { get; private set; }

        [field: Header("Buttons")]
        [field: SerializeField]
        public Button OpenPanelButton { get; private set; }

        [field: SerializeField] public Button HidePanelButton { get; private set; }
        [field: SerializeField] public Button AllTilesButton { get; private set; }
        [field: SerializeField] public Button ApplyButton { get; private set; }
        [field: SerializeField] public Button RewardApplyButton { get; private set; }
        [field: SerializeField] public Button CancelButton { get; private set; }
    }
}