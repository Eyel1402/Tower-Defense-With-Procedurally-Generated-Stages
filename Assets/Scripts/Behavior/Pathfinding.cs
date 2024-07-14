using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private GameGrid grid;

    public Pathfinding(GameGrid grid)
    {
        this.grid = grid;
    }

    public List<Node> FindPath(Vector2Int start, Vector2Int target)
    {
        Node startNode = grid.grid[start];
        Node targetNode = grid.grid[target];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        //debug lines
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 startPos = new Vector3(path[i].x, path[i].y, 0);
            Vector3 endPos = new Vector3(path[i + 1].x, path[i + 1].y, 0);
            //Debug.DrawLine(startPos, endPos, Color.yellow, 10f);
        }

        return path;
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        int[][] directions = new int[][]
        {
            new int[] { 0, 1 },  // Up
            new int[] { 0, -1 }, // Down
            new int[] { -1, 0 }, // Left
            new int[] { 1, 0 }   // Right
        };

        foreach (var dir in directions)
        {
            int checkX = node.x + dir[0];
            int checkY = node.y + dir[1];

            if (checkX >= 0 && checkX < grid.width && checkY >= 0 && checkY < grid.height)
            {
                Node neighbor = grid.grid[new Vector2Int(checkX, checkY)];
                if (neighbor.walkable)
                {
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.x - nodeB.x);
        int distY = Mathf.Abs(nodeA.y - nodeB.y);

        return distX + distY;
    }
}
