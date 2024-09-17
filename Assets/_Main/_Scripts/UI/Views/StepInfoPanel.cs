using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _Main._Scripts.UI.Views
{
    public class StepInfoPanel : AbstractView
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private float showDelay;


        public void SetInfoAndShowPanel(string info,Action callback)
        {
            infoText.text = info;
            StartCoroutine(ShowPanel(callback));
        }

        private IEnumerator ShowPanel(Action callback)
        {
            panel.SetActive(true);
            yield return new WaitForSeconds(showDelay);
            panel.SetActive(false);
            callback?.Invoke();
        }
    }
}