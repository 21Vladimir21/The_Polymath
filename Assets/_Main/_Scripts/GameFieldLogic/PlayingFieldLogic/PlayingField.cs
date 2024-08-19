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
            {
                Grid[i, j] = cells[i * MaxGridLength + j];
                Grid[i, j].SetCellCoords(i, j);
            }
        }

        public void MarkTilesAsPartOfRightWord(List<LetterTile> currentWordTiles)
        {
            foreach (var tile in currentWordTiles)
                tile.MarkInRightWord();
        }

        public List<Word> GetWordsOnField()
        {
            var startWordsKeyValuePairs = FindStartCellsIndexes();

            List<Word> words = new();
            foreach (var keyValuePair in startWordsKeyValuePairs)
                words.AddRange(CreateAWords(keyValuePair.Key, keyValuePair.Value));
            return words;
        }

        private List<Word> CreateAWords(Vector2 index, CheckingFieldDirection checkDirection)
        {
            List<Word> createdWords = new();
            Word horizontalWordResult;
            Word verticalWordResult;
            switch (checkDirection)
            {
                case CheckingFieldDirection.Horizontal:
                    horizontalWordResult = BuildWordFromGridCells(true, index);
                    if (horizontalWordResult != null)
                        createdWords.Add(horizontalWordResult);
                    break;

                case CheckingFieldDirection.Vertical:
                    verticalWordResult = BuildWordFromGridCells(false, index);
                    if (verticalWordResult != null)
                        createdWords.Add(verticalWordResult);
                    break;

                case CheckingFieldDirection.All:
                    horizontalWordResult = BuildWordFromGridCells(true, index);
                    if (horizontalWordResult != null)
                        createdWords.Add(horizontalWordResult);
                    verticalWordResult = BuildWordFromGridCells(false, index);
                    if (verticalWordResult != null)
                        createdWords.Add(verticalWordResult);
                    break;
            }

            return createdWords;
        }

        private Word BuildWordFromGridCells(bool checkFromHorizontal, Vector2 startIndex)
        {
            List<LetterTile> currentWordTiles = new();

            int wordMultiplicationValue = 1;

            CalculateTilePoints(startIndex, currentWordTiles, ref wordMultiplicationValue);

            Vector2 index = startIndex;
            int cycleIndex = checkFromHorizontal ? (int)index.y : (int)index.x;

            if (checkFromHorizontal)
                index.y += 1;
            else
                index.x += 1;

            while (cycleIndex < Grid.GetLength(checkFromHorizontal ? 0 : 1) - 1)
            {
                var gameFieldSell = Grid[(int)index.x, (int)index.y];
                if (gameFieldSell.IsBusy)
                    CalculateTilePoints(index, currentWordTiles, ref wordMultiplicationValue);
                else
                    break;

                if (checkFromHorizontal) index.y++;
                else index.x++;

                cycleIndex++;
            }

            return currentWordTiles.Count > 1
                ? new Word(currentWordTiles, checkFromHorizontal, wordMultiplicationValue)
                : null;
        }

        private void CalculateTilePoints(Vector2 startIndex, List<LetterTile> currentWordTiles,
            ref int wordMultiplicationValue)
        {
            var gameFieldSell = Grid[(int)startIndex.x, (int)startIndex.y];
            var currentTile = gameFieldSell.CurrentTile;

            if (currentTile == null) return;
            if (gameFieldSell.MultiplicationBonus > 1 && !gameFieldSell.IsWordMultiplication)
                currentTile.SetMultiplicationValue(gameFieldSell.MultiplicationBonus);
            else
                wordMultiplicationValue *= gameFieldSell.MultiplicationBonus;
            currentWordTiles.Add(currentTile);
        }

        private Dictionary<Vector2Int, CheckingFieldDirection> FindStartCellsIndexes()
        {
            Dictionary<Vector2Int, CheckingFieldDirection> indexes =
                new Dictionary<Vector2Int, CheckingFieldDirection>();
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    if (!Grid[i, j].IsBusy)
                        continue;

                    if (i == 0 && j == 0)
                    {
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.All);
                        continue;
                    }

                    bool isTopEdge = i == 0;
                    bool isLeftEdge = j == 0;
                    bool isRightEdge = j == Grid.GetLength(0) - 1;
                    bool isBottomEdge = i == Grid.GetLength(1) - 1;
                    bool hasTopNeighbor = !isTopEdge && Grid[i - 1, j].IsBusy;
                    bool hasLeftNeighbor = !isLeftEdge && Grid[i, j - 1].IsBusy;
                    bool hasRightNeighbor = !isRightEdge && Grid[i, j + 1].IsBusy;
                    bool hasBottomNeighbor = !isBottomEdge && Grid[i + 1, j].IsBusy;

                    if (isBottomEdge && isRightEdge) continue;

                    if (isTopEdge && !hasLeftNeighbor)
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.All);
                    else if (isLeftEdge && !hasTopNeighbor)
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.All);
                    else if (!hasTopNeighbor && !hasLeftNeighbor)
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.All);
                    else if (!isRightEdge && hasRightNeighbor && hasTopNeighbor && !hasLeftNeighbor)
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.Horizontal);
                    else if (!isBottomEdge && hasLeftNeighbor && hasRightNeighbor && hasBottomNeighbor &&
                             !hasTopNeighbor)
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.Vertical);
                    else if (!isBottomEdge && hasLeftNeighbor && !hasRightNeighbor && hasBottomNeighbor &&
                             !hasTopNeighbor)
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.Vertical);
                }
            }

            return indexes;
        }
    }

    public enum CheckingFieldDirection
    {
        All,
        Horizontal,
        Vertical
    }
}