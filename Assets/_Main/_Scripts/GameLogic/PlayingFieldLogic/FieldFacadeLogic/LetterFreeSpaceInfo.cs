using System;
using _Main._Scripts.GameLogic;
using _Main._Scripts.GameLogic.LettersLogic;
using UnityEngine;

namespace _Main._Scripts._GameStateMachine.States
{
    public struct LetterFreeSpaceInfo
    {
        public readonly LetterTile LetterTile;
        public readonly int FreeCellsFromBeginningLetter;
        public readonly int FreeCellsFromEndLetter;
        public bool IsHorizontal;
        public LetterFreeSpaceInfo(LetterTile letterTile, int freeCellsFromBeginningLetter, int freeCellsFromEndLetter,bool isHorizontal)
        {
            LetterTile = letterTile;
            FreeCellsFromBeginningLetter = freeCellsFromBeginningLetter;
            FreeCellsFromEndLetter = freeCellsFromEndLetter;
            IsHorizontal = isHorizontal;
        }
    }

    public struct WordFreeCellsInfo
    {

        public readonly Word Word;
        public readonly int FreeCellsFromBeginningWord;
        public readonly int FreeCellsFromEndWord;
        public Vector2Int StartWordCoords => Word.GetWordStartCoordinates;

        public WordFreeCellsInfo(Word word, int freeCellsFromBeginningWord, int freeCellsFromEndWord)
        {
            Word = word;
            FreeCellsFromBeginningWord = freeCellsFromBeginningWord;
            FreeCellsFromEndWord = freeCellsFromEndWord;
        }
    }
    
    
}