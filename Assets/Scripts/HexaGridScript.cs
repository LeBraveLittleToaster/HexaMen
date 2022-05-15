using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexaGridScript : MonoBehaviour
{
    [SerializeField] private Hex hexaPrefab;

    [SerializeField] private int rowCount;
    [SerializeField] private int columnCount;
    [SerializeField] private float hexSize = 2;

    private Hex[,] _grid;

    private void Start()
    {
        _grid = GenerateGrid();
    }

    private Hex[,] GenerateGrid()
    {
        var hexGrid = new Hex[columnCount, rowCount];
        var hexWidth = 2 * hexSize;
        var hexHeight = Mathf.Sqrt(3) * hexSize;

        for (var columnI = 0; columnI < columnCount; columnI++)
        {
            for (var rowI = 0; rowI < rowCount; rowI++)
            {
                hexGrid[columnI, rowI] =
                    Instantiate(hexaPrefab, GetPositionFromIndexes(columnI, rowI, hexWidth, hexHeight), Quaternion.identity);
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

    public void PrintJsonToConsole()
    {
        var grid = new SerializedHex[_grid.GetLength(0), _grid.GetLength(1)];
        for (var columnI = 0; columnI < _grid.GetLength(0); columnI++)
        {
            for (var rowI = 0; rowI < _grid.GetLength(1); rowI++)
            {
                grid[columnI, rowI] = _grid[columnI, rowI].GetComponent<Hex>().GetSerialized();
            }   
        }

        var sGrid = new SerializedGrid();
        sGrid.grid = grid;
        Debug.Log(grid);
        Debug.Log(JsonConvert.SerializeObject(sGrid));
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