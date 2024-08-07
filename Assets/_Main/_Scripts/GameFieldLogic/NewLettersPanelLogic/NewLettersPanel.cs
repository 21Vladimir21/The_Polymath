using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class NewLettersPanel : MonoBehaviour
    {
        [SerializeField] private List<TileCell> cells;

        public List<TileCell> GetFreeCells()
        {
            List<TileCell> freeCells = new();
            foreach (var cell in cells)
            {
                if (cell.IsBusy) continue;
                freeCells.Add(cell);
            }

            return freeCells;
        }
    }
}