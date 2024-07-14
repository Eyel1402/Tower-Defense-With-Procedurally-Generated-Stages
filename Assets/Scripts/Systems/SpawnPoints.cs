using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SpawnPortal : MonoBehaviour
{
    public GameObject spawnPointPrefab;
    public GameObject tilePrefab;
    public Transform targetPosition;
    public int numberOfSpawnPoints = 5;
    public int minimumDistanceFromTarget = 5;
    public int minimumDistanceBetweenSpawnPoints = 5;
    //[SerializeField] private int CountSpawnPointsWithTag = 0;
    public GameGrid gameGrid;
    public PerlinProps perlinProps;
    public List<Vector2Int> spawnPositions = new List<Vector2Int>(); // Added variable
    private bool hasGameGrid;
    private bool hasSpawnPointsPlaced;
    [SerializeField] private int spawnPointCountWithTag = 0;

    void Start()
    {
        // check if reference is assigned, else the script won't work
        if (gameGrid != null)
        {
            hasGameGrid = true;
        }
        else
        {
            Debug.LogWarning("GameGrid reference is not assigned. Ensure to assign it in the inspector.");
        }

        if (perlinProps == null)
        {
            Debug.LogWarning("PerlinProps reference is not assigned. Ensure to assign it in the inspector.");
        }

        if (spawnPointPrefab != null && targetPosition != null && hasGameGrid)
        {
            PlaceSpawnPoint();
        }
        else
        {
            Debug.LogError("SpawnPortal is missing required components!");
        }
    }

    void PlaceSpawnPoint()
    {
        for (int i = 0; i < numberOfSpawnPoints; i++)
        {
            TryPlacingSpawnPoint();
        }

        spawnPointCountWithTag = CountSpawnPointsWithTag("Spawn");
        if (CountSpawnPointsWithTag("Spawn") == 0)
        {
            Debug.LogWarning("No spawn points found with tag 'Spawn'.");
            PromptSceneRestart();
        }
    }
    int CountSpawnPointsWithTag(string tag)
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(tag);
        return spawnPoints.Length;
    }
    void PromptSceneRestart()
    {
        Debug.LogWarning("Restarting the scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
     
    void TryPlacingSpawnPoint()
    {
        Vector2Int spawnPosition = FindValidSpawnPosition();
        if (spawnPosition != Vector2Int.zero)
        {
            spawnPositions.Add(spawnPosition); 
            GameObject spawnPointGO = Instantiate(spawnPointPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);

            // Set the parent
            GameObject spawnPortalsParent = GameObject.Find("SpawnPortals");
            if (spawnPortalsParent != null)
            {
                spawnPointGO.transform.parent = spawnPortalsParent.transform;
            }
            else
            {
                Debug.LogError("SpawnPortals parent object not found!");
            }

            //Debug.Log("Spawn Point instantiated at position: " + spawnPosition);

            InstantiateTilesForPath(spawnPosition);
        }
        else
        {
            isSpawnPointsPlaced();
            if(isSpawnPointsPlaced())
            {
                Debug.LogWarning("Valid spawn point not found within the grid!");
            }
            if(!isSpawnPointsPlaced())
            {
                Debug.LogWarning("No Valid Spawnpoints have been placed.");
            }
        }
    }
    
    void InstantiateTilesForPath(Vector2Int spawnPosition)
    {
        // Check if a valid path for spawn point
        Pathfinding pathfinding = new Pathfinding(gameGrid);
        List<Node> path = pathfinding.FindPath(spawnPosition, new Vector2Int((int)targetPosition.position.x, (int)targetPosition.position.y));
        if (path != null && path.Count > 0)
        {
            foreach (Node node in path)
            {
                GameObject tileGO = Instantiate(tilePrefab, new Vector3(node.x, node.y, 0), Quaternion.identity);
                GameObject pathParent = GameObject.Find("PathTiles");
                
                gameGrid.UpdateNodeBuildability();
                
                if (pathParent != null)
                {
                    tileGO.transform.parent = pathParent.transform;
                }
            }
        }
    }

    private bool isSpawnPointsPlaced()
    {
        return spawnPositions.Count > 0;
    }

    Vector2Int FindValidSpawnPosition()
    {
        if (!hasGameGrid)
        {
            return Vector2Int.zero;
        }

        const int maxAttempts = 10000;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2Int randomPosition = new Vector2Int(Random.Range(0, gameGrid.width), Random.Range(0, gameGrid.height));

            bool validPosition = true;
            foreach (Vector2Int spawnPos in spawnPositions)
            {
                if (Vector2Int.Distance(randomPosition, spawnPos) < minimumDistanceBetweenSpawnPoints)
                {
                    validPosition = false;
                    break;
                }
            }

            if (validPosition && IsPathPossible(randomPosition) && gameGrid.grid[randomPosition].walkable && Vector2Int.Distance(randomPosition, new Vector2Int((int)targetPosition.position.x, (int)targetPosition.position.y)) >= minimumDistanceFromTarget)
            {
                return randomPosition;
            }
            attempts++;
        }

        return Vector2Int.zero;
    }


    bool IsPathPossible(Vector2Int spawnPosition)
    {
        if (!hasGameGrid)
        {
            return false;
        }

        Pathfinding pathfinding = new Pathfinding(gameGrid);
        List<Node> path = pathfinding.FindPath(spawnPosition, new Vector2Int((int)targetPosition.position.x, (int)targetPosition.position.y));
        return path != null && path.Count > 0;
    }
}