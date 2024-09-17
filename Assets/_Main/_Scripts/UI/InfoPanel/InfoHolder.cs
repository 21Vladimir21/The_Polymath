using System;
using UnityEngine;

namespace _Main._Scripts.UI.InfoPanel
{
    [Serializable]
    public class InfoHolder
    {
        [field: SerializeField] public InfoKey Key { get; private set; }
        [field: SerializeField] public string InfoStr { get; private set; }
    }
}