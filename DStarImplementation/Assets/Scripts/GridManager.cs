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

    public void ToggleWall(Vector2Int position)
    {
        if (grid.TryGetValue(position, out GridCell cell))
        {
            if (cell.cellType == CellType.Walkable)
            {
                cell.cellType = CellType.Wall;
                cell.GetComponent<SpriteRenderer>().color = Color.black; // Visual feedback for wall
            }
            else
            {
                cell.cellType = CellType.Walkable;
                cell.GetComponent<SpriteRenderer>().color = Color.white; // Visual feedback for walkable
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
        float arrowLength = 0.5f; // Length of the arrow body
        float arrowHeadLength = 0.2f; // Length of the arrowhead lines
        float arrowHeadAngle = 20.0f; // Angle of the arrowhead lines in degrees

        // Draw the arrow body
        Debug.DrawRay(position, direction * arrowLength, Color.red, 2f);

        // Calculate the end position of the arrow body
        Vector3 endPosition = position + direction * arrowLength;

        // Calculate the directions for the arrowhead lines
        Vector3 arrowHeadDir1 = Quaternion.Euler(0, 0, arrowHeadAngle) * (-direction);
        Vector3 arrowHeadDir2 = Quaternion.Euler(0, 0, -arrowHeadAngle) * (-direction);

        // Draw the arrowhead lines
        Debug.DrawRay(endPosition, arrowHeadDir1 * arrowHeadLength, Color.red, 2f);
        Debug.DrawRay(endPosition, arrowHeadDir2 * arrowHeadLength, Color.red, 2f);
    }
}