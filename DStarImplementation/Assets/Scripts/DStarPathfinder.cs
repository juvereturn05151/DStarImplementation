using System.Collections;
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
        }

        // Set rhs of the goal to 0 and add it to the open list
        rhs[goal] = 0;
        openList.Enqueue(goal, Heuristic(goal, start));

        ComputeShortestPath();

        return ReconstructPath();
    }

    public void UpdateEdge(Vector2Int pos)
    {
        GridCell cell = gridManager.GetCell(pos);
        if (cell != null && cell.cellType == CellType.Walkable)
        {
            foreach (GridCell neighbor in gridManager.GetNeighbors(pos))
            {
                UpdateVertex(neighbor.gridPos);
            }
            ComputeShortestPath();
        }
    }

    private void ComputeShortestPath()
    {
        while (!openList.IsEmpty() && (gScore[start] != rhs[start]))
        {
            Vector2Int current = openList.Dequeue();

            if (gScore[current] > rhs[current])
            {
                gScore[current] = rhs[current];
            }
            else
            {
                gScore[current] = float.MaxValue;
                UpdateVertex(current);
            }

            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                UpdateVertex(neighbor.gridPos);
            }
        }
    }

    private void UpdateVertex(Vector2Int pos)
    {
        if (pos != goal)
        {
            float minCost = float.MaxValue;
            foreach (GridCell neighbor in gridManager.GetNeighbors(pos))
            {
                float cost = gScore[neighbor.gridPos] + 1; // Assuming uniform cost of 1 for all edges
                if (cost < minCost)
                {
                    minCost = cost;
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
            openList.Enqueue(pos, Mathf.Min(gScore[pos], rhs[pos]) + Heuristic(pos, start));
        }
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
                if (gScore.ContainsKey(neighbor.gridPos) && gScore[neighbor.gridPos] < minCost)
                {
                    minCost = gScore[neighbor.gridPos];
                    nextStep = neighbor.gridPos;
                }
            }

            if (nextStep == current) break; // No path found
            current = nextStep;
        }

        path.Add(goal);
        return path;
    }

    //Manhattan
    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
