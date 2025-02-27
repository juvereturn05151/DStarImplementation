using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Dictionary<Vector2Int, GridCell> grid = new Dictionary<Vector2Int, GridCell>();
    private Vector3 origin;
    private float spacing;

    public void InitializeGrid(Vector3 startPosition, float gridSpacing)
    {
        origin = startPosition;
        spacing = gridSpacing;
    }

    public void AddCell(Vector2Int position, GridCell cell)
    {
        if (!grid.ContainsKey(position))
        {
            grid[position] = cell;
        }
    }

    public GridCell GetCell(Vector2Int position)
    {
        return grid.TryGetValue(position, out GridCell cell) ? cell : null;
    }

    public bool IsWalkable(Vector2Int position)
    {
        return grid.ContainsKey(position) && grid[position].cellType == CellType.Walkable;
    }

    public Vector3 GetWorldPosition(Vector2Int position)
    {
        return origin + new Vector3(position.x * spacing, position.y * spacing, 0);
    }
}
