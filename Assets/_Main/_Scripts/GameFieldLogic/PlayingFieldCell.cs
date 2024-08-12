using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class PlayingFieldCell : TileCell
    {
        [field: SerializeField, Range(1, 3)] public int MultiplicationBonus { get; private set; }
        [field: SerializeField] public bool IsWordMultiplication { get; private set; }
        
    
    }
}