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

    public void Initialize(Vector2Int position, CellType type)
    {
        gridPos = position;
        cellType = type;
    }
}
