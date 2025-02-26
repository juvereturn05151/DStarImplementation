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
    private GameObject walkablePrefab;
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private string mapFilePath = "Assets/map.txt";

    void Start()
    {
        GenerateGridFromFile();
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
                Vector3 position = startPosition.position + new Vector3(x * spacing, y * spacing,0);
                GameObject cell = null;

                if (tile == 'W') // Wall
                {
                    cell = Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else if (tile == 'O') // Walkable area
                {
                    cell = Instantiate(walkablePrefab, position, Quaternion.identity, transform);
                }

                if (cell != null)
                {
                    cell.name = $"Cell_{x}_{y}";
                }
            }
        }
    }
}
