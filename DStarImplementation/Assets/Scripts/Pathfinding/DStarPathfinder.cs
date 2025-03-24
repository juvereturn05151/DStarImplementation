using System.Collections.Generic;
using UnityEngine;

public class DStarPathfinder
{
    private GridManager gridManager;
    private Dictionary<Vector2Int, float> gScore;
    private Dictionary<Vector2Int, float> rhs;
    private PriorityQueue<Vector2Int> openList;
    private Vector2Int start;
    private Vector2Int goal;
    public Vector2Int Goal { get; private set; }
    public bool IsInitialized => Goal != default;


    public DStarPathfinder(GridManager manager)
    {
        gridManager = manager;
        gScore = new Dictionary<Vector2Int, float>();
        rhs = new Dictionary<Vector2Int, float>();
        openList = new PriorityQueue<Vector2Int>();
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
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
            cell.SetCosts(float.MaxValue, float.MaxValue, float.MaxValue); // Reset cell costs
        }

        // Set rhs of the goal to 0 and add it to the open list
        rhs[goal] = 0;
        openList.Enqueue(goal, Heuristic(goal, start));

        ComputeShortestPath();

        List<Vector2Int> path = ReconstructPath();

        return path;
    }

    public void UpdateEdge(Vector2Int pos)
    {
        if (!IsInitialized) return;

        // Only process if the changed cell affects the path
        if (gridManager.IsWalkable(pos))
        {
            UpdateRhs(pos);
            ComputeShortestPath();
        }
    }

    private void ComputeShortestPath()
    {
        while (!openList.IsEmpty())
        {
            Vector2Int current = openList.Dequeue();

            // If the start node is consistent, we're done
            if (current == start && gScore[start] == rhs[start])
            {
                break;
            }

            // A node is in Lower State
            if (gScore[current] > rhs[current])
            {
                gScore[current] = rhs[current];
            }
            // A node is in Raise State
            else if (gScore[current] < rhs[current])
            {
                gScore[current] = float.MaxValue;
                UpdateRhs(current);
            }

            // Update all neighbors
            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                if (!gridManager.IsWalkable(neighbor.gridPos))
                {
                    continue;
                }
                UpdateRhs(neighbor.gridPos);
            }
        }
    }

    private void UpdateRhs(Vector2Int pos)
    {
        // Skip if the cell is a wall or doesn't exist
        if (!gridManager.IsWalkable(pos))
        {
            return;
        }

        if (pos != goal)
        {
            float minCost = float.MaxValue;
            foreach (GridCell neighbor in gridManager.GetNeighbors(pos))
            {
                if (!gridManager.IsWalkable(neighbor.gridPos))
                {
                    continue;
                }

                // Ensure the neighbor exists in gScore
                if (gScore.ContainsKey(neighbor.gridPos))
                {
                    float cost = gScore[neighbor.gridPos] + 1; // Assuming uniform cost
                    if (cost < minCost)
                    {
                        minCost = cost;
                    }
                }
            }

            rhs[pos] = minCost;
        }

        if (openList.Contains(pos))
        {
            openList.Remove(pos);
        }

        if (gScore[pos] != rhs[pos])
        {
            float fcost = Mathf.Min(gScore[pos], rhs[pos]) + Heuristic(pos, start);
            openList.Enqueue(pos, fcost);
        }

        // Update the GridCell with the new costs
        GridCell cell = gridManager.GetCell(pos);
        if (cell != null)
        {
            cell.SetCosts(gScore[pos], rhs[pos], Mathf.Min(gScore[pos], rhs[pos]) + Heuristic(pos, start));
        }
    }

    public List<Vector2Int> ReconstructPath(Vector2Int start, Vector2Int goal)
    {
        // Ensure the goal hasn't changed
        if (goal != Goal)
        {
            Debug.LogWarning("D* goal changed - requiring full replan");
            return FindPath(start, goal);
        }

        // Reuse existing g/rhs values
        return ReconstructPath();
    }

    private List<Vector2Int> ReconstructPath()
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;

        while (current != goal)
        {
            path.Add(current);

            Vector2Int nextStep = current;
            float minCost = float.MaxValue;

            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                if (!gridManager.IsWalkable(neighbor.gridPos))
                {
                    continue;
                }

                if (gScore.ContainsKey(neighbor.gridPos))
                {
                    if (gScore[neighbor.gridPos] < minCost)
                    {
                        minCost = gScore[neighbor.gridPos];
                        nextStep = neighbor.gridPos;
                    }
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
        }
        // Return empty path if the goal was not reached
        else
        {
            return new List<Vector2Int>();
        }

        return path;
    }

    // Manhattan heuristic
    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}