using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class GameGrid : MonoBehaviour
{
    public int width = 16; 
    public int height = 9; 
    public int borderWidth = 1; 
    // ======================== //
    [SerializeField] private float evaporationRate = 0.3f;
    [SerializeField] private float evaporationInterval = 1f;
    public List<Node> allNodes;
    // ======================== //
    public GameObject[] tilePrefabs;
    public GameObject Core;
    public GameObject wallTilePrefab; 
    public GameObject cornerCastleTilePrefab;
    public GameObject centerCastleTilePrefab;
    public Transform gridParent; 
    public Dictionary<Vector2Int, Node> grid; 
    // ======================== //
    public event Action GridChanged;
  
    void Start()
    {
        GenerateGrid();
        MoveCoreToCenter(width, height);
        PlaceCastleInCenter();

        allNodes = GetAllNodes();
        StartCoroutine(EvaporatePheromones());
    }
    
    void GenerateGrid()
    {
        grid = new Dictionary<Vector2Int, Node>();

        // create the tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tilePrefab = tilePrefabs[(x + y) % tilePrefabs.Length];

                GameObject tileGO = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                tileGO.name = "tile (" + x + "," + y + ")";

                if(gridParent != null)
                    tileGO.transform.SetParent(gridParent);

                Walkable walkableComponent = tileGO.GetComponent<Walkable>();

                // if no component found
                if (walkableComponent == null)
                {
                    Debug.LogError("Tile prefab at position (" + x + ", " + y + ") doesn't have the Walkable script attached.");
                    return;
                }

                // check gameobject component if walkable or not
                Vector2Int gridPosition = new Vector2Int(x, y);
                grid[gridPosition] = new Node(x, y, walkableComponent.walkable);

                // for debug, shows that node is successfully initialized
                //Debug.Log("Node initialized at position (" + x + ", " + y + ") with walkable status: " + walkableComponent.walkable);
            }
        }

        // for debug, method is successful
        Debug.Log("Grid generation completed. Total nodes: " + grid.Count);

        // the grid is enclosed with borders
        CreateBorderWallTiles();
    }


    void CreateBorderWallTiles()
    {
        for (int x = -borderWidth; x < width + borderWidth; x++)
        {
            for (int y = -borderWidth; y < height + borderWidth; y++)
            {
                // prevents the walls being instantiated inside the playable grid
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    Vector3 position = new Vector3(x, y, 0);
                    GameObject wallTileGO = Instantiate(wallTilePrefab, position, Quaternion.identity);
                    wallTileGO.name = "wall (" + x + "," + y + ")";
                    
                    if(gridParent != null)
                        wallTileGO.transform.SetParent(gridParent);
                }
            }
        }
    }

    // updates node data from coordinates (x, y), walkable = true/false
    public void UpdateNodeWalkability()
    {
        foreach (var kvp in grid)
        {
            int x = kvp.Key.x;
            int y = kvp.Key.y;

            Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(x, y), LayerMask.GetMask("Obstacles"));

            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    Walkable walkableComponent = collider.GetComponent<Walkable>();
                    if (walkableComponent != null && walkableComponent.walkable)
                    {
                        grid[new Vector2Int(x, y)].walkable = true;
                        break;
                    }
                }
            }
            else
            {
                grid[new Vector2Int(x, y)].walkable = true;
            }
        }
        GridChanged?.Invoke();
    }
    public void UpdateNodeBuildability()
    {
        foreach (var kvp in grid)
        {
            int x = kvp.Key.x;
            int y = kvp.Key.y;

            Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(x, y), LayerMask.GetMask("Obstacles"));

            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    Buildable buildableComponent = collider.GetComponent<Buildable>();
                    if (buildableComponent != null && !buildableComponent.buildable)
                    {
                        grid[new Vector2Int(x, y)].buildable = false;
                        break;
                    }
                }
            }
            else
            {
                grid[new Vector2Int(x, y)].buildable = true;
            }
        }
        GridChanged?.Invoke();
    }
    //simply moves the "core" game object to the center of the grid
    void MoveCoreToCenter(int width, int height)
    {
        int centerX = width / 2;
        int centerY = height / 2;
        Vector3 centerPosition = new Vector3(centerX, centerY, 0);

        if (Core != null)
        {
            Core.transform.position = centerPosition;
            UpdateNodeWalkability();
            UpdateNodeBuildability();
        }
        else
        {
            Debug.LogError("Core object is not assigned.");
        }
    }

    public Node GetNode(int x, int y)
    {
        Vector2Int position = new Vector2Int(x, y);
        if (grid.ContainsKey(position))
        {
            return grid[position];
        }
        else
        {
            Debug.LogError("No node found at position (" + x + ", " + y + ")");
            return null;
        }
    }
    
    public List<Node> GetAllNodes()
    {
        List<Node> allNodes = new List<Node>();

        foreach (var kvp in grid)
        {
            allNodes.Add(kvp.Value);
        }

        return allNodes;
    }
    private IEnumerator EvaporatePheromones()
    {
        while (true)
        {
            foreach (Node node in allNodes)
            {
                node.pheromoneLevel *= (1 - evaporationRate);
            }
            yield return new WaitForSeconds(evaporationInterval);
        }
    }

    public void DepositPheromones(List<Node> path, float depositRate)
    {
        foreach (Node node in path)
        {
            node.pheromoneLevel += depositRate;
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector2Int[] directions = {
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1)   // Down
        };

        // Iterate through each direction to find neighboring nodes
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborPos = new Vector2Int(node.x + dir.x, node.y + dir.y);
            
            // Check if the neighboring position is within the grid bounds
            if (IsWithinGridBounds(neighborPos))
            {
                // Retrieve the node at the neighboring position and add it to the list of neighbors
                Node neighborNode = grid[neighborPos];
                neighbors.Add(neighborNode);
            }
        }
        return neighbors;
    }
    public bool IsWithinGridBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }
    public bool IsTargetPosition(Vector2Int position)
    {
        int centerX = width / 2;
        int centerY = height / 2;
        int range = 1;

        return Mathf.Abs(position.x - centerX) <= range && Mathf.Abs(position.y - centerY) <= range;
    }
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.y);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0);
    }
    public void TriggerGridChangedEvent()
    {
        GridChanged?.Invoke();
    }
    
    void PlaceCastleInCenter()
    {
        int centerX = width / 2;
        int centerY = height / 2;

        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                GameObject tilePrefab;

                // Determine if the current tile is a corner or not
                if ((x == centerX - 1 && y == centerY - 1) || (x == centerX + 1 && y == centerY - 1) ||
                    (x == centerX - 1 && y == centerY + 1) || (x == centerX + 1 && y == centerY + 1))
                {
                    tilePrefab = cornerCastleTilePrefab;
                }
                else
                {
                    tilePrefab = centerCastleTilePrefab;
                }

                Vector3 position = new Vector3(x, y, 0);
                GameObject tileGO = Instantiate(tilePrefab, position, Quaternion.identity);
                tileGO.name = "castle tile (" + x + "," + y + ")";

                if (gridParent != null)
                {
                    tileGO.transform.SetParent(gridParent);
                }

                Vector2Int gridPosition = new Vector2Int(x, y);
                grid[gridPosition] = new Node(x, y, true, true);  // keep the tiles walkable and buildable
            }
        }
    }
}
