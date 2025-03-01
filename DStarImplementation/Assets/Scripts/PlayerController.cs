using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int playerPosition;
    private Vector2Int goalPosition;
    private List<Vector2Int> currentPath;
    private bool isMoving = false;
    private AStarPathfinder pathfinder;
    private Coroutine movementCoroutine;

    public void SetGridManager(GridManager manager)
    {
        gridManager = manager;
        pathfinder = new AStarPathfinder(gridManager); // Initialize the A* pathfinder
        playerPosition = new Vector2Int(1, 1); // Default starting position
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
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero);
            if (hit.collider != null)
            {
                GridCell gridCell = hit.collider.GetComponent<GridCell>();
                if (gridCell != null && gridManager.IsWalkable(gridCell.gridPos))
                {
                    goalPosition = gridCell.gridPos;
                    CalculateNewPath();
                }
            }
        }
    }


    private void CalculateNewPath()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine); // Stop current movement if a new path is needed
        }

        currentPath = pathfinder.FindPath(playerPosition, goalPosition);
        if (currentPath != null && currentPath.Count > 0)
        {
            gridManager.DrawPathArrows(currentPath); // Draw updated path arrows
            movementCoroutine = StartCoroutine(MoveAlongPath());
        }
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        foreach (Vector2Int step in currentPath)
        {
            if (!gridManager.IsWalkable(step)) // Check if the path is still valid
            {
                Debug.Log("Path blocked! Recalculating...");
                CalculateNewPath();
                yield break;
            }

            Vector3 targetPosition = gridManager.GetWorldPosition(step);
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);
                yield return null;
            }
            playerPosition = step; // Update player's grid position
        }

        isMoving = false;
    }
}