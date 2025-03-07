using UnityEngine;
using System.IO;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private int width = 10;
    [SerializeField]
    private int height = 10;
    [SerializeField]
    private float spacing = 1.1f;
    [SerializeField]
    private GridCell walkablePrefab;
    [SerializeField]
    private GridCell wallPrefab;
    [SerializeField]
    private string mapFilePath = "Assets/map.txt";
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private WallAdder wallAdder;

    void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager not assigned!");
            return;
        }

        gridManager.InitializeGrid(startPosition != null ? startPosition.position : Vector3.zero, spacing);
        GenerateGridFromFile();

        if (playerController != null)
        {
            playerController.SetGridManager(gridManager);
        }

        if (wallAdder != null)
        {
            wallAdder.SetGridManager(gridManager);
        }
    }

    void GenerateGridFromFile()
    {
        if (!File.Exists(mapFilePath))
        {
            Debug.LogError("Map file not found!");
            return;
        }

        string[] lines = File.ReadAllLines(mapFilePath);
        height = lines.Length;
        width = lines[0].Length;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                char tile = lines[y][x];
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 position = gridManager.GetWorldPosition(gridPos);
                GameObject cell = null;
                CellType cellType = CellType.Wall;

                if (tile == 'W') // Wall
                {
                    cell = Instantiate(wallPrefab.gameObject, position, Quaternion.identity, gridManager.transform);
                    cell.tag = "Wall";
                }
                else if (tile == 'O') // Walkable area
                {
                    cell = Instantiate(walkablePrefab.gameObject, position, Quaternion.identity, gridManager.transform) ;
                    cell.tag = "Walkable";
                    cellType = CellType.Walkable;
                }

                if (cell != null)
                {
                    cell.name = $"Cell_{x}_{y}";
                    GridCell gridCell = cell.GetComponent<GridCell>();
                    gridCell.Initialize(gridPos, cellType);
                    gridManager.AddCell(gridPos, gridCell);
                }
            }
        }
    }
}
