using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Main._Scripts.GameLogic.LettersLogic
{
    [RequireComponent(typeof(RectTransform))]
    public class LetterTile : MonoBehaviour
    {
        [SerializeField] protected LetterData letterData;
        [SerializeField] protected TMP_Text letterText;
        [SerializeField] protected TMP_Text pointsText;
        [SerializeField] private RectTransform rectTransform;
        private int _multiplierValue = 1;
        public Letters Letter => letterData.Letter;
        public Vector2Int TileCoordinates { get; private set; }
        public bool InRightWord { get; private set; }
        public bool CanMove { get; private set; } = true;
        public string LetterString => letterData.Letter.ToString();
        public int Points => letterData.Points * _multiplierValue;
        public RectTransform RectTransform => rectTransform;
        public UnityEvent<LetterTile,LetterTile> OnSwapped;

        public void SetInWord()
        {
            InRightWord = true;
            CanMove = false;
        }

        public virtual void SetOnField(Vector2Int coordinates) => TileCoordinates = coordinates;
        public void MarkInRightWord() => InRightWord = true;

        public virtual void UpTile()
        {
            //Какая-то логика при подъеме, возможно визуал
        }

        public virtual void ResetTile()
        {
            InRightWord = false;
            CanMove = true;
            _multiplierValue = 1;
        }

        public void SetMultiplicationValue(int value) => _multiplierValue = value;

#if UNITY_EDITOR
        // private void OnValidate()
        // {
        //     if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        //     if (letterText == null) letterText = GetComponentInChildren<TMP_Text>();
        //
        //     if (letterData == false) return;
        //     letterText.text = LetterString;
        //     if (LetterString == Letters.JackPot.ToString()) letterText.text = "*";
        //     pointsText.text = letterData.Points.ToString();
        //
        //     gameObject.name = "LetterTile " + Letter;
        //
        //     EditorUtility.SetDirty(this);
        // }
#endif
    }
}