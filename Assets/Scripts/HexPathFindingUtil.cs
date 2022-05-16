using System.Collections.Generic;
using System.Linq;

public class HexPathFindingUtil
{
    /*
    var evenr_direction_differences = [
    // even rows 
    [[+1,  0], [+1, -1], [ 0, -1], 
    [-1,  0], [ 0, +1], [+1, +1]],
    // odd rows 
    [[+1,  0], [ 0, -1], [-1, -1], 
    [-1,  0], [-1, +1], [ 0, +1]],
    ]

    function evenr_offset_neighbor(hex, direction):
    var parity = hex.row & 1
    var diff = evenr_direction_differences[parity][direction]
        return OffsetCoord(hex.col + diff[0], hex.row + diff[1])
    */

    private static List<(int, int)> _evenRowEvenRDirections = new List<(int, int)>
    {
        (1, 0), (1, -1), (0, -1),
        (-1, 0), (0, 1), (1, 1)
    };

    private static List<(int, int)> _oddRowEvenRDirections = new List<(int, int)>
    {
        (1, 0), (0, -1), (-1, -1),
        (-1, 0), (-1, 1), (0, 1)
    };


    public static List<HexNeighbor> GetNeighbors(Hex[,] grid, int column, int row)
    {
        var directionVectors = row % 2 == 0 ? _evenRowEvenRDirections : _oddRowEvenRDirections;
        return directionVectors
            .Select(direction => TryGettingNeighbor(grid, column + direction.Item1, row + direction.Item2))
            .Where(neighborOpt => neighborOpt != null)
            .ToList();
    }

    private static HexNeighbor TryGettingNeighbor(Hex[,] grid, int column, int row)
    {
        if (column < 0 || row < 0 || column >= grid.GetLength(0) || row >= grid.GetLength(1))
        {
            return null;
        }

        return new HexNeighbor(column, row, grid[column, row]);
    }
}

public class HexNeighbor
{
    private int _column;
    private int _row;
    private Hex _hex;

    public HexNeighbor(int column, int row, Hex hex)
    {
        _column = column;
        _row = row;
        _hex = hex;
    }

    public int Column => _column;
    public int Row => _row;
    public Hex Hex => _hex;
}