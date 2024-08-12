using _Main._Scripts.GameFieldLogic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LetterTile : MonoBehaviour
{
    [field: SerializeField] public Letters Letter { get; private set; }
    [SerializeField, Range(1, 10)] private int points;
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private RectTransform rectTransform;
    private int _multiplierValue = 1;

    public bool InRightWord { get; private set; }
    public bool CanMove { get; private set; } = true;
    public string LetterString => Letter.ToString();
    public int Points => points * _multiplierValue;
    public RectTransform RectTransform => rectTransform;

    public void SetOnField()
    {
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
        if (LetterString == Letters.JackPot.ToString()) letterText.text = "*";
        pointsText.text = points.ToString();

        gameObject.name = "LetterTile " + Letter;

        EditorUtility.SetDirty(this);
    }
#endif
}