using System;
using UnityEngine;

namespace _Main._Scripts.BotLogic
{
    [CreateAssetMenu(fileName = "BotSettings", menuName = "NewBotSettings", order = 0)]
    public class BotComplexitySettings : ScriptableObject
    {
        [field: SerializeField] public BotComplexity Complexity { get; private set; }

        [field: SerializeField, Range(0, 100)] public int ChanceUseAllTiles { get; private set; }
        [field: SerializeField, Range(1, 7)] public int MaxTilesPerStep { get; private set; }
        [field: SerializeField] public PointsChances[] Chances { get; private set; }

#if UNITY_EDITOR

        private void OnValidate()
        {
            for (int i = 0; i < Chances.Length; i++)
            {
                var chance = Chances[i];

                if (chance.name.Equals($"Point: {chance.Points}"))
                    continue;

                chance.name = $"Point: {chance.Points} Min: {chance.MinChanceValue} Max: {chance.MaxChanceValue}";
                Chances[i] = chance;
            }
        }
#endif
    }

    [Serializable]
    public struct PointsChances
    {
        [HideInInspector] public string name;
        [field: SerializeField, Range(1, 100)] public int Points { get; private set; }

        [field: Space]
        [field: SerializeField, Range(0, 101)]
        public int MinChanceValue { get; private set; }

        [field: SerializeField, Range(0, 101)] public int MaxChanceValue { get; internal set; }
    }
}