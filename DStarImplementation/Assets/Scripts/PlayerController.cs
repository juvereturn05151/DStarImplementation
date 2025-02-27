using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int playerPosition;

    public void SetGridManager(GridManager manager)
    {
        gridManager = manager;
        playerPosition = new Vector2Int(1, 1);
        PlacePlayerAtStart();
    }

    void PlacePlayerAtStart()
    {
        if (gridManager.IsWalkable(playerPosition))
        {
            transform.position = gridManager.GetWorldPosition(playerPosition);
        }
        else
        {
            Debug.LogError("Starting GridCell (1,1) not found or not walkable!");
        }
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
                if (gridCell != null && gridManager.IsWalkable(gridCell.gridPos))
                {
                    MovePlayer(gridCell.gridPos);
                }
            }
        }
    }

    void MovePlayer(Vector2Int newPosition)
    {
        if (gridManager.IsWalkable(newPosition))
        {
            playerPosition = newPosition;
            transform.position = gridManager.GetWorldPosition(playerPosition);
        }
    }
}
