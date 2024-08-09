using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class FieldChecker
    {
        private readonly GameFieldCell[,] _grid;

        public FieldChecker(GameFieldCell[,] grid)
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

        public FreeSpaceInfo FindNeedsCellsForOpponent()
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (!_grid[i, j].IsBusy) continue;

                    var freeCells = CheckFreeCellsFromStartCell(i, j);
                    if (freeCells.x > 1)
                    {
                        return new FreeSpaceInfo(new Vector2(i, j), CheckingFieldDirection.Vertical,
                            (int)freeCells.x + 1);
                    }

                    if (freeCells.y > 1)
                    {
                        return new FreeSpaceInfo(new Vector2(i, j), CheckingFieldDirection.Horizontal,
                            (int)freeCells.y + 1);
                    }
                }
            }

            return default;
        }

        public Vector2 CheckFreeCellsFromStartCell(int x, int y)
        {
            var xCycleCount = x;
            var yCycleCount = y;


            bool isTopEdge = x == 0;
            bool isLeftEdge = y == 0;


            if (!isTopEdge && _grid[x - 1, y].IsBusy) return default;
            for (int i = x + 1; i < _grid.GetLength(1); i++)
            {
                isLeftEdge = y == 0;
                bool isRightEdge = y == _grid.GetLength(0) - 1;
                bool isBottomEdge = i == _grid.GetLength(1) - 1;

                if (_grid[i, y].IsBusy) break;
                if (!isBottomEdge && _grid[i, y + 1].IsBusy) break;
                if (!isRightEdge && _grid[i, y + 1].IsBusy) break;
                if (!isLeftEdge && _grid[i, y - 1].IsBusy) break;

                xCycleCount++;
            }

            if (!isLeftEdge && _grid[x, y - 1].IsBusy) return default;
            for (int i = y + 1; i < _grid.GetLength(0); i++)
            {
                isTopEdge = x == 0;
                bool isBottomEdge = x == _grid.GetLength(1) - 1;

                bool isRightEdge = i == _grid.GetLength(0) - 1;

                if (_grid[x, i].IsBusy) break;
                if (!isRightEdge && _grid[x, i + 1].IsBusy) break;
                if (!isBottomEdge && _grid[x + 1, i].IsBusy) break;
                if (!isTopEdge && _grid[x - 1, i].IsBusy) break;

                yCycleCount++;
            }

            Debug.Log(
                $"от буквы {_grid[x, y].CurrentTile.Letter.ToString()} в направлении вних свободно {xCycleCount - x} ячеек , в направлении вправо {yCycleCount - y}");
            return new Vector2(xCycleCount - x, yCycleCount - y);
        }
    }

    public struct FreeSpaceInfo
    {
        public Vector2 Coordinates;
        public CheckingFieldDirection Direction;
        public int Length;

        public FreeSpaceInfo(Vector2 coordinates, CheckingFieldDirection direction, int length)
        {
            Coordinates = coordinates;
            Direction = direction;
            Length = length;
        }
    }

    public enum CheckingFieldDirection
    {
        Horizontal,
        Vertical,
        All
    }
}