using System;
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


    public void ResizeGrid(int column, int row)
    {
        var oldGrid = _grid;
        _grid = ExtendToGrid(column, row, _grid);
        DeleteGrid(oldGrid);
        UpdateCameraTarget();
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

    private Hex[,] GenerateFromSavedGrid(SerializedGrid savedGrid)
    {
        var grid = GenerateGrid(savedGrid.grid.GetLength(0), savedGrid.grid.GetLength(1), hexSize);
        for (var columnI = 0; columnI < savedGrid.grid.GetLength(0); columnI++)
        {
            for (var rowI = 0; rowI < savedGrid.grid.GetLength(1); rowI++)
            {
                grid[columnI, rowI].GetComponent<Hex>().ReInitiate(savedGrid.grid[columnI, rowI].layerHeight,
                    GetEntityTypeFromSerializedType(savedGrid.grid[columnI, rowI].hexType));
            }
        }
        return grid;
    }

    private EntityType GetEntityTypeFromSerializedType(SerializedHexType hexType)
    {
        return hexType switch
        {
            SerializedHexType.NONE => EntityType.NONE,
            SerializedHexType.PLAYER => EntityType.PLAYER,
            SerializedHexType.ENEMY => EntityType.ENEMY,
            _ => throw new ArgumentOutOfRangeException(nameof(hexType), hexType, null)
        };
    }

    private void UpdateCameraTarget()
    {
        cameraTarget.position = new Vector3(_grid.GetLength(0) * hexSize / 2, cameraTarget.position.y,
            _grid.GetLength(1) * hexSize / 2);
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
                        Quaternion.identity);
            }
        }

        return hexGrid;
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
                newHex.ReInitiate(oldHex.GetSerialized().layerHeight,
                    GetEntityTypeFromSerializedType(newHex.GetSerialized().hexType));
            }
        }

        return newGrid;
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
}

[Serializable]
public class SerializedGrid
{
    public SerializedHex[,] grid;
}

[Serializable]
public class SerializedHex
{
    public int layerHeight;
    public SerializedHexType hexType;
}

[Serializable]
public enum SerializedHexType
{
    NONE,
    PLAYER,
    ENEMY
}