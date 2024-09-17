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
        [field: SerializeField] public Button CheckWordsButton { get; private set; }
        [field: SerializeField] public Button EndStepButton { get; private set; }
        [field: SerializeField] public Button ReturnLettersToPanelButton { get; private set; }
        [field: SerializeField] public Button MixTilesButton { get; private set; }
        [field: SerializeField] public SwapTilesPanelView SwapTilesPanelView { get; private set; }
        [field: SerializeField] public MenuPanel MenuPanel { get; private set; }


        public void UpdatePoints(int playerPoints, int botPoints)
        {
            playerPointsText.text = playerPoints + "\nочков";
            botPointsText.text = botPoints + "\nочков";
        }


        public void SetInteractableButtons(bool interactable)
        {
            CheckWordsButton.interactable = interactable;
            EndStepButton.interactable = interactable;
            ReturnLettersToPanelButton.interactable = interactable;
            SwapTilesPanelView.OpenPanelButton.interactable = interactable;
        }
    }
}