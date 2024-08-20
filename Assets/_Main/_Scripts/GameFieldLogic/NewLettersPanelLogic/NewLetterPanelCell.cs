namespace _Main._Scripts.GameFieldLogic
{
    public class NewLetterPanelCell : TileCell
    {
        public LetterTile LastTile { get; private set; }

        public override void AddTileAndAllowMove(LetterTile tile)
        {
            LastTile = tile;
            base.AddTileAndAllowMove(tile);
        }
    }
}