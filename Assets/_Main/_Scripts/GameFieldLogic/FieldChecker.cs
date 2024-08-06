using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class FieldChecker
    {
        private readonly GameFieldSell[,] _grid;
        
        private List<string> _words = new() //TODO: вынести от сюда нахуй 
        {
            "кам", "мак", "скам", "хуй", "смак",
        };

        public FieldChecker(GameFieldSell[,] grid)
        {
            _grid = grid;
        }

        public Dictionary<Vector2, CheckingFieldDirection> FindStartCellsIndexes() //TODO: в FC
        {
            Dictionary<Vector2, CheckingFieldDirection> indexes = new Dictionary<Vector2, CheckingFieldDirection>();
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (!_grid[i, j].IsBusy)
                        continue;

                    if (i == 0 && j == 0)
                    {
                        indexes.Add(new Vector2(i, j), CheckingFieldDirection.All);
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
                        indexes.Add(new Vector2(i, j), CheckingFieldDirection.All);
                    else if (isLeftEdge && !hasTopNeighbor)
                        indexes.Add(new Vector2(i, j), CheckingFieldDirection.All);
                    else if (!hasTopNeighbor && !hasLeftNeighbor)
                        indexes.Add(new Vector2(i, j), CheckingFieldDirection.All);
                    else if (!isRightEdge && hasRightNeighbor && hasTopNeighbor && !hasLeftNeighbor)
                        indexes.Add(new Vector2(i, j), CheckingFieldDirection.Horizontal);
                    else if (!isBottomEdge && hasLeftNeighbor && hasRightNeighbor && hasBottomNeighbor &&
                             !hasTopNeighbor)
                        indexes.Add(new Vector2(i, j), CheckingFieldDirection.Vertical);
                    else if (!isBottomEdge && hasLeftNeighbor && !hasRightNeighbor && hasBottomNeighbor &&
                             !hasTopNeighbor)
                        indexes.Add(new Vector2(i, j), CheckingFieldDirection.Vertical);
                }
            }

            return indexes;
        }
    }
    public enum CheckingFieldDirection
    {
        Horizontal,
        Vertical,
        All
    }
}