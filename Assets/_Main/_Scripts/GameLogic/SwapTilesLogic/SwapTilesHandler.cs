using System;
using System.Collections.Generic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic;
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
        private readonly FieldFacade _fieldFacade;


        public SwapTilesHandler(LettersPool lettersPool,
            NewLettersPanel newLettersPanel, FieldFacade fieldFacade,SwapTilesPanelView swapTilesPanelView)
        {
            _cells = swapTilesPanelView.Cells;
            _panel = swapTilesPanelView.Panel;
            _lettersPool = lettersPool;
            _newLettersPanel = newLettersPanel;
            _fieldFacade = fieldFacade;

            swapTilesPanelView.AllTilesButton.onClick.AddListener(SelectAllCells);
            swapTilesPanelView.OpenPanelButton.onClick.AddListener(OpenPanel);
            swapTilesPanelView.CancelButton.onClick.AddListener(CancelSwap);
            swapTilesPanelView.ApplyButton.onClick.AddListener(SwapSelectedTiles);
            swapTilesPanelView.RewardApplyButton.onClick.AddListener(RewardSwapSelectedTiles);
            swapTilesPanelView.HidePanelButton.onClick.AddListener(HidePanel);
        }


        private void OpenPanel()
        {
            _newLettersPanel.ReturnAllTilesIntoCells(_fieldFacade.GetCellsFromMovableTiles());
            _newLettersPanel.UpAndShakeAllTiles();
            _fieldFacade.ClearMovableTiles();
            _panel.SetActive(true);
        }

        private void HidePanel()
        {
            CancelSwap();
            _panel.SetActive(false);
            _newLettersPanel.DownAndStopShakeAllTiles();
        }

        private void SwapSelectedTiles()
        {
            SwapTiles();
            OnSwapped?.Invoke();
        }

        private void RewardSwapSelectedTiles()
        {
            SwapTiles();
        }

        private void SwapTiles()
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
                cell.AddTileAndAllowMove(newLetterPanelCell.CurrentTile,false);
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
                    cell.AddTileAndAllowMove(panelCell.CurrentTile,false);
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