using System;
using _Main._Scripts.BotLogic;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.GameLogic
{
    [RequireComponent(typeof(Button))]
    public class ComplexityButton : MonoBehaviour
    {
        [field: SerializeField] public BotComplexity Complexity { get; private set; }
        [field: SerializeField] public Button Button { get; private set; }
        public Action<BotComplexity> OnChooseComplexity;

        public void Init() => Button.onClick.AddListener(ChooseComplexity);

        private void ChooseComplexity()
        {
            OnChooseComplexity?.Invoke(Complexity);
            Button.interactable = false;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Button == null) Button = GetComponent<Button>();
        }
#endif
    }
}