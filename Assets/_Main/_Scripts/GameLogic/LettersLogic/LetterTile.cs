using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace _Main._Scripts.GameLogic
{
    [RequireComponent(typeof(RectTransform))]
    public class LetterTile : MonoBehaviour
    {
        public Action ReturnToPanel;
        [SerializeField] private LetterData letterData;
        public Letters Letter => letterData.Letter;
        [SerializeField] private TMP_Text letterText;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private RectTransform rectTransform;
        private int _multiplierValue = 1;

        public Vector2Int TileCoordinates { get; private set; }
        public bool InRightWord { get; private set; }
        public bool CanMove { get; private set; } = true;
        public string LetterString =>  letterData.Letter.ToString();
        public int Points => letterData.Points * _multiplierValue;
        public RectTransform RectTransform => rectTransform;

        public void SetOnField(Vector2Int coordinates)
        {
            TileCoordinates = coordinates;
            InRightWord = true;
            CanMove = false;
        }

        public void MarkInRightWord() => InRightWord = true;

        public void ResetTile()
        {
            InRightWord = false;
            CanMove = true;
            _multiplierValue = 1;
        }

        public void SetMultiplicationValue(int value) => _multiplierValue = value;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (letterText == null) letterText = GetComponentInChildren<TMP_Text>();

            letterText.text = LetterString;
            // if (LetterString == Letters.JackPot.ToString()) letterText.text = "*"; 
            pointsText.text = letterData.Points.ToString();

            gameObject.name = "LetterTile " + Letter;

            EditorUtility.SetDirty(this);
        }
#endif
    }
}