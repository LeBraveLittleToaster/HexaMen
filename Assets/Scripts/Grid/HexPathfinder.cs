using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{
    public class HexPathfinder
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

        public List<IHexTile> GetPath(IHexGrid grid, IHexTile tileA, IHexTile tileB, int maxLayerDifference)
        {
            var nodeA = new AStartNode(tileA.GetPosition(), tileA.GetLayerHeight());
            var nodeB = new AStartNode(tileB.GetPosition(), tileB.GetLayerHeight());
            var path = FindPath(ConvertToAStartNodes(grid.GetTiles()), nodeA, nodeB, maxLayerDifference);
            return GetHexTilesFromAStarNodes(path, grid);
        }

        private List<IHexTile> GetHexTilesFromAStarNodes(List<AStartNode> nodes, IHexGrid grid)
        {
            return nodes.Select(node => grid.GetTileAt(node.position.X, node.position.Y)).ToList();
        }

        private List<AStartNode> FindPath(AStartNode[,] grid, AStartNode startNode,
            AStartNode targetNode, int maxLayerDifference)
        {
            var openSet = new List<AStartNode>();
            var closedSet = new HashSet<AStartNode>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                foreach (var t in openSet)
                {
                    if (t.FCost() < currentNode.FCost() ||
                        t.FCost() == currentNode.FCost() && t.hCost < currentNode.hCost)
                    {
                        currentNode = t;
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (Equals(currentNode, targetNode))
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (var neighbor in GetNeighbors(grid, currentNode.position.X, currentNode.position.Y,
                             maxLayerDifference))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    var newMovementCostToNeighbor = currentNode.gCost + GetMarshalDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor >= neighbor.gCost && openSet.Contains(neighbor)) continue;

                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetMarshalDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private static List<AStartNode> GetNeighbors(AStartNode[,] grid, int column, int row, int maxLayerDifference)
        {
            var directionVectors = row % 2 == 0 ? EvenRowEvenRDirections : OddRowEvenRDirections;
            return directionVectors
                .Select(direction =>
                    TryGettingNeighbor(grid, column, row, column + direction.Item1, row + direction.Item2,
                        maxLayerDifference))
                .Where(neighborOpt => neighborOpt != null)
                .ToList();
        }

        private static AStartNode TryGettingNeighbor(AStartNode[,] grid, int columnStart, int rowStart, int columnEnd,
            int rowEnd, int maxLayerDifference)
        {
            if (columnStart < 0 || rowStart < 0 || columnStart >= grid.GetLength(0) || rowStart >= grid.GetLength(1)
                || columnEnd < 0 || rowEnd < 0 || columnEnd >= grid.GetLength(0) || rowEnd >= grid.GetLength(1))
            {
                return null;
            }

            if (Mathf.Sqrt(Mathf.Pow(grid[columnStart, rowStart].layerHeight - grid[columnEnd, rowEnd].layerHeight,
                    2)) > maxLayerDifference)
            {
                return null;
            }

            return grid[columnEnd, rowEnd];
        }


        List<AStartNode> RetracePath(AStartNode startNode, AStartNode endNode)
        {
            var path = new List<AStartNode> { startNode };
            var currentNode = endNode;
            while (!Equals(currentNode, startNode))
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            return path;
        }

        private static int GetMarshalDistance(AStartNode nodeA, AStartNode nodeB)
        {
            var dstX = Mathf.Abs(nodeA.position.X - nodeB.position.X);
            var dstY = Mathf.Abs(nodeA.position.Y - nodeB.position.Y);
            return Mathf.Max(dstX, dstY);
        }

        private static AStartNode[,] ConvertToAStartNodes(IHexTile[,] grid)
        {
            var columnCount = grid.GetLength(0);
            var rowCount = grid.GetLength(1);
            var aGrid = new AStartNode[columnCount, rowCount];
            for (var column = 0; column < columnCount; column++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    var hexTile = grid[column, row];
                    aGrid[column, row] = new AStartNode(hexTile.GetPosition(), hexTile.GetLayerHeight());
                }
            }

            return aGrid;
        }
    }


    public class AStartNode
    {
        public readonly Position position;
        public readonly int layerHeight;

        public int gCost;
        public int hCost;
        public AStartNode parent;

        public AStartNode(Position position, int layerHeight)
        {
            this.position = position;
            this.layerHeight = layerHeight;
        }

        public int FCost()
        {
            return gCost + hCost;
        }

        protected bool Equals(AStartNode other)
        {
            return Equals(position, other.position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AStartNode)obj);
        }

        public override int GetHashCode()
        {
            return (position != null ? position.GetHashCode() : 0);
        }
    }
}