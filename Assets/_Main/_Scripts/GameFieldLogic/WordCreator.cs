using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class WordCreator
    {
        private readonly GameFieldSell[,] _grid;

        public WordCreator(GameFieldSell[,] grid)
        {
            _grid = grid;
        }

        public List<Word> CreateAWords(Vector2 index, CheckingFieldDirection checkDirection) //TODO: в WC
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

        private Word BuildWordFromGridCells(bool checkFromHorizontal, Vector2 startIndex) //TODO: в WC
        {
            List<LetterTile> currentWordTiles = new();
            var currentTile = _grid[(int)startIndex.x, (int)startIndex.y].CurrentTile;

            if (currentTile != null) currentWordTiles.Add(currentTile);

            Vector2 index = startIndex;
            int cycleIndex = checkFromHorizontal ? (int)index.y : (int)index.x;

            if (checkFromHorizontal)
                index.y += 1;
            else
                index.x += 1;

            while (cycleIndex < _grid.GetLength(checkFromHorizontal ? 0 : 1) - 1)
            {
                if (_grid[(int)index.x, (int)index.y]?.IsBusy == true)
                {
                    currentTile = _grid[(int)index.x, (int)index.y].CurrentTile;
                    if (currentTile != null) currentWordTiles.Add(currentTile);
                }
                else
                    break;

                if (checkFromHorizontal)
                    index.y++;
                else
                    index.x++;

                cycleIndex++;
            }

            return currentWordTiles.Count > 1 ? new Word(currentWordTiles) : null;
        }
    }
}