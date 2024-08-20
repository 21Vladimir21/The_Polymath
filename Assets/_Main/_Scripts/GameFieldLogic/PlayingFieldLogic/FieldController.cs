using _Main._Scripts.DictionaryLogic;
using _Main._Scripts.GameDatas;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class FieldController
    {
        private readonly PlayingField _playingField;
        private readonly FieldFreeSpaceHandler _fieldFreeSpaceHandler;
        private readonly WordCreateHandler _wordCreateHandler;
        private readonly SortingDictionary _dictionary;
        private readonly LettersPool _lettersPool;
        private readonly GameData _gameData;

        public FieldController(PlayingField playingField, GameData gameData, SortingDictionary dictionary,
            LettersPool lettersPool)
        {
            _playingField = playingField;
            _gameData = gameData;
            _dictionary = dictionary;
            _lettersPool = lettersPool;
            playingField.InitializeGrid();
            _fieldFreeSpaceHandler = new FieldFreeSpaceHandler(_playingField, _gameData);
            _wordCreateHandler = new WordCreateHandler(_dictionary, _playingField, _lettersPool);
        }


        public bool CheckAndPlaceWord()
        {
            var freeSpaceInfos =
                _fieldFreeSpaceHandler.TryGetWordFreeSpaceInfo();
            if (freeSpaceInfos == null)
            {
                Debug.Log("Не нашлось нужных слов для составления нового слова!");
                return false;
            }

            foreach (var info in freeSpaceInfos)
            {
                if (!_wordCreateHandler.CanPlaceWord(info)) continue;
                UpdateFieldWords();
                return true;
            }

            return false;
        }

        public bool
            CheckAndPlaceWordFromLetter() //TODO:Наверное можно сделать generic класс и скоратить код до 1 метода 
        {
            var letterFreeSpaceInfos = _fieldFreeSpaceHandler.TryGetStartCells();

            if (letterFreeSpaceInfos == null)
            {
                Debug.Log("Для составления слова нет буквы на доске!");
                return false;
            }

            foreach (var space in letterFreeSpaceInfos)
                // TODO: Добавить рандомный выьор первой точки , что бы бот не ставил в предсказуемое место 
            {
                if (!_wordCreateHandler.CanPlaceWord(space)) continue;
                UpdateFieldWords();
                return true;
            }

            return false;
        }

        public void ClearNotRightTiles()
        {
            for (int i = 0; i < _playingField.Grid.GetLength(0); i++)
            for (int j = 0; j < _playingField.Grid.GetLength(1); j++)
                if (_playingField.Grid[i, j].CurrentTile != null && _playingField.Grid[i, j].CurrentTile.InRightWord == false)
                    _playingField.Grid[i, j].ClearTileData();
        }
        public void ClearMovableTiles()
        {
            for (int i = 0; i < _playingField.Grid.GetLength(0); i++)
            for (int j = 0; j < _playingField.Grid.GetLength(1); j++)
                if (_playingField.Grid[i, j].CurrentTile != null && _playingField.Grid[i, j].CurrentTile.CanMove)
                    _playingField.Grid[i, j].ClearTileData();
        }

        public void UpdateFieldWords()
        {
            var words = _playingField.GetWordsOnField();
            _gameData.AddNewWords(words);
        }
    }
}