using UnityEngine;

public class WallAdder : MonoBehaviour
{
    private GridManager gridManager;

    public void SetGridManager(GridManager manager)
    {
        gridManager = manager;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Left click to toggle walls
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);
            if (hit.collider != null)
            {
                GridCell gridCell = hit.collider.GetComponent<GridCell>();
                if (gridCell != null)
                {
                    ToggleWall(gridCell.gridPos);
                }
            }
        }
    }

    public void ToggleWall(Vector2Int position)
    {
        if (gridManager != null)
        {
            gridManager.ToggleWall(position);
        }
    }
}