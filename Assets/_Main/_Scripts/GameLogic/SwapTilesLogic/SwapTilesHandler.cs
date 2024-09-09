using System;
using System.Collections.Generic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;

namespace _Main._Scripts.GameLogic.SwapTilesLogic
{
    public class SwapTilesHandler
    {
        public Action OnSwapped;

        private List<TileCell> _cells;
        private GameObject _panel;

        private LettersPool _lettersPool;
        private NewLettersPanel _newLettersPanel;


        public SwapTilesHandler(LettersPool lettersPool,
            NewLettersPanel newLettersPanel, SwapTilesPanelView swapTilesPanelView)
        {
            _cells = swapTilesPanelView.Cells;
            _panel = swapTilesPanelView.Panel;
            _lettersPool = lettersPool;
            _newLettersPanel = newLettersPanel;

            swapTilesPanelView.AllTilesButton.onClick.AddListener(SelectAllCells);
            swapTilesPanelView.OpenPanelButton.onClick.AddListener(OpenPanel);
            swapTilesPanelView.CancelButton.onClick.AddListener(CancelSwap);
            swapTilesPanelView.ApplyButton.onClick.AddListener(SwapSelectedTiles);
            swapTilesPanelView.RewardApplyButton.onClick.AddListener(RewardSwapSelectedTiles);
            swapTilesPanelView.HidePanelButton.onClick.AddListener(HidePanel);
        }


        private void OpenPanel() => _panel.SetActive(true);

        private void HidePanel()
        {
            CancelSwap();
            _panel.SetActive(false);
        }

        private void SwapSelectedTiles()
        {
            RewardSwapSelectedTiles();
            OnSwapped?.Invoke();
            Debug.Log("Default swap!");
        }

        private void RewardSwapSelectedTiles()
        {
            ReturnTilesInPool();
            _newLettersPanel.SetNewLettersInPanel();
            HidePanel();
        }

        private void SelectAllCells()
        {
            var freeCells = GetFreeCells();
            foreach (var cell in freeCells)
            foreach (var newLetterPanelCell in _newLettersPanel.Cells)
            {
                if (!newLetterPanelCell.IsBusy) continue;
                cell.AddTileAndAllowMove(newLetterPanelCell.CurrentTile);
                newLetterPanelCell.ClearTileData();
                break;
            }
        }

        private void CancelSwap()
        {
            var cells = _newLettersPanel.GetFreeCells();
            foreach (var cell in cells)
            {
                foreach (var panelCell in _cells)
                {
                    if (!panelCell.IsBusy) continue;
                    cell.AddTileAndAllowMove(panelCell.CurrentTile);
                    panelCell.ClearTileData();
                    break;
                }
            }
        }

        private void ReturnTilesInPool()
        {
            foreach (var cell in _cells)
                if (cell.IsBusy)
                {
                    _lettersPool.ReturnTile(cell.CurrentTile);
                    cell.ClearTileData();
                }
        }

        private List<TileCell> GetFreeCells()
        {
            List<TileCell> freeCells = new();
            foreach (var cell in _cells)
            {
                if (cell.IsBusy) continue;
                freeCells.Add(cell);
            }

            return freeCells;
        }
    }
}