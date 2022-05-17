using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{
    /**
     * Even R method for indexing
     * Pathfinding via A*
     */
    public class HexGrid : IHexGrid
    {
        private static readonly List<(int, int)> EvenRowEvenRDirections = new()
        {
            (1, 0), (1, -1), (0, -1),
            (-1, 0), (0, 1), (1, 1)
        };

        private static readonly List<(int, int)> OddRowEvenRDirections = new()
        {
            (1, 0), (0, -1), (-1, -1),
            (-1, 0), (-1, 1), (0, 1)
        };

        private IHexTile[,] _grid;

        public HexGrid(int column, int row)
        {
            _grid = GenerateGrid(column, row);
        }

        public List<IHexTile> GetNeighborsFor(int column, int row, int maxLayerDifference)
        {
            return GetNeighbors(_grid, column, row, maxLayerDifference);
        }

        public void ResizeTo(int column, int row, bool preservePreviousGridData)
        {
            var newGrid = GenerateGrid(column, row);
            if (preservePreviousGridData)
            {
                newGrid = CopyGrid(newGrid, _grid);
            }

            _grid = newGrid;
        }

        public IHexTile GetTileAt(int column, int row)
        {
            return column < 0 || row < 0 || column >= _grid.GetLength(0) || row >= _grid.GetLength(1)
                ? null
                : _grid[column, row];
        }

        public List<IHexTile> GetPathFromTo(IHexTile tileA, IHexTile tileB, int maxLayerDifference)
        {
            return new HexPathfinder().GetPath(this, tileA, tileB, maxLayerDifference);
        }

        public IHexTile[,] GetTiles()
        {
            return _grid;
        }

        private static IHexTile[,] GenerateGrid(int column, int row)
        {
            var tiles = new IHexTile[column, row];
            for (var c = 0; c < column; c++)
            {
                for (var r = 0; r < row; r++)
                {
                    tiles[c, r] = new HexTile(new Position(c, r), 1);
                }
            }

            return tiles;
        }

        private static IHexTile[,] CopyGrid(IHexTile[,] newTiles, IHexTile[,] oldTiles)
        {
            var maxColumn = MathF.Min(oldTiles.GetLength(0), newTiles.GetLength(0));
            var maxRow = MathF.Min(oldTiles.GetLength(1), newTiles.GetLength(1));
            for (var c = 0; c < maxColumn; c++)
            {
                for (var r = 0; r < maxRow; r++)
                {
                    newTiles[c, r].SetLayerHeight(oldTiles[c, r].GetLayerHeight());
                }
            }

            return newTiles;
        }

        private static List<IHexTile> GetNeighbors(IHexTile[,] grid, int column, int row, int maxLayerDifference)
        {
            var directionVectors = row % 2 == 0 ? EvenRowEvenRDirections : OddRowEvenRDirections;
            return directionVectors
                .Select(direction => TryGettingNeighbor(grid, column, row, column + direction.Item1,
                    row + direction.Item2, maxLayerDifference))
                .Where(neighborOpt => neighborOpt != null)
                .ToList();
        }

        private static IHexTile TryGettingNeighbor(IHexTile[,] grid, int columnStart, int rowStart, int columnEnd,
            int rowEnd, int maxLayerDifference)
        {
            if (columnStart < 0 || rowStart < 0 || columnStart >= grid.GetLength(0) || rowStart >= grid.GetLength(1)
                || columnEnd < 0 || rowEnd < 0 || columnEnd >= grid.GetLength(0) || rowEnd >= grid.GetLength(1))
            {
                return null;
            }

            return GetLayerHeightDifference(grid[columnStart, columnEnd], grid[rowStart, rowEnd]) > maxLayerDifference
                ? null
                : grid[columnEnd, rowEnd];
        }

        private static float GetLayerHeightDifference(IHexTile tileA, IHexTile tileB)
        {
            return Mathf.Sqrt(
                Mathf.Pow(tileA.GetLayerHeight() - tileB.GetLayerHeight(), 2));
        }
    }
}