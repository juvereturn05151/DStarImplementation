using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Event to notify when the grid changes
    public event Action<Vector2Int> OnGridChanged;

    private Dictionary<Vector2Int, GridCell> grid = new Dictionary<Vector2Int, GridCell>();
    private HashSet<GridCell> walkableCells = new HashSet<GridCell>();
    private HashSet<GridCell> allCells = new HashSet<GridCell>();
    private Vector3 origin;
    private float spacing;
    private DStarPathfinder dstarPathfinder;

    public void InitializeGrid(Vector3 startPosition, float gridSpacing)
    {
        origin = startPosition;
        spacing = gridSpacing;
    }

    public void SetDStarPathfinder(DStarPathfinder pathfinder)
    {
        dstarPathfinder = pathfinder;
    }

    public void AddCell(Vector2Int position, GridCell cell)
    {
        if (!grid.ContainsKey(position))
        {
            grid[position] = cell;

            // Add to walkableCells if the cell is walkable
            if (cell.cellType == CellType.Walkable)
            {
                walkableCells.Add(cell);
            }

            allCells.Add(cell);
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
            if (cell.cellType == CellType.Walkable)
            {
                cell.GetComponent<SpriteRenderer>().color = color;
            }

        }
    }

    public void ResetCellColors()
    {
        foreach (var cell in grid.Values)
        {
            if (cell.cellType == CellType.Walkable) 
            {
                cell.GetComponent<SpriteRenderer>().color = Color.white; 
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
                walkableCells.Remove(cell);
            }
            else
            {
                cell.cellType = CellType.Walkable;
                cell.GetComponent<SpriteRenderer>().color = Color.white; // Visual feedback for walkable
                walkableCells.Add(cell);
            }

            OnGridChanged?.Invoke(position);
        }
    }

    // Return a list of all walkable cells
    public List<GridCell> GetAllWalkableCells()
    {
        return new List<GridCell>(walkableCells); 
    }

    public List<GridCell> GetAllCells()
    {
        return new List<GridCell>(allCells);
    }

    public List<GridCell> GetNeighbors(Vector2Int position)
    {
        List<GridCell> neighbors = new List<GridCell>();

        // Define the 4 possible directions (up, down, left, right)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 0)   // Right
        };

        // Check each direction
        foreach (var direction in directions)
        {
            Vector2Int neighborPos = position + direction;
            GridCell neighbor = GetCell(neighborPos);
            if (neighbor != null && neighbor.cellType == CellType.Walkable)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
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

    public void ResetAllGridPathfindingData()
    {
        foreach (var cell in grid.Values)
        {
            cell.ResetPathfindingData();
        }
    }
}