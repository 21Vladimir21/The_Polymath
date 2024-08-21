using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic
{
    public class WordSearchHandler
    {
        private readonly PlayingFieldCell[,] _grid;

        public WordSearchHandler(PlayingFieldCell[,] grid)
        {
            _grid = grid;
        }

        public Dictionary<Vector2Int, CheckingFieldDirection> FindStartCellsIndexes()
        {
            Dictionary<Vector2Int, CheckingFieldDirection> indexes =
                new Dictionary<Vector2Int, CheckingFieldDirection>();
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (!_grid[i, j].IsBusy)
                        continue;

                    if (i == 0 && j == 0)
                    {
                        indexes.Add(new Vector2Int(i, j), CheckingFieldDirection.All);
                        continue;
                    }

                    bool isTopEdge = i == 0;
                    bool isLeftEdge = j == 0;
                    bool isRightEdge = j == _grid.GetLength(0) - 1;
                    bool isBottomEdge = i == _grid.GetLength(1) - 1;
                    bool hasTopNeighbor = !isTopEdge && _grid[i - 1, j].IsBusy;
                    bool hasLeftNeighbor = !isLeftEdge && _grid[i, j - 1].IsBusy;
                    bool hasRightNeighbor = !isRightEdge && _grid[i, j + 1].IsBusy;
                    bool hasBottomNeighbor = !isBottomEdge && _grid[i + 1, j].IsBusy;

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

        public List<Word> CreateAWords(Vector2 index, CheckingFieldDirection checkDirection)
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

            while (cycleIndex < _grid.GetLength(checkFromHorizontal ? 0 : 1) - 1)
            {
                var gameFieldSell = _grid[(int)index.x, (int)index.y];
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
            var gameFieldSell = _grid[(int)startIndex.x, (int)startIndex.y];
            var currentTile = gameFieldSell.CurrentTile;

            if (currentTile == null) return;
            if (gameFieldSell.MultiplicationBonus > 1 && !gameFieldSell.IsWordMultiplication)
                currentTile.SetMultiplicationValue(gameFieldSell.MultiplicationBonus);
            else
                wordMultiplicationValue *= gameFieldSell.MultiplicationBonus;
            currentWordTiles.Add(currentTile);
        }
    }
}