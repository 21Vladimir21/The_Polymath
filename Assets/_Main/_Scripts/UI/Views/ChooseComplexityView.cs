using _Main._Scripts.GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.UI.Views
{
    public class ChooseComplexityView : AbstractView
    {
        [field:SerializeField] public ComplexityButton[] ComplexityButtons { get; private set; }
        [field:SerializeField] public Button StartGameButton { get; private set; }
        [field:SerializeField] public Button RulesButton { get; private set; }
        
    }
}
