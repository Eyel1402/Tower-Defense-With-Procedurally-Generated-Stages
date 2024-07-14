using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public GameObject redHighlight;
    public GameObject blueHighlight;
    public GameGrid gameGrid;

    void Start()
    {
        // Check if gameGrid is assigned to the gameobject
        if (gameGrid == null)
        {
            Debug.LogError("GameGrid reference is not assigned.");
        }

        //VisualizeGrid();
    }
    public void UpdateVisual()
    {
        Debug.Log("Grid changed. Updating visualization...");
        //VisualizeGrid();
    }
    void VisualizeGrid()
    {
        // Check if gameGrid is assigned
        if (gameGrid == null)
        {
            Debug.LogError("GameGrid reference is not assigned.");
            return;
        }

        // Check if grid data is initialized
        if (gameGrid.grid == null)
        {
            Debug.LogError("Grid dictionary is not initialized.");
            return;
        }

        // Check if prefabs are assigned
        if (redHighlight == null || blueHighlight == null)
        {
            Debug.LogError("Red or blue highlight prefab is not assigned.");
            return;
        }

        // Prevent duplication of game objects
        DestroyExistingHighlights();
        
        GameObject redParent = GameObject.Find("Red");
        if (redParent == null)
        {
            redParent = new GameObject("Red");
        }
        GameObject blueParent = GameObject.Find("Blue");
        if (blueParent == null)
        {
            blueParent = new GameObject("Blue");
        }

        foreach (var kvp in gameGrid.grid)
        {
            Vector2Int gridPosition = kvp.Key;
            Node node = kvp.Value;

            Vector3 position = new Vector3(gridPosition.x, gridPosition.y, 1);

            if (node.walkable)
            {
                GameObject blueObj = Instantiate(blueHighlight, position, Quaternion.identity, blueParent.transform);
                blueObj.name = "Blue (" + gridPosition.x + ", " + gridPosition.y + ")";
            }
            else
            {
                GameObject redObj = Instantiate(redHighlight, position, Quaternion.identity, redParent.transform);
                redObj.name = "Red (" + gridPosition.x + ", " + gridPosition.y + ")";
            }
        }

        Debug.Log("Grid visualization updated.");
    }

    void DestroyExistingHighlights()
    {
        GameObject[] existingHighlights = GameObject.FindGameObjectsWithTag("highlight");

        foreach (GameObject highlight in existingHighlights)
        {
            Destroy(highlight);
        }
    }

}
