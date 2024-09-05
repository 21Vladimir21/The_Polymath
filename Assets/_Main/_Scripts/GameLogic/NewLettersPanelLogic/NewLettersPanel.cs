using System;
using System.Collections.Generic;
using _Main._Scripts.GameLogic.LettersLogic;
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

        public void SwapTiles()
        {
            foreach (var cell in cells)
                if (cell.IsBusy)
                {
                    _lettersPool.ReturnTile(cell.CurrentTile);
                    cell.ClearTileData();
                }
            
            SetNewLettersInPanel();
        }

        public void SetNewLettersInPanel()
        {
            var freeCells = GetFreeCells();
            foreach (var cell in freeCells)
            {
                var tile = _lettersPool.GetRandomTile();
                cell.AddTileAndAllowMove(tile);
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

        public void ReturnNotRightTilesToPanel()
        {
            foreach (var cell in cells)
            {
                if (cell.IsBusy || cell.LastTile.InRightWord)
                    continue;
                ReturnTileToFreeCell(cell.LastTile);
            }
        }

        public void ReturnAllTilesToPool()
        {
            foreach (var cell in cells)
            {
                if (!cell.IsBusy) continue;

                var tile = cell.CurrentTile;
                tile.ResetTile();
                _lettersPool.ReturnTile(tile);
                cell.ClearTileData();
            }
        }

        public void MixTheTiles()
        {
            List<LetterTile> tiles = new();

            foreach (var cell in cells)
                if (cell.IsBusy)
                {
                    tiles.Add(cell.CurrentTile);
                    cell.ClearTileData();
                }

            while (tiles.Count > 0)
            {
                var randomIndex = Random.Range(0, tiles.Count);
                ReturnTileToFreeCell(tiles[randomIndex]);
                tiles.RemoveAt(randomIndex);
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

        public void ReturnTileToFreeCell(LetterTile tile)
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