using UnityEngine;

public enum CellType
{
    Walkable,
    Wall
}

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPos;
    public CellType cellType;

    // A* Pathfinding properties
    public float gCost; // Cost from start to this node
    public float fCost; // Total cost (gCost + heuristic)
    public GridCell parent; // Parent node for path reconstruction

    public void Initialize(Vector2Int position, CellType type)
    {
        gridPos = position;
        cellType = type;
        ResetPathfindingData();
    }

    public void ResetPathfindingData()
    {
        gCost = float.MaxValue;
        fCost = float.MaxValue;
        parent = null;
    }
}