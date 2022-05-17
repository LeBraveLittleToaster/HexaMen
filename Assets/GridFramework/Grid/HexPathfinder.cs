using System;
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
            var path = FindPath(ConvertToAStarNodes(grid.GetTiles()), tileA.GetPosition(), tileB.GetPosition() , maxLayerDifference);
            return GetHexTilesFromAStarNodes(path, grid);
        }

        private List<IHexTile> GetHexTilesFromAStarNodes(List<AStarNode> nodes, IHexGrid grid)
        {
            return nodes.Select(node => grid.GetTileAt(node.position.X, node.position.Y)).ToList();
        }

        private List<AStarNode> GetNodesWithParent(AStarNode[,] grid, AStarNode startNode, AStarNode targetNode)
        {
            var l = new List<AStarNode>();
            for(var c = 0; c < grid.GetLength(0); c++)
            {
                for(var r = 0; r < grid.GetLength(1); r++)
                {
                    var node = grid[c, r];
                    if (node.parent != null)
                    {
                        l.Add(node);
                    }
                }
            }

            return l;
        }
        
        private List<AStarNode> FindPath(AStarNode[,] grid, Position startPos,
            Position targetPos, int maxLayerDifference)
        {
            var startNode = grid[startPos.X, startPos.Y];
            var targetNode = grid[targetPos.X, targetPos.Y];
            
            var openSet = new List<AStarNode>();
            var closedSet = new HashSet<AStarNode>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (var i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost() < currentNode.FCost() ||
                        openSet[i].FCost() == currentNode.FCost() && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    /*
                    var nodesWithParent = GetNodesWithParent(grid, startNode , targetNode);
                    foreach (var node in nodesWithParent)
                    {
                        Debug.Log(node.position + " | " + (node.parent == null ? " <>" : node.parent.position));
                    }
                    */
                    return RetracePath(startNode, targetNode);
                }

                var neighbors = GetNeighbors(grid, currentNode.position.X, currentNode.position.Y,
                    maxLayerDifference);
                foreach (var neighbor in neighbors)
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

        private static List<AStarNode> GetNeighbors(AStarNode[,] grid, int column, int row, int maxLayerDifference)
        {
            var directionVectors = row % 2 == 0 ? EvenRowEvenRDirections : OddRowEvenRDirections;
            return directionVectors
                .Select(direction =>
                    TryGettingNeighbor(grid, column, row, column + direction.Item1, row + direction.Item2,
                        maxLayerDifference))
                .Where(neighborOpt => neighborOpt != null)
                .ToList();
        }

        private static AStarNode TryGettingNeighbor(AStarNode[,] grid, int columnStart, int rowStart, int columnEnd,
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


        List<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode)
        {
            var path = new List<AStarNode> { startNode };
            var currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();

            return path;
        }

        private static int GetMarshalDistance(AStarNode nodeA, AStarNode nodeB)
        {
            var dstX = Mathf.Abs(nodeA.position.X - nodeB.position.X);
            var dstY = Mathf.Abs(nodeA.position.Y - nodeB.position.Y);
            return Mathf.Max(dstX, dstY);
        }

        private static AStarNode[,] ConvertToAStarNodes(IHexTile[,] grid)
        {
            var columnCount = grid.GetLength(0);
            var rowCount = grid.GetLength(1);
            var aGrid = new AStarNode[columnCount, rowCount];
            for (var column = 0; column < columnCount; column++)
            {
                for (var row = 0; row < rowCount; row++)
                {
                    var hexTile = grid[column, row];
                    aGrid[column, row] = new AStarNode(hexTile.GetPosition(), hexTile.GetLayerHeight());
                }
            }

            return aGrid;
        }
    }


    public class AStarNode
    {
        public Position position;
        public int layerHeight;

        public int gCost;
        public int hCost;
        public AStarNode parent;

        public AStarNode(Position position, int layerHeight)
        {
            this.position = position;
            this.layerHeight = layerHeight;
        }

        public int FCost()
        {
            return gCost + hCost;
        }
    }
}