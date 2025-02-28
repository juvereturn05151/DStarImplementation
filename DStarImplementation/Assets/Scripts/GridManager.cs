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

    public void SetCellColor(Vector2Int position, Color color)
    {
        if (grid.TryGetValue(position, out GridCell cell))
        {
            cell.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void ResetCellColors()
    {
        foreach (var cell in grid.Values)
        {
            if (cell.cellType == CellType.Walkable) 
            {
                cell.GetComponent<SpriteRenderer>().color = Color.white; // Reset to default color
            }
        }
    }

    public void DrawPathArrows(List<Vector2Int> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2Int current = path[i];
            Vector2Int next = path[i + 1];
            Vector3 direction = (GetWorldPosition(next) - GetWorldPosition(current)).normalized;
            DrawArrow(GetWorldPosition(current), direction);
        }
    }

    private void DrawArrow(Vector3 position, Vector3 direction)
    {
        Debug.DrawRay(position, direction * 0.5f, Color.red, 2f); // Draw an arrow using Debug.DrawRay
    }
}