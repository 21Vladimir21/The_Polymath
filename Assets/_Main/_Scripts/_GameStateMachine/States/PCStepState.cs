using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main._Scripts._GameStateMachine.States
{
    public class PCStepState : IState
    {
        private readonly IStateSwitcher _stateSwitcher;
        private readonly PlayingField _playingField;
        private readonly SortingDictionary _dictionary;
        private readonly LettersPool _lettersPool;
        private readonly GameData _gameData;

        public PCStepState(IStateSwitcher stateSwitcher,
            PlayingField playingField,
            SortingDictionary dictionary, LettersPool lettersPool, GameData gameData)
        {
            _stateSwitcher = stateSwitcher;
            _playingField = playingField;
            _dictionary = dictionary;
            _lettersPool = lettersPool;
            _gameData = gameData;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                NewMethod();
                _stateSwitcher.SwitchState<PlayerStepState>();
            }
        }

        private void NewMethod()
        {
            var spaces = FindNeedsCellsForOpponent();

            foreach (var space in spaces)
            {
                var firstCell = _playingField.Grid[space.Coordinates.x, space.Coordinates.y];
                if (space.Equals(default(FreeSpaceInfo)))
                    return;

                List<string> words = new();
                for (int i = 0; i < space.Length; i++)
                {
                    int randomWordLength = Random.Range(2, space.Length);
                    words = _dictionary.GetWordsFromFirstLetterAndLength(firstCell.CurrentTile.Letter,
                        randomWordLength);
                    if (words != null)
                        break;
                }
                //TODO:Написал какуе-то хуету, надо переделат 

                if (words == null)
                {
                    Debug.Log($"Word with max length{space.Length} not found");
                    continue;
                }


                var word = words[Random.Range(0, words.Count)];
                var charArray = word.ToList();

                var startIndex = 0;
                if (space.Direction == CheckingFieldDirection.Horizontal)
                    startIndex = space.Coordinates.y + 1;
                else if (space.Direction == CheckingFieldDirection.Vertical)
                    startIndex = space.Coordinates.x + 1;

                for (int i = 1; i < charArray.Count; i++)
                {
                    var letter = charArray[i].ToString().ToUpper();
                    var enumLetter = Enum.Parse<Letters>(letter);
                    var tile = _lettersPool.GetTile(enumLetter);
                    if (space.Direction == CheckingFieldDirection.Horizontal)
                        _playingField.Grid[space.Coordinates.x, startIndex].AddTile(tile);
                    else if (space.Direction == CheckingFieldDirection.Vertical)
                        _playingField.Grid[startIndex, space.Coordinates.y].AddTile(tile);
                    tile.SetOnField();
                    startIndex++;
                }

                return;
            }

            Debug.Log($"Word not found");
            _stateSwitcher.SwitchState<PlayerStepState>();
        }

        private List<FreeSpaceInfo> FindNeedsCellsForOpponent()
        {
            List<FreeSpaceInfo> freeSpaceInfos = new();
            for (int i = 0; i < _playingField.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < _playingField.Grid.GetLength(1); j++)
                {
                    if (!_playingField.Grid[i, j].IsBusy) continue;

                    var freeCells = CheckFreeCellsFromStartCell(i, j);
                    if (freeCells.x > 1)
                        freeSpaceInfos.Add(new FreeSpaceInfo(new Vector2Int(i, j), CheckingFieldDirection.Vertical,
                            freeCells.x + 1));

                    if (freeCells.y > 1)
                        freeSpaceInfos.Add(new FreeSpaceInfo(new Vector2Int(i, j), CheckingFieldDirection.Horizontal,
                            freeCells.y + 1));
                }
            }

            return freeSpaceInfos.Count > 0 ? freeSpaceInfos : null;
        }

        private Vector2Int CheckFreeCellsFromStartCell(int x, int y)
        {
            var xCycleCount = x;
            var yCycleCount = y;

            bool isTopEdge;
            bool isBottomEdge;
            bool isRightEdge;
            bool isLeftEdge;


            for (int i = x; i < _playingField.Grid.GetLength(1); i++)
            {
                isTopEdge = i == 0;
                isLeftEdge = y == 0;
                isBottomEdge = i == _playingField.Grid.GetLength(1) - 1;
                isRightEdge = y == _playingField.Grid.GetLength(0) - 1;

                if (i - 1 != x && !isTopEdge && _playingField.Grid[i - 1, y].IsBusy) break;
                if (!isBottomEdge && _playingField.Grid[i + 1, y].IsBusy) break;
                if (i + 2 <= 13 && _playingField.Grid[i + 2, y].IsBusy) break;
                if (!isBottomEdge && !isRightEdge && _playingField.Grid[i + 1, y + 1].IsBusy) break;
                if (!isBottomEdge && !isLeftEdge && _playingField.Grid[i + 1, y - 1].IsBusy) break;

                xCycleCount++;
            }

            for (int i = y; i < _playingField.Grid.GetLength(0); i++)
            {
                isTopEdge = x == 0;
                isLeftEdge = i == 0;
                isBottomEdge = x == _playingField.Grid.GetLength(1) - 1;
                isRightEdge = i == _playingField.Grid.GetLength(0) - 1;

                if (i - 1 != y && !isLeftEdge && _playingField.Grid[x, i - 1].IsBusy) break;
                if (!isRightEdge && _playingField.Grid[x, i + 1].IsBusy) break;
                if (i + 2 <= 13 && _playingField.Grid[x, i + 2].IsBusy) break;
                if (!isRightEdge && !isBottomEdge && _playingField.Grid[x + 1, i + 1].IsBusy) break;
                if (!isRightEdge && !isTopEdge && _playingField.Grid[x - 1, i + 1].IsBusy) break;


                yCycleCount++;
            }

            Debug.Log(
                $"от буквы {_playingField.Grid[x, y].CurrentTile.Letter.ToString()} в направлении вних свободно {xCycleCount - x} ячеек , в направлении вправо {yCycleCount - y}");
            return new Vector2Int(xCycleCount - x, yCycleCount - y);
        }
    }

    public struct FreeSpaceInfo
    {
        public Vector2Int Coordinates;
        public CheckingFieldDirection Direction;
        public int Length;

        public FreeSpaceInfo(Vector2Int coordinates, CheckingFieldDirection direction, int length)
        {
            Coordinates = coordinates;
            Direction = direction;
            Length = length;
        }
    }
}