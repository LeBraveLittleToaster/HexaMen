using System.Collections.Generic;
using Grid;
using UnityEngine;

public class GameboardScript : MonoBehaviour
{
    [SerializeField] private int column = 7;
    [SerializeField] private int row = 5;

    [SerializeField] private int hexSize = 2;

    [SerializeField] private TileScript hexPrefab;
    [SerializeField] private EnemyScript enemyPrefab;
    [SerializeField] private PlayerScript playerPrefab;

    private IHexGrid _grid;
    private TileScript[,] _gaGrid;

    private List<IHexTile> _highlightedTiles = new();

    private void Awake()
    {
        _grid = new HexGrid(column, row);
        _gaGrid = GenerateEmptyBoard(_grid);
        IncreaseRandomHeight(1000);
    }

    public void TestRandomPathFinding()
    {
        foreach (var highlightedTile in _highlightedTiles)
        {
            _gaGrid[highlightedTile.GetPosition().X, highlightedTile.GetPosition().Y].SetHighlighted(false);
        }

        var randomColumnStart = Random.Range(0, _grid.GetTiles().GetLength(0));
        var randomRowStart = Random.Range(0, _grid.GetTiles().GetLength(1));
        var randomColumnEnd = Random.Range(0, _grid.GetTiles().GetLength(0));
        var randomRowEnd = Random.Range(0, _grid.GetTiles().GetLength(1));
        Debug.Log("Finding path from [" + randomColumnStart + "," + randomRowStart + " to [" + randomColumnEnd + "," +
                  randomRowEnd + "]");
        var hexPathfinder = new HexPathfinder();
        _highlightedTiles = hexPathfinder.GetPath(
            _grid,
            _grid.GetTileAt(randomColumnStart, randomRowStart),
            _grid.GetTileAt(randomColumnEnd, randomRowEnd),
            100);
        foreach (var highlightedTile in _highlightedTiles)
        {
            _gaGrid[highlightedTile.GetPosition().X, highlightedTile.GetPosition().Y].SetHighlighted(true);
        }
    }

    private void IncreaseRandomHeight(int steps)
    {
        var columnCount = _grid.GetTiles().GetLength(0);
        var rowCount = _grid.GetTiles().GetLength(1);
        for (var i = 0; i < steps; i++)
        {
            var randomColumn = Random.Range(0, columnCount);
            var randomRow = Random.Range(0, rowCount);
            AddLayerAt(randomColumn, randomRow);
        }
    }

    public bool AddLayerAt(int columnI, int rowI)
    {
        var tile = _grid.GetTileAt(columnI, rowI);
        if (tile == null) return false;
        tile.AddLayer();
        _gaGrid[columnI, rowI].SyncWithHexTile(tile);
        return true;
    }

    private TileScript[,] GenerateEmptyBoard(IHexGrid grid)
    {
        var columnCount = grid.GetTiles().GetLength(0);
        var rowCount = grid.GetTiles().GetLength(1);

        var gaTiles = new TileScript[columnCount, rowCount];
        var hexWidth = 2 * hexSize;
        var hexHeight = Mathf.Sqrt(3) * hexSize;
        for (var c = 0; c < columnCount; c++)
        {
            for (var r = 0; r < rowCount; r++)
            {
                gaTiles[c, r] = Instantiate(hexPrefab, GetPositionFromIndexes(c, r, hexWidth, hexHeight),
                    Quaternion.identity).SyncWithHexTile(grid.GetTiles()[c, r]);
            }
        }

        return gaTiles;
    }

    private static Vector3 GetPositionFromIndexes(int columnI, int rowI, float hexWidth, float hexHeight)
    {
        return rowI % 2 == 0
            ? new Vector3(columnI * hexWidth + hexWidth / 2, 0, rowI * hexHeight)
            : new Vector3(columnI * hexWidth, 0, rowI * hexHeight);
    }
}