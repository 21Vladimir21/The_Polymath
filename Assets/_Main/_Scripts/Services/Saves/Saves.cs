using System;
using System.Collections.Generic;
using System.IO;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.JackPotLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;
using UnityEngine;

namespace _Main._Scripts.Services.Saves
{
    [Serializable]
    public class Saves
    {
        [field: SerializeField] public bool AdEnabled { get; set; } = true;
        [field: SerializeField] public int PlayerPoints { get; set; }
        [field: SerializeField] public int BotPoints { get; set; }
        [field: SerializeField] public bool HasStartGame { get; set; }
        [field: SerializeField] public List<GridCell> CellsData { get; private set; } = new();
        [field: SerializeField] public List<string> LettersInPanel { get; private set; } = new();


        private string _filePath;

        #region Game

        public void SaveGrid(PlayingFieldCell[,] grid)
        {
            CellsData.Clear();
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var index = new Vector2Int(i, j);
                    if (!grid[index.x, index.y].IsBusy) continue;

                    var tile = grid[index.x, index.y].CurrentTile;
                    CellsData.Add(new GridCell(index, tile.LetterString, tile.InRightWord, tile.CanMove,
                        (tile is JackPotTile)));
                }
            }

            InvokeSave();
        }

        public void SaveLetterInPanel(List<NewLetterPanelCell> cells)
        {
            LettersInPanel.Clear();
            foreach (var cell in cells)
            {
                if (!cell.IsBusy) continue;
                LettersInPanel.Add(cell.CurrentTile.LetterString);
            }

            InvokeSave();
        }

        #endregion

        #region Save

        public void LoadSaves()
        {
        }

        public void InvokeSave(bool forcedToDo = false)
        {
#if !UNITY_EDITOR
            SetCloudSaveData();
#endif
#if UNITY_EDITOR
            SetLocalSaveData();
#endif
        }

        private void SetLocalSaveData()
        {
            _filePath = GetFilePath();

            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(_filePath, json);
        }

        private void SetCloudSaveData()
        {
            // var json = JsonUtility.ToJson(this, true);
            // Cloud.SetValue(SaveKey, json, true, () => Debug.Log("Save success \n" + json),
            //     (log) => Debug.Log("Save error \n" + log));
        }

        public static string SaveKey => "saves";

        public static string GetFilePath()
        {
            return Path.Combine(Application.dataPath, "save.json");
        }

        #endregion

        #region Purchases.NoAd.BuyAll

        public void BuyNoAd()
        {
            AdEnabled = false;
            InvokeSave();
        }

        #endregion
    }

    [Serializable]
    public struct GridCell
    {
        public Vector2Int Coords;
        public string Letter;
        public bool InRightWord;
        public bool CanMove;
        public bool IsJackPotTile;

        public GridCell(Vector2Int coords, string letter, bool inRightWord, bool canMove, bool isJackPotTile)
        {
            Coords = coords;
            Letter = letter;
            InRightWord = inRightWord;
            CanMove = canMove;
            IsJackPotTile = isJackPotTile;
        }
    }
}