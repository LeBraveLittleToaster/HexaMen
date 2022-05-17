using System.Collections.Generic;

namespace Grid
{
    public interface IHexGrid
    {
        void ResizeTo(int column, int row, bool preservePreviousGridData);
        IHexTile GetTileAt(int column, int row);
        List<IHexTile> GetPathFromTo(IHexTile tileA, IHexTile tileB, int maxLayerDifference);

        IHexTile[,] GetTiles();
    }
}