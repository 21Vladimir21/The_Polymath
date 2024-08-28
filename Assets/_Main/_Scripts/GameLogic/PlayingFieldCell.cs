using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using UnityEngine;

namespace _Main._Scripts.GameLogic
{
    public class PlayingFieldCell : TileCell
    {
        [field: SerializeField, Range(1, 3)] public int MultiplicationBonus { get; private set; }
        [field: SerializeField] public bool IsWordMultiplication { get; private set; }
        public bool WasUsed;
    }
}