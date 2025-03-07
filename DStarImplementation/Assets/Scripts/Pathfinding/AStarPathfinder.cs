using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    private GridManager gridManager;

    public AStarPathfinder(GridManager manager)
    {
        gridManager = manager;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        // Initialize open and closed lists
        var openList = new List<GridCell>();
        var closedList = new HashSet<Vector2Int>();

        // Get the start and target cells
        GridCell startCell = gridManager.GetCell(start);
        GridCell targetCell = gridManager.GetCell(target);

        if (startCell == null || targetCell == null)
        {
            Debug.LogError("Start or target cell is null!");
            return null;
        }

        gridManager.ResetCellColors(); // Reset colors after pathfinding

        // Initialize start node
        startCell.ResetPathfindingData();
        startCell.GCost = 0;
        startCell.HCost = Heuristic(start, target);
        startCell.FCost = startCell.GCost + startCell.HCost;
        openList.Add(startCell);

        // Debug: Color the start and target cells
        gridManager.SetCellColor(start, Color.green); // Start cell
        gridManager.SetCellColor(target, Color.red); // Target cell

        while (openList.Count > 0)
        {
            // Get the node with the lowest fCost from the open list
            GridCell currentNode = GetLowestFCostNode(openList);

            // If we've reached the target, reconstruct and return the path
            if (currentNode.gridPos == target)
            {
                var path = ReconstructPath(currentNode);

                return path;
            }

            // Move the current node from the open list to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode.gridPos);

            // Debug: Color the closed list cells yellow
            gridManager.SetCellColor(currentNode.gridPos, Color.yellow);

            // Evaluate each neighbor
            foreach (Vector2Int neighborPos in GetNeighbors(currentNode.gridPos))
            {
                GridCell neighborCell = gridManager.GetCell(neighborPos);

                // Skip unwalkable or already evaluated neighbors
                if (neighborCell == null || neighborCell.cellType == CellType.Wall) continue;
                if (closedList.Contains(neighborPos)) continue;

                // Calculate tentative gCost (cost from start to neighbor)
                float tentativeGCost = currentNode.GCost + 1; // Assuming each step has a cost of 1

                // Check if the neighbor is already in the open list
                if (!openList.Contains(neighborCell))
                {
                    // If not in the open list, add it
                    neighborCell.GCost = tentativeGCost;
                    neighborCell.FCost = tentativeGCost + Heuristic(neighborPos, target);
                    neighborCell.parent = currentNode;
                    openList.Add(neighborCell);

                    // Debug: Color the open list cells blue
                    gridManager.SetCellColor(neighborPos, Color.blue);
                }
                else if (tentativeGCost < neighborCell.GCost)
                {
                    // If in the open list and the new gCost is lower, update the node
                    neighborCell.GCost = tentativeGCost;
                    neighborCell.FCost = tentativeGCost + Heuristic(neighborPos, target);
                    neighborCell.parent = currentNode;
                }
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(GridCell endNode)
    {
        var path = new List<Vector2Int>();
        GridCell currentNode = endNode;

        while (currentNode != null)
        {
            path.Insert(0, currentNode.gridPos);
            currentNode = currentNode.parent;
        }

        // Debug: Draw arrows along the path
        gridManager.DrawPathArrows(path);

        return path;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        // Manhattan distance heuristic
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private GridCell GetLowestFCostNode(List<GridCell> openList)
    {
        GridCell lowestNode = openList[0];
        foreach (GridCell node in openList)
        {
            if (node.FCost < lowestNode.FCost)
            {
                lowestNode = node;
            }
        }
        return lowestNode;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int position)
    {
        // Returns walkable neighbors (up, down, left, right)
        var neighbors = new List<Vector2Int>
        {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1)
        };

        // Filter out unwalkable neighbors
        neighbors.RemoveAll(neighbor => !gridManager.IsWalkable(neighbor));
        return neighbors;
    }
}