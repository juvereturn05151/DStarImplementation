using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField]
    private TextMeshProUGUI performanceText;

    private GridManager gridManager;
    private Vector2Int playerPosition;
    private Vector2Int goalPosition;
    private List<Vector2Int> currentPath;
    private bool isMoving = false;
    private AStarPathfinder astarPathfinder;
    private DStarPathfinder dstarPathfinder;
    private Coroutine movementCoroutine;

    private float astarLastComputeTime;
    private float dstarLastUpdateTime;
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

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
            astarLastComputeTime = 0.0f;
            dstarLastUpdateTime = 0.0f;

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

        UpdatePerformanceDisplay();
    }

    private void HandleGridChanged(Vector2Int changedPosition)
    {

        if (pathfindingMode == PathfindingMode.DStar)
        {
            stopwatch.Restart();
            // Only update the affected edge in D*
            dstarPathfinder.UpdateEdge(changedPosition);

            // Reconstruct path from existing D* data
            currentPath = dstarPathfinder.ReconstructPath(playerPosition, goalPosition);

            stopwatch.Stop();
            dstarLastUpdateTime += stopwatch.ElapsedMilliseconds;

            gridManager.DrawPathArrows(currentPath);

            // If currently moving, restart movement with the updated path
            if (movementCoroutine != null) StopCoroutine(movementCoroutine);
            movementCoroutine = StartCoroutine(MoveAlongPath());
        }
        else 
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



        if (pathfindingMode == PathfindingMode.AStar)
        {
            stopwatch.Restart(); 
            gridManager.ResetAllGridPathfindingData();
            currentPath = astarPathfinder.FindPath(playerPosition, goalPosition);
            stopwatch.Stop();
            astarLastComputeTime += stopwatch.ElapsedMilliseconds;
        }
        else 
        {
            stopwatch.Restart();
            // First-time planning or goal change requires full initialization
            if (dstarPathfinder.Goal != goalPosition || !dstarPathfinder.IsInitialized)
            {
                currentPath = dstarPathfinder.FindPath(playerPosition, goalPosition);
            }
            else
            {
                // Subsequent updates use incremental reconstruction
                currentPath = dstarPathfinder.ReconstructPath(playerPosition, goalPosition);
            }
            stopwatch.Stop();
            dstarLastUpdateTime += stopwatch.ElapsedMilliseconds;
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
            Vector3 targetPosition = gridManager.GetWorldPosition(step);
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);
                yield return null;
            }
            playerPosition = step; // Update player's grid position
        }

        currentPath.Clear();

        isMoving = false;
    }

    private void UpdatePerformanceDisplay()
    {
        performanceText.text = $"A*: {astarLastComputeTime}ms\nD*: {dstarLastUpdateTime}ms";

        // Auto-hide after 3 seconds
        //StartCoroutine(HidePerformanceText());
    }

    private IEnumerator HidePerformanceText()
    {
        yield return new WaitForSeconds(3f);
        performanceText.text = "";
    }
}

