using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class PlayingField : MonoBehaviour
    {
        [field: SerializeField] private PlayingFieldCell[] cells;
        public PlayingFieldCell[,] Grid { get; private set; }

        private const int MaxGridLength = 15;

        public void InitializeGrid()
        {
            Grid = new PlayingFieldCell [MaxGridLength, MaxGridLength];
            for (int i = 0; i < MaxGridLength; i++)
            for (int j = 0; j < MaxGridLength; j++)
                Grid[i, j] = cells[i * MaxGridLength + j];
        }

        public void MarkTilesAsPartOfRightWord(List<LetterTile> currentWordTiles)
        {
            foreach (var tile in currentWordTiles)
                tile.MarkInRightWord();
        }
    }
}