using _Main._Scripts.GameLogic.LettersLogic;
using _Main._Scripts.GameLogic.NewLettersPanelLogic;

namespace _Main._Scripts.GameLogic.SwapTilesLogic
{
    public class SwapCell : TileCell
    {
        public override void AddTileAndAllowMove(LetterTile tile, bool isField = true)
        {
            base.AddTileAndAllowMove(tile, false);
        }
    }
}