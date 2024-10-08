using System.Collections.Generic;
using _Main._Scripts._GameStateMachine.States;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameLogic.LettersLogic;
using UnityEngine;

namespace _Main._Scripts.GameLogic.PlayingFieldLogic.FieldFacadeLogic
{
    public class FieldFreeSpaceHandler
    {
        private readonly PlayingFieldCell[,] _grid;
        private readonly PlayingField _playingField;
        private CurrentGameData _currentGameData;

        public FieldFreeSpaceHandler(PlayingFieldCell[,] grid,PlayingField playingField, CurrentGameData currentGameData)
        {
            _grid = grid;
            _playingField = playingField;
            _currentGameData = currentGameData;
        }


        public List<WordFreeCellsInfo> TryGetWordFreeSpaceInfo()
        {
            List<WordFreeCellsInfo> infos = new();
            foreach (var word in _currentGameData.CreatedWords)
            {
                if (word.IsHorizontal)
                {
                    var startCoords = word.GetWordStartCoordinates;
                    var endCoords = word.GetWordEndCoordinates;


                    var leftFreeSpace = GetFreeHorizontalSpace(startCoords, false);
                    var rightFreeSpace = GetFreeHorizontalSpace(endCoords, true);

                    infos.Add(new WordFreeCellsInfo(word, leftFreeSpace, rightFreeSpace));
                }
                else
                {
                    var startCoords = word.GetWordStartCoordinates;
                    var endCoords = word.GetWordEndCoordinates;


                    var upperFreeSpace = GetFreeVerticalSpace(startCoords, false);
                    var bottomFreeSpace = GetFreeVerticalSpace(endCoords, true);

                    infos.Add(new WordFreeCellsInfo(word, upperFreeSpace, bottomFreeSpace));
                }
            }

            return infos.Count <= 0 ? null : infos;
        }


        private int GetFreeHorizontalSpace(Vector2Int coords, bool rightDirection)
        {
            var direction = rightDirection ? 1 : -1;
            int start = coords.y + direction;
            var freeSpaceCount = 0;

            while (start >= 0 && start < _grid.GetLength(0))
            {
                if (CheckUpDownTilesOnBusy(coords.x, start, ref freeSpaceCount))
                    break;

                start += direction;
            }

            return freeSpaceCount;
        }

        private int GetFreeVerticalSpace(Vector2Int coords, bool bottomDirection)
        {
            var direction = bottomDirection ? 1 : -1;
            int start = coords.x + direction;
            var freeSpaceCount = 0;

            while (start >= 0 && start < _grid.GetLength(0))
            {
                if (CheckLeftRightTilesOnBusy(start, coords.y, ref freeSpaceCount))
                    break;

                start += direction;
            }

            return freeSpaceCount;
        }


        private bool CheckUpDownTilesOnBusy(int x, int y, ref int freeSpaceCount)
        {
            var isTopEdge = x == 0;
            var isBottomEdge = x == _grid.GetLength(1) - 1;
            if (_grid[x, y].IsBusy)
            {
                freeSpaceCount--;
                return true;
            }

            if (!isBottomEdge && _grid[x + 1, y].IsBusy) return true;
            if (!isTopEdge && _grid[x - 1, y].IsBusy) return true;
            freeSpaceCount++;
            return false;
        }

        private bool CheckLeftRightTilesOnBusy(int x, int y, ref int cycleValue)
        {
            var isLeftEdge = y == 0;
            var isRightEdge = y == _grid.GetLength(0) - 1;

            if (_grid[x, y].IsBusy)
            {
                cycleValue--;
                return true;
            }

            if (!isRightEdge && _grid[x, y + 1].IsBusy) return true;
            if (!isLeftEdge && _grid[x, y - 1].IsBusy) return true;
            cycleValue++;
            return false;
        }


        public List<LetterFreeSpaceInfo> TryGetStartCells()
        {
            List<LetterFreeSpaceInfo> freeSpaceInfos = new();
            foreach (var createdWord in _currentGameData.CreatedWords)
            {
                foreach (var tile in createdWord.Tiles)
                {
                    var info = CheckFreeCellsFromLetter(tile);
                    if (info.Equals(default(LetterFreeSpaceInfo))) continue;
                    freeSpaceInfos.Add(info);
                }
            }

            return freeSpaceInfos.Count > 0 ? freeSpaceInfos : null;
        }

        private LetterFreeSpaceInfo CheckFreeCellsFromLetter(LetterTile tile)
        {
            var coords = tile.TileCoordinates;

            var freeRightSpace = GetFreeHorizontalSpace(coords, true);
            var freeLeftSpace = GetFreeHorizontalSpace(coords, false);

            var freeUpperSpace = GetFreeVerticalSpace(coords, false);
            var freeBottomSpace = GetFreeVerticalSpace(coords, true);

            if (freeBottomSpace == 0 && freeUpperSpace == 0 && freeRightSpace == 0 && freeLeftSpace == 0)
                return default;

            int horizontalSpace = freeLeftSpace >= freeRightSpace ? freeLeftSpace : freeRightSpace;
            int verticalSpace = freeBottomSpace >= freeUpperSpace ? freeBottomSpace : freeUpperSpace;

            return horizontalSpace > verticalSpace
                ? new LetterFreeSpaceInfo(tile, freeLeftSpace, freeRightSpace, true)
                : new LetterFreeSpaceInfo(tile, freeUpperSpace, freeBottomSpace, false);
        }
    }
}