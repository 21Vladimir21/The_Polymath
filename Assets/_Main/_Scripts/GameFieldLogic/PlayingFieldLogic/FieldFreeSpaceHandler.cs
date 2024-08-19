using System.Collections.Generic;
using _Main._Scripts._GameStateMachine.States;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class FieldFreeSpaceHandler
    {
        private readonly PlayingField _playingField;
        private GameDatas _gameData;

        public FieldFreeSpaceHandler(PlayingField playingField)
        {
            _playingField = playingField;
        }


        public WordFreeCellsInfo CheckFreeCellsCountFromWordEdges(Word word)
        {
            if (word.IsHorizontal)
            {
                var startCoords = word.GetWordStartCoordinates;
                var endCoords = word.GetWordEndCoordinates;


                var rightFreeSpace = GetFreeHorizontalSpace(endCoords, true);
                var leftFreeSpace = GetFreeHorizontalSpace(startCoords, false);

                return
                    new WordFreeCellsInfo(word, rightFreeSpace, leftFreeSpace);
            }
            else
            {
                var startCoords = word.GetWordStartCoordinates;
                var endCoords = word.GetWordEndCoordinates;


                var bottomFreeSpace = GetFreeVerticalSpace(endCoords, true);
                var upperFreeSpace = GetFreeVerticalSpace(startCoords, false);

                return
                    new WordFreeCellsInfo(word, upperFreeSpace, bottomFreeSpace);
            }

            return default;
        }


        private int GetFreeHorizontalSpace(Vector2Int coords, bool rightDirection)
        {
            var direction = rightDirection ? 1 : -1;
            int start = coords.y + direction;
            var freeSpaceCount = 0;

            while (start >= 0 && start < _playingField.Grid.GetLength(0))
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

            while (start >= 0 && start < _playingField.Grid.GetLength(0))
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
            var isBottomEdge = x == _playingField.Grid.GetLength(1) - 1;
            if (_playingField.Grid[x, y].IsBusy)
            {
                freeSpaceCount--;
                return true;
            }

            if (!isBottomEdge && _playingField.Grid[x + 1, y].IsBusy) return true;
            if (!isTopEdge && _playingField.Grid[x - 1, y].IsBusy) return true;
            freeSpaceCount++;
            return false;
        }

        private bool CheckLeftRightTilesOnBusy(int x, int y, ref int cycleValue)
        {
            var isLeftEdge = y == 0;
            var isRightEdge = y == _playingField.Grid.GetLength(0) - 1;

            if (_playingField.Grid[x, y].IsBusy)
            {
                cycleValue--;
                return true;
            }

            if (!isRightEdge && _playingField.Grid[x, y + 1].IsBusy) return true;
            if (!isLeftEdge && _playingField.Grid[x, y - 1].IsBusy) return true;
            cycleValue++;
            return false;
        }


        private List<LetterFreeSpaceInfo> TryGetStartCells()
        {
            List<LetterFreeSpaceInfo> freeSpaceInfos = new();
            foreach (var createdWord in _gameData.CreatedWords)
            {
                foreach (var tile in createdWord.Tiles)
                {
                    var freeCells = CheckFreeCellsFromLetter(tile.TileCoordinates);
                    if (freeCells.x > 1)
                        freeSpaceInfos.Add(new LetterFreeSpaceInfo(tile.TileCoordinates,
                            freeCells.x + 1));

                    if (freeCells.y > 1)
                        freeSpaceInfos.Add(new LetterFreeSpaceInfo(tile.TileCoordinates,
                            freeCells.y + 1));
                }
            }

            return freeSpaceInfos.Count > 0 ? freeSpaceInfos : null;
        }

        private LetterFreeSpaceInfo CheckFreeCellsFromLetter(LetterTile tile)
        {
            var coords = tile.TileCoordinates;
            var freeRightSpace = 0;
            var freeLeftSpace = 0;
            var freeUpperSpace = 0;
            var freeBottomSpace = 0;

            freeRightSpace = GetFreeHorizontalSpace(coords, true);
            freeLeftSpace = GetFreeHorizontalSpace(coords, false);


            freeUpperSpace = GetFreeVerticalSpace(coords, false);
            freeBottomSpace = GetFreeVerticalSpace(coords, true);

            if ((freeUpperSpace <= 0 || freeBottomSpace <= 0) && (freeLeftSpace > 0 || freeRightSpace > 0))
                return new LetterFreeSpaceInfo(tile, freeLeftSpace, freeRightSpace,true);
            if else(){}

                Debug.Log(
                    $"от буквы {_playingField.Grid[coords.x, coords.y].CurrentTile.Letter.ToString()} в направлении вних свободно {freeSpaceX} ячеек , в направлении вправо {}");
            return new Vector2Int(freeSpaceX,);
        }
    }
}