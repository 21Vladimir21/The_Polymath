using UnityEngine;

namespace _Main._Scripts.GameLogic.PlayingFieldLogic
{
    public class PlayingField : MonoBehaviour
    {
        [field: SerializeField] public PlayingFieldCell[] Cells { get; private set; }
    }
}