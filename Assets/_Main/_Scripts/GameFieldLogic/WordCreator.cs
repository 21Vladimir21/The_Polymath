using System.Collections.Generic;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class WordCreator
    {
        private readonly GameFieldCell[,] _grid;

        public WordCreator(GameFieldCell[,] grid)
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

            int wordMultiplicationValue = 1;

            CheckingGridTile(startIndex, currentWordTiles, ref wordMultiplicationValue);

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
                    CheckingGridTile(index, currentWordTiles, ref wordMultiplicationValue);
                else
                    break;

                if (checkFromHorizontal) index.y++;
                else index.x++;

                cycleIndex++;
            }

            return currentWordTiles.Count > 1 ? new Word(currentWordTiles, wordMultiplicationValue) : null;
        }

        private void CheckingGridTile(Vector2 startIndex, List<LetterTile> currentWordTiles,
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