using _Main._Scripts.GameLogic.LettersLogic;
using UnityEngine;

namespace _Main._Scripts.GameLogic.NewLettersPanelLogic
{
    public class TileCell : MonoBehaviour
    {
        public bool IsBusy { get; private set; }
        public LetterTile CurrentTile { get; private set; }

        protected Vector2Int Coords;
        public void SetCellCoords(int x,int y) => Coords =  new Vector2Int(x,y);


        public virtual void AddTileAndAllowMove(LetterTile tile)
        {
            AddTile(tile);
            tile.ResetTile();
        }
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

        public virtual void ResetTilePosition() => CurrentTile.transform.position = transform.position;
    }
}