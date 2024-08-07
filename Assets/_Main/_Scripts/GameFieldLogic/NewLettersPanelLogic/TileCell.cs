using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class TileCell : MonoBehaviour
    {
        public bool IsBusy { get; private set; }
        public LetterTile CurrentTile { get; private set; }

        public void AddTile(LetterTile tile)
        {
            CurrentTile = tile;
            IsBusy = true;
            ResetTilePosition();
        }

        public void ClearTileData()
        {
            CurrentTile = null;
            IsBusy = false;
        }

        public void ResetTilePosition() => CurrentTile.transform.position = transform.position;
    }
}