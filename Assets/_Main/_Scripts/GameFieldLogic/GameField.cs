using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class GameField : MonoBehaviour
    {
        [field: SerializeField] private GameFieldSell[] cells;

        private GameFieldSell[,] _grid;
        private DragAndDrop _dragAndDrop;

        private List<string> _words = new()
        {
            "кам", "мак", "скам"
        };

        private List<string> _createdWords = new();

        private void Start()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            _grid = new GameFieldSell [5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _grid[i, j] = cells[i * 5 + j];
                }
            }
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.ClearDeveloperConsole();
                CheckingField();
            }
        }

        private void CheckingField()
        {
            var indexes = FindStartCellsIndexes();
            _createdWords.Clear();

            foreach (var index in indexes)
            {
                // Debug.Log(index);
                CreateAWord(index);
            }

            foreach (var word in _createdWords)
            {
                Debug.Log($"Created word: {word}");
            }
        }

        private List<Vector2> FindStartCellsIndexes()
        {
            List<Vector2> indexes = new List<Vector2>();
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (!_grid[i, j].IsBusy)
                        continue;

                    if (i == 0 && j == 0)
                    {
                        indexes.Add(new Vector2(i, j));
                        continue;
                    }

                    bool isTopEdge = i == 0;
                    bool isLeftEdge = j == 0;
                    bool hasTopNeighbor = !isTopEdge && _grid[i - 1, j].IsBusy;
                    bool hasLeftNeighbor = !isLeftEdge && _grid[i, j - 1].IsBusy;

                    if ((isTopEdge && !hasLeftNeighbor)
                        || (isLeftEdge && !hasTopNeighbor)
                        || (!hasTopNeighbor && !hasLeftNeighbor))
                        indexes.Add(new Vector2(i, j));
                }
            }

            return indexes;
        }

        private void CreateAWord(Vector2 index)
        {
            string word = null;
            word += _grid[(int)index.x, (int)index.y].CurrentTile.Letter;

            if (_grid[(int)index.x + 1, (int)index.y].IsBusy)
            {
                for (int i = (int)index.x + 1; i < _grid.GetLength(0); i++)
                {
                    if (_grid[i, (int)index.y].IsBusy)
                        word += _grid[i, (int)index.y].CurrentTile.Letter;
                    else break;
                }

                _createdWords.Add(word);
            }

            string secondWord = null;
            secondWord += _grid[(int)index.x, (int)index.y].CurrentTile.Letter;

            if (_grid[(int)index.x, (int)index.y + 1].IsBusy)
            {
                for (int i = (int)index.y + 1; i < _grid.GetLength(0); i++)
                {
                    if (_grid[(int)index.x, i].IsBusy)
                        secondWord += _grid[(int)index.x, i].CurrentTile.Letter;
                    else break;
                }

                _createdWords.Add(secondWord);
            }
        }
    }
}