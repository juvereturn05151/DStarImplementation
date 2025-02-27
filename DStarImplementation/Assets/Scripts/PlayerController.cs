using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2Int playerPosition;
    private float spacing;
    private int height;

    public void SetGridParameters(float gridSpacing, int gridHeight)
    {
        spacing = gridSpacing;
        height = gridHeight;
        playerPosition = new Vector2Int(1, 1);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);
            if (hit.collider != null)
            {
                GridCell gridCell = hit.collider.GetComponent<GridCell>();
                if (gridCell != null && gridCell.cellType == CellType.Walkable)
                {
                    MovePlayer(gridCell);
                }
            }
        }
    }

    void MovePlayer(GridCell targetCell)
    {
        playerPosition = targetCell.gridPos;
        transform.position = targetCell.transform.position; // Use actual cell position
    }
}
