using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DStarPathfinder
{
    private GridManager gridManager;
    private Dictionary<Vector2Int, float> gScore;
    private Dictionary<Vector2Int, float> rhs;
    private PriorityQueue<Vector2Int> openList;
    private Vector2Int start;
    private Vector2Int goal;

    public DStarPathfinder(GridManager manager)
    {
        gridManager = manager;
        gScore = new Dictionary<Vector2Int, float>();
        rhs = new Dictionary<Vector2Int, float>();
        openList = new PriorityQueue<Vector2Int>();
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        Debug.Log($"Starting D* pathfinding from {start} to {goal}");
        this.start = start;
        this.goal = goal;
        gScore.Clear();
        rhs.Clear();
        openList.Clear();

        // Initialize gScore and rhs for all walkable cells
        foreach (GridCell cell in gridManager.GetAllWalkableCells())
        {
            gScore[cell.gridPos] = float.MaxValue;
            rhs[cell.gridPos] = float.MaxValue;
            Debug.Log($"Initialized cell {cell.gridPos}: gScore = {gScore[cell.gridPos]}, rhs = {rhs[cell.gridPos]}");
        }

        // Set rhs of the goal to 0 and add it to the open list
        rhs[goal] = 0;
        openList.Enqueue(goal, Heuristic(goal, start));
        Debug.Log($"Set rhs[{goal}] = 0 and added to open list");

        ComputeShortestPath();

        List<Vector2Int> path = ReconstructPath();
        if (path.Count == 0)
        {
            Debug.Log("No valid path found. Returning empty path.");
        }
        else
        {
            Debug.Log($"Reconstructed path: {string.Join(" -> ", path)}");
        }

        return path;
    }

    public void UpdateEdge(Vector2Int pos)
    {
        Debug.Log($"Updating edge at {pos}");
        GridCell cell = gridManager.GetCell(pos);
        if (cell != null && cell.cellType == CellType.Walkable)
        {
            foreach (GridCell neighbor in gridManager.GetNeighbors(pos))
            {
                Debug.Log($"Updating vertex at neighbor {neighbor.gridPos}");
                UpdateVertex(neighbor.gridPos);
            }
            ComputeShortestPath();
        }
    }

    private void ComputeShortestPath()
    {
        Debug.Log("Computing shortest path...");
        while (!openList.IsEmpty())
        {
            Vector2Int current = openList.Dequeue();
            Debug.Log($"Processing cell {current}: gScore = {gScore[current]}, rhs = {rhs[current]}");

            // If the start node is consistent, we're done
            if (current == start && gScore[start] == rhs[start])
            {
                Debug.Log("Start node is consistent. Pathfinding complete.");
                break;
            }

            // If the node is overconsistent (gScore > rhs), update its gScore
            if (gScore[current] > rhs[current])
            {
                gScore[current] = rhs[current];
                Debug.Log($"Updated gScore[{current}] = {gScore[current]}");
            }
            // If the node is underconsistent (gScore < rhs), reset its gScore and update its rhs
            else
            {
                gScore[current] = float.MaxValue;
                Debug.Log($"Reset gScore[{current}] = {gScore[current]}");
                UpdateVertex(current);
            }

            // Update all neighbors
            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                Debug.Log($"Updating vertex at neighbor {neighbor.gridPos}");
                UpdateVertex(neighbor.gridPos);
            }
        }
    }

    private void UpdateVertex(Vector2Int pos)
    {
        Debug.Log($"Updating vertex at {pos}");

        // Skip if the cell is a wall or doesn't exist
        if (!gridManager.IsWalkable(pos))
        {
            Debug.Log($"Cell {pos} is a wall or doesn't exist. Skipping.");
            return;
        }

        if (pos != goal)
        {
            float minCost = float.MaxValue;
            foreach (GridCell neighbor in gridManager.GetNeighbors(pos))
            {
                if (!gridManager.IsWalkable(neighbor.gridPos))
                {
                    Debug.Log($"Neighbor {neighbor.gridPos} is a wall or doesn't exist. Skipping.");
                    continue;
                }

                // Ensure the neighbor exists in gScore
                if (gScore.ContainsKey(neighbor.gridPos))
                {
                    float cost = gScore[neighbor.gridPos] + 1; // Assuming uniform cost of 1 for all edges
                    Debug.Log($"Checking neighbor {neighbor.gridPos}: cost = {cost}");
                    if (cost < minCost)
                    {
                        minCost = cost;
                        Debug.Log($"New minCost = {minCost} at {neighbor.gridPos}");
                    }
                }
                else
                {
                    Debug.Log($"Neighbor {neighbor.gridPos} not found in gScore. Skipping.");
                }
            }
            rhs[pos] = minCost;
            Debug.Log($"Updated rhs[{pos}] = {rhs[pos]}");
        }

        if (openList.Contains(pos))
        {
            Debug.Log($"Removing {pos} from open list");
            openList.Remove(pos);
        }

        if (gScore[pos] != rhs[pos])
        {
            float key = Mathf.Min(gScore[pos], rhs[pos]) + Heuristic(pos, start);
            Debug.Log($"Adding {pos} to open list with key = {key}");
            openList.Enqueue(pos, key);
        }
    }

    private List<Vector2Int> ReconstructPath()
    {
        Debug.Log("Reconstructing path...");
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;

        while (current != goal)
        {
            path.Add(current);
            Debug.Log($"Added {current} to path");

            Vector2Int nextStep = current;
            float minCost = float.MaxValue;

            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                // Skip if the neighbor is a wall or doesn't exist
                if (!gridManager.IsWalkable(neighbor.gridPos))
                {
                    Debug.Log($"Neighbor {neighbor.gridPos} is a wall or doesn't exist. Skipping.");
                    continue;
                }

                // Ensure the neighbor exists in gScore
                if (gScore.ContainsKey(neighbor.gridPos))
                {
                    Debug.Log($"Checking neighbor {neighbor.gridPos}: gScore = {gScore[neighbor.gridPos]}");
                    if (gScore[neighbor.gridPos] < minCost)
                    {
                        minCost = gScore[neighbor.gridPos];
                        nextStep = neighbor.gridPos;
                        Debug.Log($"New best neighbor {neighbor.gridPos} with gScore = {minCost}");
                    }
                }
                else
                {
                    Debug.Log($"Neighbor {neighbor.gridPos} not found in gScore. Skipping.");
                }
            }

            if (nextStep == current) 
            {
                Debug.Log("No path found: nextStep == current");
                break;
            } 
            current = nextStep;
        }

        // Add the goal to the path only if the path is valid
        if (current == goal)
        {
            path.Add(goal);
            Debug.Log($"Added goal {goal} to path");
        }
        else
        {
            Debug.Log("No valid path found. Returning empty path.");
            return new List<Vector2Int>(); // Return empty path if the goal was not reached
        }

        return path;
    }

    //Manhattan
    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        float h = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        Debug.Log($"Heuristic from {a} to {b} = {h}");
        return h;
    }
}
