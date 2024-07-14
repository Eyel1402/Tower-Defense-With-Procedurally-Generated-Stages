using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class PerlinProps : MonoBehaviour
{
    public GameObject treePrefab; 
    public GameObject rockPrefab; 
    public GameGrid gameGrid;
    // seed parameters
    public int seedMinRange = -10000;
    public int seedMaxRange = 10000; 
    // perlin parameters and its randomization treshold
    public float treePerlinTreshold =  0.2f;
    public float treeRandomTreshhold = 0.4f;
    public float rockPerlinTreshold = 0.6f;
    public float rockRandomTreshold = 0.3f;

    void Start()
    {
        GenerateProps();
    }

    void GenerateProps()
    {
        // seeds to randomize procedural generation
        int seed = Random.Range(seedMinRange, seedMaxRange);
        Random.InitState(seed);

        int width = gameGrid.width;
        int height = gameGrid.height;

        foreach (var kvp in gameGrid.grid)
        {
            int x = kvp.Key.x;
            int y = kvp.Key.y;

            // ignores the area at the center of the grid
            if (CenterTiles(x, y, width, height))
            {
                continue;
            }

            float xCoord = (float)x / width;
            float yCoord = (float)y / height;

            float perlinValue = Mathf.PerlinNoise(xCoord + seed, yCoord + seed);

            // generate trees
            if (perlinValue > treePerlinTreshold && Random.value < treeRandomTreshhold)
            {
                GameObject tree = Instantiate(treePrefab, new Vector3(x, y, 1), Quaternion.identity);
                tree.name = $"Tree ({x},{y})";
                tree.transform.parent = transform; 

                tree.layer = LayerMask.NameToLayer("Obstacles");

                // updates walkable data of coordinates that the trees are generated
                Walkable walkableComponent = tree.GetComponent<Walkable>();
                if (walkableComponent != null)
                {
                    gameGrid.grid[new Vector2Int(x, y)].walkable = walkableComponent.walkable;
                }
                
                //Debug.Log("Node initialized at position (" + x + ", " + y + ") with walkable status changed to: " + walkableComponent.walkable);
                Buildable buildableComponent = tree.GetComponent<Buildable>();
                if (buildableComponent != null)
                {
                    gameGrid.grid[new Vector2Int(x, y)].buildable = buildableComponent.buildable;
                }
                //Debug.Log("Tree Node initialized at position (" + x + ", " + y + ") with buildable status changed to: " + buildableComponent.buildable);
            }
            // generates rocks
            else if (perlinValue > rockPerlinTreshold && Random.value < rockRandomTreshold)
            {
                GameObject rock = Instantiate(rockPrefab, new Vector3(x, y, 1), Quaternion.identity);
                rock.name = $"Rock ({x},{y})";
                rock.transform.parent = transform;

                rock.layer = LayerMask.NameToLayer("Obstacles");

                // updates walkable data of coordinates that the rocks are generated
                Walkable walkableComponent = rock.GetComponent<Walkable>();
                if (walkableComponent != null)
                {
                    gameGrid.grid[new Vector2Int(x, y)].walkable = walkableComponent.walkable;
                }
                //Debug.Log("Node initialized at position (" + x + ", " + y + ") with walkable status changed to: " + walkableComponent.walkable);
                
                Buildable buildableComponent = rock.GetComponent<Buildable>();
                if (buildableComponent != null)
                {
                    gameGrid.grid[new Vector2Int(x, y)].buildable = buildableComponent.buildable;
                }
                //Debug.Log("Rock Node initialized at position (" + x + ", " + y + ") with buildable status changed to: " + buildableComponent.buildable);
            }
            else
            {
                // nothing is generated, empty space
            }
        }
    }

    //determines the area to avoid instantiating gameobjects
    bool CenterTiles(int x, int y, int width, int height)
    {
        int centerX = width / 2;
        int centerY = height / 2;

        int distanceX = Mathf.Abs(x - centerX);
        int distanceY = Mathf.Abs(y - centerY);

        return (distanceX <= 1 && distanceY <= 1) || (x == centerX && distanceY == 2) || (distanceX == 2 && y == centerY);
    }
}