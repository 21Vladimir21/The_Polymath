using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class GameFieldSell : MonoBehaviour
    {
        public bool IsBusy { get; private set; }
        [field: SerializeField] public LetterTile CurrentTile { get; private set; }

//TODO:Для тестов (незабыть убратьы)
        private void Start()
        {
            if (CurrentTile)
            {
                ResetTilePosition();
                IsBusy = true;
            }
        }

        public void AddTile(LetterTile tile)
        {
            CurrentTile = tile;
            IsBusy = true;
        }

        public void ClearTileData()
        {
            CurrentTile = null;
            IsBusy = false;
        }

        public void ResetTilePosition() => CurrentTile.transform.position = transform.position;
    }
}