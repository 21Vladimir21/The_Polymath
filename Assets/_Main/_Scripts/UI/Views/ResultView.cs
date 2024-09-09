using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.UI.Views
{
    public class ResultView : AbstractView
    {
        [SerializeField] private TMP_Text PlayerPointsText;
        [SerializeField] private TMP_Text BotPointsText;

        [field: SerializeField] public Button MenuButton { get; private set; }
        [field: SerializeField] public GameObject WinPanel { get; private set; }
        [field: SerializeField] public GameObject LoosePanel { get; private set; }

        public void SetResult(int playerPoints, int botPoints)
        {
            var isWin = playerPoints > botPoints;
            ActivateNeedPanel(isWin);
            PlayerPointsText.text = playerPoints.ToString();
            BotPointsText.text = botPoints.ToString();
        }

        private void ActivateNeedPanel(bool isWin)
        {
            WinPanel.SetActive(isWin);
            LoosePanel.SetActive(!isWin);
        }
    }
}