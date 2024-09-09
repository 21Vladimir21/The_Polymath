using System;
using System.Collections;
using _Main._Scripts.GameLogic.SwapTilesLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main._Scripts.UI.Views
{
    public class InGameView : AbstractView
    {
        [SerializeField] private TMP_Text playerPointsText;
        [SerializeField] private TMP_Text botPointsText;
        [SerializeField] private GameObject playerStepPanel;
        [SerializeField] private GameObject botStepPanel;

        [field: SerializeField] public Button CheckWordsButton { get; private set; }
        [field: SerializeField] public Button EndStepButton { get; private set; }
        [field: SerializeField] public Button ReturnLettersToPanelButton { get; private set; }
        [field: SerializeField] public Button MixTilesButton { get; private set; }
        [field: SerializeField] public SwapTilesPanelView SwapTilesPanelView { get; private set; }

        [Header("Settings")] [SerializeField] private int showStepPanelsDuration = 1;


        public void UpdatePoints(int playerPoints, int botPoints)
        {
            playerPointsText.text = playerPoints + "\nочков";
            botPointsText.text = botPoints + "\nочков";
        }

        public void ShowPlayerPanel(Action callback = null) => StartCoroutine(ShowPanelRoutine(playerStepPanel,callback));
        public void ShowBotPanel(Action callback = null) => StartCoroutine(ShowPanelRoutine(botStepPanel,callback));


        private IEnumerator ShowPanelRoutine(GameObject panel,Action callBack)
        {
            panel.gameObject.SetActive(true);
            yield return new WaitForSeconds(showStepPanelsDuration);
            panel.gameObject.SetActive(false);
            callBack?.Invoke();
        }
    }
}