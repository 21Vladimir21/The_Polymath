using System.Collections.Generic;
using _Main._Scripts.UI.Views;
using UnityEngine;

namespace _Main._Scripts.UI
{
    public class UIViewsHolder : MonoBehaviour
    {
        [field: SerializeField] public List<AbstractView> Views { get; private set; }
    }
}