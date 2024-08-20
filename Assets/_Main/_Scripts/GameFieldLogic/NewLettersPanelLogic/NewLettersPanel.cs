using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class NewLettersPanel : MonoBehaviour
    {
        [SerializeField] private List<NewLetterPanelCell> cells;

        public List<NewLetterPanelCell> GetFreeCells()
        {
            List<NewLetterPanelCell> freeCells = new();
            foreach (var cell in cells)
            {
                if (cell.IsBusy) continue;
                freeCells.Add(cell);
            }

            return freeCells;
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

        private void ReturnTileToFreeCell(LetterTile tile)
        {
            var freeCells = GetFreeCells();
            freeCells[0].AddTileAndAllowMove(tile);
        }
    }
}