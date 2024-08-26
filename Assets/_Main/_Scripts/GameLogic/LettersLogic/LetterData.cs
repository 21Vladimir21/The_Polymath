using UnityEngine;

namespace _Main._Scripts.GameLogic
{
    [CreateAssetMenu(fileName = "LetterData", menuName = "Configs/NewLetterData", order = 0)]
    public class LetterData : ScriptableObject
    {
        [field: SerializeField] public Letters Letter { get; private set; }
        [field: SerializeField, Range(1, 10)] public int Points { get; private set; }
    }
}