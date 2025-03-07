using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathfindingMode 
{
    AStar,
    DStar
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PathfindingMode pathfindingMode;
    public PathfindingMode PathfindingMode => pathfindingMode;

    private GridManager gridManager;
    private Vector2Int playerPosition;
    private Vector2Int goalPosition;
    private List<Vector2Int> currentPath;
    private bool isMoving = false;
    private AStarPathfinder astarPathfinder;
    private DStarPathfinder dstarPathfinder;
    private Coroutine movementCoroutine;

    public void SetGridManager(GridManager manager)
    {
        gridManager = manager;
        astarPathfinder = new AStarPathfinder(gridManager); // Initialize the A* pathfinder
        dstarPathfinder = new DStarPathfinder(gridManager);
        playerPosition = new Vector2Int(1, 1); // Default starting position
        PlacePlayerAtStart();

        gridManager.SetDStarPathfinder(dstarPathfinder);

        // Subscribe to grid changes
        gridManager.OnGridChanged += HandleGridChanged;
    }

    void OnDestroy()
    {
        // Unsubscribe from grid changes to avoid memory leaks
        if (gridManager != null)
        {
            gridManager.OnGridChanged -= HandleGridChanged;
        }
    }

    // Public method to set the pathfinding mode
    public void SetPathfindingMode(PathfindingMode mode)
    {
        pathfindingMode = mode;
        CalculateNewPath(); // Recompute the path when the mode changes
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

    private void HandleGridChanged(Vector2Int changedPosition)
    {
        Debug.Log($"Grid changed at {changedPosition}. Recomputing path...");
        if (pathfindingMode == PathfindingMode.DStar)
        {
            CalculateNewPath();
        }
    }

    private void CalculateNewPath()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine); // Stop current movement if a new path is needed
        }

        gridManager.ResetAllGridPathfindingData();

        if (pathfindingMode == PathfindingMode.AStar)
        {
            currentPath = astarPathfinder.FindPath(playerPosition, goalPosition);
        }
        else 
        {
            currentPath = dstarPathfinder.FindPath(playerPosition, goalPosition);
        }

        if (currentPath != null && currentPath.Count > 0)
        {
            gridManager.DrawPathArrows(currentPath); // Draw updated path arrows
            movementCoroutine = StartCoroutine(MoveAlongPath());
        }
    }

    private IEnumerator MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.Log("No valid path. Player will not move.");
            yield break; // Exit if the path is empty
        }

        isMoving = true;

        foreach (Vector2Int step in currentPath)
        {
            //if (!gridManager.IsWalkable(step)) // Check if the path is still valid
            //{
            //    Debug.Log("Path blocked! Recalculating...");
            //    CalculateNewPath();
            //    yield break;
            //}

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