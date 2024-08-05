using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LetterTile : MonoBehaviour
{
    [field: SerializeField] public char Letter { get; private set; }
    [SerializeField] private TMP_Text text;
    [SerializeField] private RectTransform rectTransform;

    public RectTransform RectTransform => rectTransform;

    private void Start() => text.text = Letter.ToString();

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (text == null) text = GetComponentInChildren<TMP_Text>();
        EditorUtility.SetDirty(this);
    }

#endif
}