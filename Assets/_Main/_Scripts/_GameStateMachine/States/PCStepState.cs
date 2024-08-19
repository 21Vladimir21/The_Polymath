using System;
using System.Collections.Generic;
using System.Linq;
using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.GameFieldLogic;
using _Main._Scripts.LetterPooLogic;
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

        private const int MaxWordLength = 8;

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
                CreateWordFromFirstLetter();
                var words = _playingField.GetWordsOnField();
                _gameData.AddNewWords(words);
                _stateSwitcher.SwitchState<PlayerStepState>();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                foreach (var createdWord in _gameData.CreatedWords)
                {
                    if (CheckFreeCellsCountFromWordEdges(createdWord))
                    {
                        var words = _playingField.GetWordsOnField();
                        _gameData.AddNewWords(words);
                        _stateSwitcher.SwitchState<PlayerStepState>();
                        break;
                    }
                }
            }
        }


        private bool CheckFreeCellsCountFromWordEdges(Word word)
        {
            if (word.IsHorizontal)
            {
                var startCoords = word.GetWordStartCoordinates;
                var endCoords = word.GetWordEndCoordinates;


                var rightFreeSpace = GetFreeHorizontalSpace(endCoords, true);
                var leftFreeSpace = GetFreeHorizontalSpace(startCoords, false);

                return
                    TryPastWord(word, rightFreeSpace, leftFreeSpace, startCoords,true);
            }
            else
            {
                var startCoords = word.GetWordStartCoordinates;
                var endCoords = word.GetWordEndCoordinates;


                var bottomFreeSpace = GetFreeVerticalSpace(endCoords, true);
                var upperFreeSpace = GetFreeVerticalSpace(startCoords, false);

                return
                    TryPastWord(word, bottomFreeSpace, upperFreeSpace, startCoords,false);
            }

            return false;
        }

        private bool TryPastWord(Word word, int rightFreeSpace, int leftFreeSpace, Vector2Int startCoords,
            bool isHorizontal = true)
        {
            var words = _dictionary.GetWordsFromWordPart(word.StringWord);

            foreach (var worde in words)
            {
                Debug.Log(worde.Word);

                if (CheckFreeCellsForWord(word, worde, leftFreeSpace, rightFreeSpace))
                {
                    var startLetterIndex =
                        worde.Word.IndexOf(word.StringWord[0], StringComparison.OrdinalIgnoreCase);

                    var checkingWord = worde.Word;


                    foreach (var letter in word.StringWord)
                    {
                        if (!worde.Word.Contains(letter, StringComparison.OrdinalIgnoreCase)) continue;
                        var index = checkingWord.IndexOf(letter, StringComparison.OrdinalIgnoreCase);
                        checkingWord = checkingWord.Remove(index, 1).Insert(index, "-");
                    }

                    for (int i = 0; i < checkingWord.Length; i++)
                    {
                        if (checkingWord[i].Equals('-')) continue;

                        var newIndex = (isHorizontal ? startCoords.y : startCoords.x) + (i - startLetterIndex);
                        var tile = _lettersPool.GetTileFromChar(checkingWord[i]);
                        if (isHorizontal)
                            _playingField.Grid[startCoords.x, newIndex].AddTile(tile);
                        else
                            _playingField.Grid[newIndex, startCoords.y].AddTile(tile);
                    }

                    return true;
                }
            }

            return false;
        }


        private bool CheckFreeCellsForWord(Word startWord, DictionaryWord dictionaryWord, int leftFreeSpace,
            int rightFreeSpace)
        {
            var checkingWord = dictionaryWord.Word;
            foreach (var letter in startWord.StringWord)
            {
                if (!dictionaryWord.Word.Contains(letter, StringComparison.OrdinalIgnoreCase)) continue;
                var index = checkingWord.IndexOf(letter, StringComparison.OrdinalIgnoreCase);
                checkingWord = checkingWord.Remove(index, 1).Insert(index, "-");
            }


            var startLetterIndex =
                checkingWord.IndexOf('-');
            var endLetterIndex =
                checkingWord.LastIndexOf('-');


            int lettersToLeft = 0;
            int lettersToRight = 0;


            for (var index = 0; index < dictionaryWord.Word.Length; index++)
            {
                if (index < startLetterIndex)
                    lettersToLeft++;
                if (index > endLetterIndex)
                    lettersToRight++;
            }

            if (lettersToLeft > leftFreeSpace || lettersToRight > rightFreeSpace)
                return false;

            return true;
        }

        #region WordEndgesFreeSpaceCheck

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

        #endregion


        private void CreateWordFromFirstLetter()
        {
            var spaces = TryGetStartCells();

            if (spaces == null)
            {
                Debug.Log("Нужных ячеек нет!");
                return;
            }

            foreach (var space in
                     spaces) // TODO: Добавить рандомный выьор первой точки , что бы бот не ставил в предсказуемое место 
            {
                if (space.Equals(default(FreeSpaceInfo)))
                    return;

                var firstCell = _playingField.Grid[space.Coordinates.x, space.Coordinates.y];

                List<LetterTile> foundedTiles = new();
                bool impossibleCreateWord = false;

                for (int i = 0; i < space.Length; i++)
                {
                    int randomWordLength = Random.Range(2, space.Length > MaxWordLength ? MaxWordLength : space.Length);
                    var words = _dictionary.GetWordsFromFirstLetterAndLength(firstCell.CurrentTile.Letter,
                        randomWordLength);
                    if (words == null)
                    {
                        impossibleCreateWord = true;
                        continue;
                    }

                    foreach (var word in words)
                    {
                        if (TilesFounded(word, foundedTiles))
                        {
                            impossibleCreateWord = false;
                            break;
                        }

                        impossibleCreateWord = true;
                    }


                    break;
                }

                if (impossibleCreateWord)
                {
                    foreach (var tile in foundedTiles) _lettersPool.ReturnTile(tile);
                    Debug.Log($"impossible create word");
                    continue;
                }

                SetFoundTilesOnGrid(space, foundedTiles);
                return;
            }

            Debug.Log($"Word not found");
            _stateSwitcher.SwitchState<PlayerStepState>();
        }


        private void SetFoundTilesOnGrid(FreeSpaceInfo space, List<LetterTile> foundedTiles)
        {
            var startIndex = space.Direction == CheckingFieldDirection.Horizontal
                ? space.Coordinates.y + 1
                : space.Coordinates.x + 1;

            foreach (var tile in foundedTiles)
            {
                if (space.Direction == CheckingFieldDirection.Horizontal)
                    _playingField.Grid[space.Coordinates.x, startIndex].AddTile(tile);
                else if (space.Direction == CheckingFieldDirection.Vertical)
                    _playingField.Grid[startIndex, space.Coordinates.y].AddTile(tile);


                startIndex++;
            }
        }

        private bool TilesFounded(string word, List<LetterTile> foundedTiles)
        {
            var charArray = word.ToList();
            for (int j = 1; j < charArray.Count; j++)
            {
                var letter = charArray[j];
                var tile = _lettersPool.GetTileFromChar(letter);
                if (tile)
                    foundedTiles.Add(tile);
                else return false;
            }

            return true;
        }


        private List<FreeSpaceInfo> TryGetStartCells()
        {
            List<FreeSpaceInfo> freeSpaceInfos = new();
            foreach (var createdWord in _gameData.CreatedWords)
            {
                foreach (var tile in createdWord.Tiles)
                {
                    var freeCells = CheckFreeCellsFromStartCell(tile.TileCoordinates);
                    if (freeCells.x > 1)
                        freeSpaceInfos.Add(new FreeSpaceInfo(tile.TileCoordinates, CheckingFieldDirection.Vertical,
                            freeCells.x + 1));

                    if (freeCells.y > 1)
                        freeSpaceInfos.Add(new FreeSpaceInfo(tile.TileCoordinates, CheckingFieldDirection.Horizontal,
                            freeCells.y + 1));
                }
            }

            return freeSpaceInfos.Count > 0 ? freeSpaceInfos : null;
        }

        private Vector2Int CheckFreeCellsFromStartCell(Vector2Int coords)
        {
            var freeSpaceY = 0;
            var freeSpaceX = 0;
            if (LeftCellIsBusy(coords) == false)
                freeSpaceY = GetFreeHorizontalSpace(coords, true);

            if (UpperCellIsBusy(coords) == false)
                freeSpaceX = GetFreeVerticalSpace(coords, true);

            Debug.Log(
                $"от буквы {_playingField.Grid[coords.x, coords.y].CurrentTile.Letter.ToString()} в направлении вних свободно {freeSpaceX} ячеек , в направлении вправо {freeSpaceY}");
            return new Vector2Int(freeSpaceX, freeSpaceY);
        }

        private bool UpperCellIsBusy(Vector2Int coords)
        {
            var isTopEdge = coords.x == 0;
            return !isTopEdge && _playingField.Grid[coords.x - 1, coords.y].IsBusy;
        }

        private bool LeftCellIsBusy(Vector2Int coords)
        {
            var isLeftEdge = coords.y == 0;
            return !isLeftEdge && _playingField.Grid[coords.x, coords.y - 1].IsBusy;
        }
    }
}