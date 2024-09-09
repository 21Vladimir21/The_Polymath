using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Main._Scripts.UI.Views
{
    public class ValidationWordsView : AbstractView
    {
        [SerializeField] private GameObject PointsPhrase;
        [SerializeField] private GameObject NotFoundWordPhrase;

        [field: SerializeField] public TMP_Text ResulText { get; private set; }
        [field: SerializeField] public Button HideButton { get; private set; }

        public override void Init()
        {
            HideButton.onClick.AddListener(() => Close());
            base.Init();
        }


        public void SetResultText(List<string> notFoundWords, int points, Action callback)
        {
            if (notFoundWords.Count >= 1)
            {
                CreateWordsString(notFoundWords);
                callback.Invoke();
            }
            else if (points > 0)
            {
                ResulText.text = points.ToString();
                ActivateNeedText(true);
                callback.Invoke();
            }
        }

        private void CreateWordsString(List<string> notFoundWords)
        {
            var notWoundWordsStr = "";
            for (var index = 0; index < notFoundWords.Count; index++)
            {
                var word = notFoundWords[index];
                notWoundWordsStr += word;
                if (index >= notFoundWords.Count - 1) break;
                notWoundWordsStr += ", ";
            }

            ResulText.text = notWoundWordsStr;
            ActivateNeedText(false);
        }

        private void ActivateNeedText(bool isPointsText)
        {
            PointsPhrase.SetActive(isPointsText);
            NotFoundWordPhrase.SetActive(!isPointsText);
        }
    }
}