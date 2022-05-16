using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
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


    private static List<AStartNode> GetNeighbors(AStartNode[,] grid, int column, int row)
    {
        var directionVectors = row % 2 == 0 ? _evenRowEvenRDirections : _oddRowEvenRDirections;
        return directionVectors
            .Select(direction => TryGettingNeighbor(grid, column, row,column + direction.Item1, row + direction.Item2, 1))
            .Where(neighborOpt => neighborOpt != null)
            .ToList();
    }

    private static AStartNode TryGettingNeighbor(AStartNode[,] grid, int columnStart, int rowStart, int columnEnd, int rowEnd, int maxLayerDifference)
    {
        if (columnStart < 0 || rowStart < 0 || columnStart >= grid.GetLength(0) || rowStart >= grid.GetLength(1)
            || columnEnd < 0 || rowEnd < 0 || columnEnd >= grid.GetLength(0) || rowEnd >= grid.GetLength(1))
        {
            return null;
        }

        if (Mathf.Sqrt(Mathf.Pow(grid[columnStart, rowStart].layerHeight - grid[columnEnd, rowEnd].layerHeight, 2)) > maxLayerDifference)
        {
            return null;
        }

        return grid[columnEnd, rowEnd];
    }

    public List<AStartNode> FindPath(AStartNode[,] grid, AStartNode startNode, AStartNode targetNode)
    {
        var openSet = new List<AStartNode>();
        var closedSet = new HashSet<AStartNode>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet[0];
            for (var i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost() < currentNode.fCost() ||
                    openSet[i].fCost() == currentNode.fCost() && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (var neighbor in GetNeighbors(grid, currentNode.position.x, currentNode.position.y))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                var newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    List<AStartNode> RetracePath(AStartNode startNode, AStartNode endNode)
    {
        var path = new List<AStartNode>{startNode};
        var currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }

    private int GetDistance(AStartNode nodeA, AStartNode nodeB)
    {
        var dstX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        var dstY = Mathf.Abs(nodeA.position.y - nodeB.position.y);
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * dstX - dstY;
        }

        return 14 * dstX + 10 * dstY - dstX;
    }
}

public class AStartNode
{
    public Vector2Int position;
    public int layerHeight;

    public int gCost;
    public int hCost;
    public AStartNode parent;

    public int fCost()
    {
        return gCost + hCost;
    }
    
}


public class PathfindingUtils
{
    public static AStartNode[,] GetAStartNodeGridFromHexGrid(Hex[,] grid)
    {
        var aGrid = new AStartNode[grid.GetLength(0), grid.GetLength(1)];
        for (var column = 0; column < grid.GetLength(0); column++)
        {
            for (var row = 0; row < grid.GetLength(1); row++)
            {
                var n = grid[column, row];
                aGrid[column, row] = new AStartNode()
                {
                    position = new Vector2Int(column, row),
                    layerHeight = n.GetSerialized().layerHeight
                };
            }
        }

        return aGrid;
    }
}