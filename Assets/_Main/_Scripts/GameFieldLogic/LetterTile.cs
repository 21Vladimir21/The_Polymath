using _Main._Scripts.GameFieldLogic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LetterTile : MonoBehaviour
{
    [SerializeField] private Letters letter;
    [field: SerializeField, Range(1, 10)] public int Points { get; private set; }
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private RectTransform rectTransform;

    public bool InRightWord { get; private set; }
    public bool CanMove { get; private set; } = true;
    public string Letter => letter.ToString();
    public RectTransform RectTransform => rectTransform;

    public void SetOnField()
    {
        InRightWord = true;
        CanMove = false;
    }

    public void SetInRightWord() => InRightWord = true;

    public void ResetTile()
    {
        InRightWord = false;
        CanMove = true;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (letterText == null) letterText = GetComponentInChildren<TMP_Text>();

        letterText.text = Letter;
        if (Letter == Letters.JackPot.ToString()) letterText.text = "*";

        gameObject.name = "LetterTile " + Letter;

        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Set prefab name")]
    private void SetName()
    {
    }

#endif
}