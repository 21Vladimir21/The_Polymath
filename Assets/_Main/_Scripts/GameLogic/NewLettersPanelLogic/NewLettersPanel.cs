using System;
using System.Collections.Generic;
using _Main._Scripts.GameLogic.LettersLogic;
using _Main._Scripts.GameLogic.SwapTilesLogic;
using _Main._Scripts.LetterPooLogic;
using _Main._Scripts.Services;
using _Main._Scripts.Services.Saves;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts.GameLogic.NewLettersPanelLogic
{
    public class NewLettersPanel : MonoBehaviour
    {
        [field: SerializeField] public List<NewLetterPanelCell> Cells { get; private set; }

        private LettersPool _lettersPool;
        private Saves _saves;

        public void Initialize(LettersPool lettersPool)
        {
            _lettersPool = lettersPool;
            
            var savesService = ServiceLocator.Instance.GetServiceByType<SavesService>();
            _saves = savesService.Saves;
        }
        
        

        public void SaveLetters() => _saves.SaveLetterInPanel(Cells);

        public void SetLettersFromSave()
        {
            foreach (var letter in _saves.LettersInPanel)
            {
                var tile = _lettersPool.GetTile(letter);
                ReturnTileToFreeCell(tile);
            }
        }
        public void SetNewLettersInPanel()
        {
            var freeCells = GetFreeCells();
            foreach (var cell in freeCells)
            {
                var tile = _lettersPool.GetRandomTile();
                cell.AddTileAndAllowMove(tile,false);
            }
            
            SaveLetters();
        }

        public void ReturnAllTilesIntoCells(List<PlayingFieldCell> cells)
        {
            foreach (var cell in cells)
            {
                ReturnTileToFreeCell(cell.CurrentTile);
                cell.ClearTileData();
            }
        }


        public void ReturnAllTilesToPool()
        {
            foreach (var cell in Cells)
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

            foreach (var cell in Cells)
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

        public List<NewLetterPanelCell> GetFreeCells()
        {
            List<NewLetterPanelCell> freeCells = new();
            foreach (var cell in Cells)
            {
                if (cell.IsBusy) continue;
                freeCells.Add(cell);
            }

            return freeCells;
        }

        public void ReturnTileToFreeCell(LetterTile tile)
        {
            var freeCells = GetFreeCells();
            freeCells[0].AddTileAndAllowMove(tile,false);
        }

        public void UpAndShakeAllTiles()
        {
            foreach (var cell in Cells)
                if (cell.IsBusy)
                    cell.UpAndShakeTile();
        }

        public void DownAndStopShakeAllTiles()
        {
            foreach (var cell in Cells)
                if (cell.IsBusy)
                    cell.DownAndStopShake();
        }
    }
}