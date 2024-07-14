using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingSlimeBehavior : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    public int PlayerLoseHP = 1;
    private GameGrid grid; 
    private LifeSystem lifeSystem;
    private FlyPathfinding flyPathfinding; //for calculating best path
    private float exploreProbability = 0.3f; //higher means more exploration
    private float pheromoneDepositRate = 0.3f; //higher means more deposite
    private float evaporationRate = 0.1f; //higher means faster rate
    private bool isDestroyed = false;
    [SerializeField] float randomThreshold = 0.3f;

    // fields for slow effect
    private float originalMoveSpeed;
    private float slowDuration;
    private float slowPercentage;
    private bool isSlowed = false;
    private Coroutine slowEffectCoroutine;
    // field for freeze effect
    private float freezeDuration;
    private bool isFrozen = false;
    private Coroutine freezeEffectCoroutine;
    private Coroutine currentPathCoroutine;
    // fields for color change
    private Color slowColor = new Color(0.1f, 0.2f, 0.7f, 1.0f);
    private Color freezeColor = new Color(0.1f, 0.2f, 1.0f, 1.0f);
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private enum AgentState
    {
        None,
        Exploring,
        FollowingBestPath
    }
    private AgentState currentState = AgentState.None;
    private bool isExploring = false;
    //
    public delegate void SlimeDestroyedHandler(GameObject slime);
    public static event SlimeDestroyedHandler SlimeDestroyed;

    private void Start()
    {
        originalMoveSpeed = moveSpeed; // Store the original move speed

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found. Color changes will be skipped.");
        }

        grid = FindFirstObjectByType<GameGrid>();
        if (grid == null)
        {
            Debug.LogError("GameGrid not found in the scene!");
            enabled = false; // Disable the script if a critical component is missing
            return;
        }

        lifeSystem = FindFirstObjectByType<LifeSystem>();
        if (lifeSystem == null)
        {
            Debug.LogError("LifeSystem not found.");
            enabled = false; // Disable the script if a critical component is missing
            return;
        }

        flyPathfinding = new FlyPathfinding(grid);
        Debug.Log("Agent initialized.");
        AgentExploreOrBestPath();
    }

    private void OnDestroy()
    {
        SlimeDestroyed?.Invoke(gameObject);
        if (currentPathCoroutine != null)
        {
            StopCoroutine(currentPathCoroutine);
        }
        StopAllCoroutines();
    }

    private void AgentExploreOrBestPath()
    {
        float randomValue = Random.value;

        if (currentPathCoroutine != null)
        {
            StopCoroutine(currentPathCoroutine);
        }

        if (randomValue <= exploreProbability)
        {
            Debug.Log("Agent is exploring...");
            isExploring = true;
            currentPathCoroutine = StartCoroutine(Explore());
        }
        else
        {
            Debug.Log("Agent is moving through best path...");
            isExploring = false;
            currentPathCoroutine = StartCoroutine(CalculateBestPath());
        }
    }
    
    private IEnumerator Explore()
    {
        isExploring = true;
        Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        while (true)
        {
            if (IsTargetPosition(currentPosition))
            {
                HandleReachedTarget();
                yield break;
            }
            List<Node> neighbors = grid.GetNeighbors(grid.GetNode(currentPosition.x, currentPosition.y));
            List<Node> unvisitedNeighbors = new List<Node>();
            foreach (Node neighbor in neighbors)
            {
                if (!neighbor.visited)
                {
                    unvisitedNeighbors.Add(neighbor);
                }
            }

            if (unvisitedNeighbors.Count > 0)
            {
                Node nextNode = null;
                float maxPheromoneLevel = float.MinValue;
                foreach (Node neighbor in unvisitedNeighbors)
                {
                    if (neighbor.pheromoneLevel > maxPheromoneLevel)
                    {
                        maxPheromoneLevel = neighbor.pheromoneLevel;
                        nextNode = neighbor;
                    }
                }
                if (Random.value < randomThreshold)
                {
                    nextNode = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                }

                nextNode.visited = true;
                List<Node> path = flyPathfinding.FindPath(currentPosition, new Vector2Int(nextNode.x, nextNode.y));
                if (path != null && path.Count > 0)
                {
                    yield return StartCoroutine(FollowNextNode(path, new Vector2Int(nextNode.x, nextNode.y))); 
                }
            }
            else
            {
                break;
            }
            currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        }

        GameObject targetObject = GameObject.FindGameObjectWithTag("Target");
        if (targetObject != null && currentPosition == new Vector2Int((int)targetObject.transform.position.x, (int)targetObject.transform.position.y))
        {
            //Destroy(gameObject);
            //lifeSystem.LoseLifePoints(PlayerLoseHP);
            HandleReachedTarget();
        }
        else
        {
            StartCoroutine(CalculateBestPath());
        }
    }

    private IEnumerator CalculateBestPath()
    {
        Vector2Int startPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int targetPosition = GetTargetPosition();
        List<Node> path = flyPathfinding.FindPath(startPosition, targetPosition);

        while (true)
        {
            if (!IsPathValid(path))
            {
                path = flyPathfinding.FindPath(startPosition, targetPosition);
                if (path == null || path.Count == 0)
                {
                    yield return StartCoroutine(HandleBlockedPaths());
                    yield break;
                }
            }
            yield return StartCoroutine(FollowPath(path));

            if (Vector2Int.Distance(new Vector2Int((int)transform.position.x, (int)transform.position.y), targetPosition) < 0.1f)
            {
                HandleReachedTarget();
                yield break;
            }

            if (HasReachedNextNode(path))
            {
                path = flyPathfinding.FindPath(startPosition, targetPosition);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private bool HasReachedNextNode(List<Node> path)
    {
        if (path == null || path.Count == 0)
        {
            return false;
        }

        Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int nextNodePosition = new Vector2Int(path[0].x, path[0].y);
        return currentPosition == nextNodePosition;
    }

    private bool IsPathValid(List<Node> path)
    {
        if (path == null || path.Count == 0)
        {
            return false;
        }

        foreach (Node node in path)
        {
            if (!node.walkable)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator FollowPath(List<Node> path)
    {
        int currentNodeIndex = 0;

        while (path != null && currentNodeIndex < path.Count)
        {
            Node currentNode = path[currentNodeIndex];
            Vector3 targetPosition = new Vector3(currentNode.x, currentNode.y, 0);
            float distance = Vector3.Distance(transform.position, targetPosition);
            float duration = distance / moveSpeed;

            float startTime = Time.time;
            Vector3 startPosition = transform.position;

            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                
                if (isSlowed)
                {
                    duration = distance / (moveSpeed * (1f - slowPercentage));
                    targetPosition = new Vector3(currentNode.x, currentNode.y, 0);
                }
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    break;
                }

                yield return null;
            }

            transform.position = targetPosition;
            DepositPheromoneLevels(new List<Node> { currentNode });

            if (currentNodeIndex == path.Count - 1 && IsTargetPosition(new Vector2Int((int)transform.position.x, (int)transform.position.y)))
            {
                HandleReachedTarget();
                yield break;
            }

            if (HasReachedNextNode(path))
            {
                Vector2Int thisPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
                Vector2Int nextPosition = GetTargetPosition();
                path = flyPathfinding.FindPath(thisPosition, nextPosition);
                currentNodeIndex = 0;
            }
            else
            {
                currentNodeIndex++;
            }
        }
    }

    private IEnumerator FollowNextNode(List<Node> path, Vector2Int nextNodePosition)
    {
        foreach (Node node in path)
        {
            Vector3 targetPosition = new Vector3(node.x, node.y, 0);
            float distance = Vector3.Distance(transform.position, targetPosition);
            float duration = distance / moveSpeed;

            float startTime = Time.time;
            Vector3 startPosition = transform.position;

            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                if (isSlowed)
                {
                    duration = distance / (moveSpeed * (1f - slowPercentage));
                    targetPosition = new Vector3(nextNodePosition.x, nextNodePosition.y, 0);
                }
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            transform.position = targetPosition;
            DepositPheromoneLevels(new List<Node> { node });

            if (transform.position.x == nextNodePosition.x && transform.position.y == nextNodePosition.y)
            {
                yield break;
            }
        }
    }

    private Vector2Int GetTargetPosition()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag("Target");
        if (targetObject != null)
        {
            return new Vector2Int((int)targetObject.transform.position.x, (int)targetObject.transform.position.y);
        }
        else
        {
            Debug.LogWarning("Target object not found!");
            return Vector2Int.zero;
        }
    }

    private void DepositPheromoneLevels(List<Node> path)
    {
        grid.DepositPheromones(path, pheromoneDepositRate);
    }

    private bool IsTargetPosition(Vector2Int position)
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag("Target");
        if (targetObject != null)
        {
            Vector2Int targetPosition = new Vector2Int((int)targetObject.transform.position.x, (int)targetObject.transform.position.y);
            return position == targetPosition;
        }
        else
        {
            Debug.LogWarning("Target object not found!");
            return false;
        }
    }

    private IEnumerator HandleBlockedPaths()
    {
        Vector2Int startPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int targetPosition = GetTargetPosition();
        List<Node> newPath = flyPathfinding.FindPath(startPosition, targetPosition);

        if (newPath != null && newPath.Count > 0)
        {
            yield return StartCoroutine(FollowPath(newPath));
        }
        else
        {
            Debug.Log("All paths towards the target are still blocked even after attempting to remove an obstacle. Implementing further alternative actions.");
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(HandleBlockedPaths());
        }
    }

    private void HandleReachedTarget()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            lifeSystem.LoseLifePoints(PlayerLoseHP);
            SlimeDestroyed?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }
    public void ApplySlow(float slowPercentage, float slowDuration)
    {
        if (slowEffectCoroutine != null)
        {
            StopCoroutine(slowEffectCoroutine);
        }
        slowEffectCoroutine = StartCoroutine(SlowEffectCoroutine(slowPercentage, slowDuration));
    }
    private IEnumerator SlowEffectCoroutine(float slowPercentage, float slowDuration)
    {
        isSlowed = true;
        moveSpeed = originalMoveSpeed * (1f - slowPercentage);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = slowColor;
        }
        yield return new WaitForSeconds(slowDuration);

        RemoveSlowEffect();
    }

    private void RemoveSlowEffect()
    {
        if (isSlowed)
        {
            isSlowed = false;
            moveSpeed = originalMoveSpeed;
            if (slowEffectCoroutine != null)
            {
                StopCoroutine(slowEffectCoroutine);
                slowEffectCoroutine = null;
            }
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    internal void ApplySlow(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyFreeze(float freezeDuration)
    {
        if (currentPathCoroutine != null)
        {
            StopCoroutine(currentPathCoroutine);
        }
        currentState = isExploring ? AgentState.Exploring : AgentState.FollowingBestPath;
        freezeEffectCoroutine = StartCoroutine(FreezeEffectCoroutine(freezeDuration));
    }

    private IEnumerator FreezeEffectCoroutine(float freezeDuration)
    {
        isFrozen = true;
        moveSpeed = 0f; // Set movement speed to zero to stop the slime

        if (spriteRenderer != null)
        {
            spriteRenderer.color = freezeColor;
        }
        yield return new WaitForSeconds(freezeDuration);

        RemoveFreezeEffect();
    }

    private void RemoveFreezeEffect()
    {
        if (isFrozen)
        {
            isFrozen = false;
            moveSpeed = originalMoveSpeed;
            if (freezeEffectCoroutine != null)
            {
                StopCoroutine(freezeEffectCoroutine);
                freezeEffectCoroutine = null;
            }
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            ResumePreviousState();
        }
    }

    private void ResumePreviousState()
    {
        switch (currentState)
        {
            case AgentState.Exploring:
                currentPathCoroutine = StartCoroutine(Explore());
                break;
            case AgentState.FollowingBestPath:
                currentPathCoroutine = StartCoroutine(CalculateBestPath());
                break;
            default:
                AgentExploreOrBestPath(); // fallback to first action
                break;
        }
    }
}
