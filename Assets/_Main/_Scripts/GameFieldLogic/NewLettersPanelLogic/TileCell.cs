using UnityEngine;

namespace _Main._Scripts.GameFieldLogic
{
    public class TileCell : MonoBehaviour
    {
        public bool IsBusy { get; private set; }
        public LetterTile CurrentTile { get; private set; }

        private Vector2Int _coords;
        public void SetCellCoords(int x,int y) => _coords =  new Vector2Int(x,y);


        public virtual void AddTileAndAllowMove(LetterTile tile)
        {
            AddTile(tile);
            tile.ResetTile();
        }
        public void AddTile(LetterTile tile)
        {
            CurrentTile = tile;
            CurrentTile.SetOnField(_coords);
            IsBusy = true;
            ResetTilePosition();
        }

        public void ClearTileData()
        {
            Debug.Log($"Tile{CurrentTile.LetterString} deleted from cell");
            CurrentTile = null;
            IsBusy = false;
        }

        public void ResetTilePosition() => CurrentTile.transform.position = transform.position;
    }
}