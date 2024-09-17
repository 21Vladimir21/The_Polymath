using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.UI.InfoPanel
{
    [CreateAssetMenu(fileName = "InfoHolder", menuName = "InfoPanel/Create new info holder", order = 0)]
    public class InfosHolder : ScriptableObject
    {
        [field: SerializeField] public List<InfoHolder> Holders { get; private set; }
    }
}