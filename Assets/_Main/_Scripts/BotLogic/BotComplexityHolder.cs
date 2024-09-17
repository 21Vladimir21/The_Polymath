using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.BotLogic
{
    [CreateAssetMenu(fileName = "BotComplexitysHolder", menuName = "Bot/Create new new complexity holder", order = 0)]
    public class BotComplexityHolder : ScriptableObject
    {
        [field: SerializeField] public List<BotComplexitySettings> BotComplexitySettings { get; private set; }

#if UNITY_EDITOR

        private const int MaxComplexitiesCount = 3;
        private void OnValidate()
        {
            if (BotComplexitySettings.Count > MaxComplexitiesCount)
            {
                BotComplexitySettings.RemoveRange(MaxComplexitiesCount,
                    BotComplexitySettings.Count - MaxComplexitiesCount);
            }

            foreach (var setting in BotComplexitySettings) setting.SetNames();
        }
#endif
    }
}