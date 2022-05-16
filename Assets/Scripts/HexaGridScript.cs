using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class HexaGridScript : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Hex hexaPrefab;

    [SerializeField] private int rowCount;
    [SerializeField] private int columnCount;
    [SerializeField] private float hexSize = 2;

    private Hex[,] _grid;
    private List<HexNeighbor> currentHightlightedNeighbors = new List<HexNeighbor>();

    private List<AStartNode> path = new();  
    
    public void TestPathfinding()
    {
        ClearOldPath(path);
        var aGrid = PathfindingUtils.GetAStartNodeGridFromHexGrid(_grid);
        var pathfinding = new Pathfinding();
        path = pathfinding.FindPath(aGrid, aGrid[0,0], aGrid[8,12]);
        HightLightPath(path);
    }

    private void HightLightPath(List<AStartNode> nodes)
    {
        if (nodes == null) return;
        foreach (var node in nodes)
        {
            _grid[node.position.x, node.position.y].HightLightHex(true);
        }
    }
    
    private void ClearOldPath(List<AStartNode> nodes)
    {
        if (nodes == null) return;
        foreach (var node in nodes)
        {
            _grid[node.position.x, node.position.y].HightLightHex(false);
        }
    }
    
    private void Start()
    {
        try
        {
            var savedGameBoard = File.ReadAllText("gameboard.json");
            var savedGrid = JsonConvert.DeserializeObject<SerializedGrid>(savedGameBoard);
            _grid = GenerateFromSavedGrid(savedGrid);
        }
        catch (Exception e)
        {
            _grid = GenerateGrid(columnCount, rowCount, hexSize);
        }

        UpdateCameraTarget();
    }

    public void ResizeGrid(int column, int row)
    {
        var oldGrid = _grid;
        _grid = ExtendToGrid(column, row, _grid);
        DeleteGrid(oldGrid);
        UpdateCameraTarget();
    }
    private Hex[,] ExtendToGrid(int rowCount, int columnCount, Hex[,] oldGrid)
    {
        var newGrid = GenerateGrid(columnCount, rowCount, hexSize);
        for (var column = 0; column < newGrid.GetLength(0); column++)
        {
            for (var row = 0; row < newGrid.GetLength(1); row++)
            {
                if (column >= oldGrid.GetLength(0) || row >= oldGrid.GetLength(1)) continue;
                var newHex = newGrid[column, row].GetComponent<Hex>();
                var oldHex = oldGrid[column, row].GetComponent<Hex>();
                newHex.ReInitiate(column, row, oldHex.GetSerialized().layerHeight,
                    GetEntityTypeFromSerializedType(oldHex.GetSerialized().hexType));
            }
        }

        return newGrid;
    }
    private static void DeleteGrid(Hex[,] oldGrid)
    {
        for (var columnI = 0; columnI < oldGrid.GetLength(0); columnI++)
        {
            for (var rowI = 0; rowI < oldGrid.GetLength(1); rowI++)
            {
                Destroy(oldGrid[columnI, rowI].gameObject);
            }
        }
    }
    
    private void UpdateCameraTarget()
    {
        cameraTarget.position = new Vector3(_grid.GetLength(0) * hexSize / 2, cameraTarget.position.y,
            _grid.GetLength(1) * hexSize / 2);
    }

    private Hex[,] GenerateFromSavedGrid(SerializedGrid savedGrid)
    {
        var grid = GenerateGrid(savedGrid.grid.GetLength(0), savedGrid.grid.GetLength(1), hexSize);
        for (var columnI = 0; columnI < savedGrid.grid.GetLength(0); columnI++)
        {
            for (var rowI = 0; rowI < savedGrid.grid.GetLength(1); rowI++)
            {
                grid[columnI, rowI].GetComponent<Hex>().ReInitiate(columnI, rowI, savedGrid.grid[columnI, rowI].layerHeight,
                    GetEntityTypeFromSerializedType(savedGrid.grid[columnI, rowI].hexType));
            }
        }
        return grid;
    }

    private static EntityType GetEntityTypeFromSerializedType(SerializedHexType hexType)
    {
        return hexType switch
        {
            SerializedHexType.NONE => EntityType.NONE,
            SerializedHexType.PLAYER => EntityType.PLAYER,
            SerializedHexType.ENEMY => EntityType.ENEMY,
            _ => throw new ArgumentOutOfRangeException(nameof(hexType), hexType, null)
        };
    }

    

    private Hex[,] GenerateGrid(int columnCount, int rowCount, float hexSize)
    {
        var hexGrid = new Hex[columnCount, rowCount];
        var hexWidth = 2 * hexSize;
        var hexHeight = Mathf.Sqrt(3) * hexSize;

        for (var columnI = 0; columnI < columnCount; columnI++)
        {
            for (var rowI = 0; rowI < rowCount; rowI++)
            {
                hexGrid[columnI, rowI] =
                    Instantiate(hexaPrefab, GetPositionFromIndexes(columnI, rowI, hexWidth, hexHeight),
                        Quaternion.identity).Initiate(columnI, rowI);
            }
        }

        return hexGrid;
    }

    

    /**
     * Indexes as "even-r"
     */
    private static Vector3 GetPositionFromIndexes(int columnI, int rowI, float hexWidth, float hexHeight)
    {
        return rowI % 2 == 0
            ? new Vector3(columnI * hexWidth + hexWidth / 2, 0, rowI * hexHeight)
            : new Vector3(columnI * hexWidth, 0, rowI * hexHeight);
    }

    public void SaveGameBoardToFile()
    {
        var grid = new SerializedHex[_grid.GetLength(0), _grid.GetLength(1)];
        for (var columnI = 0; columnI < _grid.GetLength(0); columnI++)
        {
            for (var rowI = 0; rowI < _grid.GetLength(1); rowI++)
            {
                grid[columnI, rowI] = _grid[columnI, rowI].GetComponent<Hex>().GetSerialized();
            }
        }

        File.WriteAllText(
            "gameboard.json",
            JsonConvert.SerializeObject(new SerializedGrid {grid = grid})
        );
    }

    public void TestHexClicked()
    {
        OnHexClicked(3,3);
    }
    
    private void OnHexClicked(int column, int row)
    {
        var hexNeighbors = HexPathFindingUtil.GetNeighbors(_grid, column, row);
        foreach (var neighbor in currentHightlightedNeighbors)
        {
            neighbor.Hex.HightLightHex(false);
        }

        currentHightlightedNeighbors = hexNeighbors;
        
        foreach (var neighbor in currentHightlightedNeighbors)
        {
            neighbor.Hex.HightLightHex(true);
        }
    }
   
}