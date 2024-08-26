using System;
using System.Collections.Generic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.GameLogic.NewLettersPanelLogic
{
    public class NewLettersPanel : MonoBehaviour
    {
        [SerializeField] private List<NewLetterPanelCell> cells;

        private LettersPool _lettersPool;

        public void Initialize(LettersPool lettersPool)
        {
            _lettersPool = lettersPool;
        }

        public void SetNewLettersInPanel()
        {
            var freeCells = GetFreeCells();
            var randomLetters = CreateRandomLettersList(freeCells.Count);
            for (int i = 0; i < freeCells.Count; i++)
            {
                var tile = _lettersPool.GetTile(randomLetters[i]);
                freeCells[i].AddTileAndAllowMove(tile);
            }
        }

        public void ReturnAllTilesIntoCells()
        {
            foreach (var cell in cells)
            {
                if (cell.IsBusy)
                    continue;
                ReturnTileToFreeCell(cell.LastTile);
            }
        }

        public void ReturnNotRightTiles()
        {
            foreach (var cell in cells)
            {
                if (cell.IsBusy || cell.LastTile.InRightWord)
                    continue;
                ReturnTileToFreeCell(cell.LastTile);
            }
        }

        private List<NewLetterPanelCell> GetFreeCells()
        {
            List<NewLetterPanelCell> freeCells = new();
            foreach (var cell in cells)
            {
                if (cell.IsBusy) continue;
                freeCells.Add(cell);
            }

            return freeCells;
        }

        private void ReturnTileToFreeCell(LetterTile tile)
        {
            var freeCells = GetFreeCells();
            freeCells[0].AddTileAndAllowMove(tile);
        }

        private List<Letters> CreateRandomLettersList(int count)
        {
            List<Letters> randomLettersArray = new();
            var letters = Enum.GetValues(typeof(Letters));
            for (int i = 0; i < count; i++)
            {
                var randomIndex = Random.Range(0, letters.Length);
                var randomLetter = (Letters)letters.GetValue(randomIndex);
                randomLettersArray.Add(randomLetter);
            }

            return randomLettersArray;
        }
    }
}