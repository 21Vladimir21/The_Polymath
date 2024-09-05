using _Main._Scripts.GameLogic.LettersLogic;
using UnityEngine;

namespace _Main._Scripts.GameLogic.JackPotLogic
{
    public class JackPotTile : LetterTile
    {
        [SerializeField] private JackPotLetterHolder[] holders;
        [SerializeField] private GameObject letterPanel;
        private const int YPanelPosition = 130;


        private void Awake()
        {
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
            pointsText.gameObject.SetActive(false);
            letterData = null;
        }

        public override void UpTile()
        {
            HideLetterPanel();
            base.UpTile();
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
        }
    }
}