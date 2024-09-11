using System;
using _Main._Scripts.GameLogic.LettersLogic;
using UnityEngine;

namespace _Main._Scripts.GameLogic.JackPotLogic
{
    public class JackPotTile : LetterTile
    {
        [SerializeField] private JackPotLetterHolder[] holders;
        [SerializeField] private GameObject letterPanel;
        [SerializeField] private GameObject star;

        private const int YPanelPosition = 130;
        private LetterData _defaultData;


        private void Awake()
        {
            _defaultData = letterData;
            foreach (var holder in holders)
                holder.OnSelectLetter += ChangeLetterData;
        }

        public override void SetOnField(Vector2Int coordinates)
        {
            OpenLetterPanel(coordinates.x);
            base.SetOnField(coordinates);
        }

        public override void ResetTile()
        {
            base.ResetTile();
            letterText.text = "*";
            star.SetActive(false);
            pointsText.gameObject.SetActive(false);
            letterData = _defaultData;
        }

        public override void UpTile()
        {
            HideLetterPanel();
            base.UpTile();
        }

        public void SetLetterDataFromLetter(string letter)
        {
            var enumLetter = Enum.Parse<Letters>(letter);
            if (enumLetter == Letters.JACKPOT)
            {
                HideLetterPanel();
                return;
            }
            foreach (var holder in holders)
                if (holder.LetterData.Letter.Equals(enumLetter))
                    ChangeLetterData(holder.LetterData);

            
        }

        private void OpenLetterPanel(int x)
        {
            if (x <= 7)
                letterPanel.transform.localPosition = new Vector3(0, -YPanelPosition, 0);
            else
                letterPanel.transform.localPosition = new Vector3(0, YPanelPosition, 0);

            letterPanel.SetActive(true);
        }

        private void HideLetterPanel()
        {
            letterPanel.SetActive(false);
        }

        private void ChangeLetterData(LetterData data)
        {
            HideLetterPanel();

            letterData = data;
            letterText.text = LetterString;
            pointsText.gameObject.SetActive(true);
            pointsText.text = letterData.Points.ToString();
            star.SetActive(true);
        }
    }
}