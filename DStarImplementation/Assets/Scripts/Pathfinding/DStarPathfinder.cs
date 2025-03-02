using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

        //initialize gScore and rhs for all walkable cells
        foreach (GridCell cell in gridManager.GetAllWalkableCells())
        {
            gScore[cell.gridPos] = float.MaxValue;
            rhs[cell.gridPos] = float.MaxValue;
        }

        //set rhs of the goal to 0 and add it to the open list
        rhs[goal] = 0;
        openList.Enqueue(goal, Heuristic(goal, start));

        ComputeShortestPath();

        List<Vector2Int> path = ReconstructPath();

        return path;
    }

    public void UpdateEdge(Vector2Int pos)
    {
        GridCell cell = gridManager.GetCell(pos);
        if (cell != null && cell.cellType == CellType.Walkable)
        {
            foreach (GridCell neighbor in gridManager.GetNeighbors(pos))
            {
                UpdateRhs(neighbor.gridPos);
            }
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
            // If the node is overconsistent (gScore > rhs), update its gScore
            if (gScore[current] > rhs[current])
            {
                gScore[current] = rhs[current];
            }
            //A node is in Raise State
            // If the node is underconsistent (gScore < rhs), reset its gScore and update its rhs
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
        //skip if the cell is a wall or doesn't exist
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

                //ensure the neighbor exists in gScore
                if (gScore.ContainsKey(neighbor.gridPos))
                {
                    //assuming uniform cost of 1 for all edges
                    float cost = gScore[neighbor.gridPos] + 1;
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
            //Debug.Log("fcost" + fcost);
            openList.Enqueue(pos, fcost);
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
                //skip if the neighbor is a wall or doesn't exist
                if (!gridManager.IsWalkable(neighbor.gridPos))
                {
                    continue;
                }

                //ensure the neighbor exists in gScore
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

        //add the goal to the path only if the path is valid
        if (current == goal)
        {
            path.Add(goal);
        }
        //return empty path if the goal was not reached
        else
        {
            return new List<Vector2Int>(); 
        }

        return path;
    }

    //Manhattan
    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        float h = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
       // Debug.Log($"Heuristic from {a} to {b} = {h}");
        return h;
    }
}
