using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.GameLogic.JackPotLogic
{
    public class JackPotLetterHolder : MonoBehaviour
    {
        [SerializeField] private LetterData letterData;
        [SerializeField] private Button button;

        public Action<LetterData> OnSelectLetter;
        public LetterData LetterData => letterData;

        private void Awake() => button.onClick.AddListener(SelectLetter);

        private void SelectLetter()
        {
            if (letterData != null) OnSelectLetter?.Invoke(letterData);
            else Debug.LogWarning($"Letter data not found on {gameObject.name}");
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (button != false) return;
            button = GetComponent<Button>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}