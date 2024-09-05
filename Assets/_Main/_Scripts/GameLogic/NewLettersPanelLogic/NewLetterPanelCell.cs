using _Main._Scripts.GameLogic.LettersLogic;

namespace _Main._Scripts.GameLogic.NewLettersPanelLogic
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