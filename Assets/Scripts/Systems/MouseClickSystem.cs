using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MouseClickSystem : MonoBehaviour
{
    // instance
    public static MouseClickSystem instance;
    // gameobjects
    private List<GameObject> spawnPoints;
    private List<GameObject> targetPoints;
    public GameObject towersParent;
    public GameObject highlightPrefab;
    public GameObject circlePrefab;
    public TMP_Text detailButtonText;
    private GameObject currentPrefab;
    private GameObject currentSilhouettePrefab;
    private GameObject selectedTower;
    private GameObject selectionCircle;
    
    // script references
    public GameGrid gameGrid;
    public GridVisualizer gridVisualizer;
    public GoldSystem goldSystem;
    private TowerStatsUI statsUI;
    private TowerStats currentStats;
    public CameraControl screenShake;
    // parameters
    private int currentPrefabCost;
    private int removalCost = 0;
    private bool isActive = false;
    public bool isDeletingProps = false;
    private bool isSelectingTower = false; // not used but declared in case needed
    public global::System.Boolean IsSelectingTower { get => isSelectingTower; set => isSelectingTower = value; } 
    // for prop removal handling
    private GameObject lastHighlightedProp; // to track of the highlighted prop (tree/rock)
    private SpriteRenderer lastHighlightedSpriteRenderer;
    private Color originalColor; // for reverting color change
    private Color highlightColor = Color.red; //color change for visualization to the player
    // upgrade parameters
    [SerializeField] private int stoneDamageUpgrade = 5;
    [SerializeField] private int iceDamageUpgrade = 3;
    [SerializeField] private int fireDamageUpgrade = 10;
    [SerializeField] private int shockDamageUpgrade = 3;
    [SerializeField] private int spikeDamageUpgrade = 5;
    [SerializeField] private int megaDamageUpgrade = 25;
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {   
        //ensure script references are assigned
        if (gameGrid != null)
        {
            gameGrid.GridChanged += OnGridChanged;
        }
        else
        {
            Debug.LogError("GameGrid reference is not assigned. Ensure to assign it in the inspector.");
        }

        if (goldSystem == null)
        {
            goldSystem = FindFirstObjectByType<GoldSystem>();  
        }

        if (towersParent == null)
        {
            towersParent = new GameObject("Towers");
        }
        statsUI = FindFirstObjectByType<TowerStatsUI>();
        if (statsUI == null)
        {
            Debug.LogError("TowerStatsUI reference is not assigned.");
        }
        else
        {
            Debug.Log("TowerStatsUI successfully assigned.");
        }
        //find relevant spawn and target points
        FindSpawnAndTargetPoints();
    }

    void OnGridChanged()
    {
        Debug.Log("Grid changed event received.");
        FindSpawnAndTargetPoints();
    }

    public void SetPlacementPrefab(GameObject prefab, GameObject silhouettePrefab, int cost)
    {
        if (prefab != null)
        {
            DestroySilhouette();
            currentPrefab = prefab;
            currentSilhouettePrefab = silhouettePrefab;
            currentPrefabCost = cost;
            isActive = true;
            IsSelectingTower = false;
            CreateSilhouette();
            DeselectTower(); // Cancel any current selection when entering placement mode
        }
        else
        {
            Debug.LogWarning("No prefab selected for placement.");
            currentPrefab = null;
            currentSilhouettePrefab = null; 
            currentPrefabCost = 0;
            isActive = false;
            DestroySilhouette();
        }
    }
    public void SetPropRemovalCost(int value)
    {
        removalCost = value;
        Debug.Log("Prop removal cost set to: " + removalCost);
    }
    void CreateSilhouette()
    { 
        if (currentSilhouettePrefab != null)
        {
            currentSilhouettePrefab = Instantiate(currentSilhouettePrefab);
            UpdateSilhouetteColor(Color.red);
        }
    }

    void DestroySilhouette()
    {
        if (currentSilhouettePrefab != null)
        {
            Destroy(currentSilhouettePrefab);
            currentSilhouettePrefab = null;
        }
    }

    void UpdateSilhouettePosition(Vector3 position)
    {
        if (currentSilhouettePrefab != null)
        {
            Vector2Int gridSize = new Vector2Int(gameGrid.width, gameGrid.height);

            float minX = 0.5f;
            float maxX = gridSize.x - 0.5f;
            float minY = 0.5f; 
            float maxY = gridSize.y - 0.5f; 

            float clampedX = Mathf.Clamp(position.x, minX, maxX);
            float clampedY = Mathf.Clamp(position.y, minY, maxY);
            Vector3 clampedPosition = new Vector3(clampedX, clampedY, 0);

            Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(clampedPosition.x), Mathf.RoundToInt(clampedPosition.y));
            Vector3 snappedPosition = new Vector3(gridPosition.x, gridPosition.y, 0);

            currentSilhouettePrefab.transform.position = snappedPosition;     
        }
    }

    void UpdateSilhouetteColor(Color color)
    {
        if (currentSilhouettePrefab != null)
        {
            Renderer[] renderers = currentSilhouettePrefab.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
            }
        }
    }
    void Update()
    {
        if (isDeletingProps)
        {
            HandlePropsDeletionMode();
            return;
        }
        if (isActive)
        {
            HandlePlacementMode();
            return;
        }
        HandleTowerSelectionMode();
    }

    void HandlePlacementMode()
    {
        isSelectingTower = false;
        isDeletingProps = false; 
        
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            UpdateSilhouettePosition(mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (currentPrefab == null)
                {
                    Debug.LogWarning("No prefab selected for placement.");
                    return;
                }

                if (gameGrid.IsWithinGridBounds(gridPosition))
                {
                    Node node = gameGrid.GetNode(gridPosition.x, gridPosition.y);

                    if (node.buildable && IsPathPossible(gridPosition))
                    {
                        if (!goldSystem.SpendGold(currentPrefabCost))
                        {
                            Debug.LogWarning("Not enough gold to place the tower.");
                            screenShake.TriggerShake();
                            return;
                        }

                        node.walkable = false;
                        GameObject placedObject = Instantiate(currentPrefab, new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity, towersParent.transform);

                        placedObject.GetComponent<Buildable>().buildable = false; 
                        placedObject.GetComponent<Walkable>().walkable = false;
                        node.buildable = false;
                        node.walkable = false;
                        isActive = false;
                        detailButtonText.text = " ";
                        DestroySilhouette();
                    }
                    else
                    {
                        screenShake.TriggerShake();
                        Debug.LogWarning("Cannot place object here as it would block the path or the location is invalid.");
                    }
                }
                else
                {
                    screenShake.TriggerShake();
                    Debug.LogWarning("Cannot place object at this location. Node is outside grid bounds.");
                }
            }
            else if (Input.GetMouseButtonDown(1)) 
            {
                isActive = false;
                DestroySilhouette();
                detailButtonText.text = " ";
            }

            if (gameGrid.IsWithinGridBounds(gridPosition))
            {
                Node node = gameGrid.GetNode(gridPosition.x, gridPosition.y);
                if (node.buildable && IsPathPossible(gridPosition))
                {
                    UpdateSilhouetteColor(Color.green);
                }
                else
                {
                    UpdateSilhouetteColor(Color.red);
                }
            }
            else
            {
                UpdateSilhouetteColor(Color.red);
            }
        }
    }

    void HandleTowerSelectionMode()
    {
        isActive = false;
        isDeletingProps = false;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));

            GameObject clickedTower = GetTowerAtPosition(gridPosition);
            if (clickedTower != null)
            {
                SelectTower(clickedTower);
            }
            else
            {
                DeselectTower();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            DeselectTower();
        }
    }

    void HandlePropsDeletionMode()
    {
        isActive = false;
        DestroyHighlight();

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y));
        
        GameObject hoveredProp = GetPropAtPosition(gridPosition);
        if (hoveredProp != lastHighlightedProp)
        {
            DestroyPropRemovalHighlight();
            if (hoveredProp != null)
            {
                PropRemovalHighlight(hoveredProp);
            }
        }
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            
            if (gameGrid.IsWithinGridBounds(gridPosition) && hoveredProp != null)
            {
                if (!goldSystem.SpendGold(removalCost))
                {
                    Debug.LogWarning("Not enough gold to remove tree/rock");
                    screenShake.TriggerShake();
                    return;
                }
                Node node = gameGrid.GetNode(gridPosition.x, gridPosition.y);
                
                node.walkable = true;
                node.buildable = true;

                DestroyPropRemovalHighlight();
                Destroy(hoveredProp);
                
                isDeletingProps = false;
                detailButtonText.text = " ";
            }
            else
            {
                screenShake.TriggerShake();
                //detailButtonText.text = "No tree/rock found. Try again";
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            isDeletingProps = false;
            detailButtonText.text = " ";
        }
    }
    void PropRemovalHighlight(GameObject prop)
    {
        lastHighlightedProp = prop;
        lastHighlightedSpriteRenderer = prop.GetComponent<SpriteRenderer>();
        if (lastHighlightedSpriteRenderer != null)
        {
            originalColor = lastHighlightedSpriteRenderer.color;
            lastHighlightedSpriteRenderer.color = highlightColor;
        }
    }
    void DestroyPropRemovalHighlight()
    {
        if (lastHighlightedSpriteRenderer != null)
        {
            lastHighlightedSpriteRenderer.color = originalColor;
            lastHighlightedSpriteRenderer = null;
        }
        lastHighlightedProp = null;
    }
    void SelectTower(GameObject tower)
    {
        if (selectedTower != null && selectedTower != tower)
        {
            DeselectTower();
        }
        selectedTower = tower;
        ShowSelectionCircle(); 
        ShowHighlight();
        IsSelectingTower = true;

        BasicTowerAI basicTowerAI = tower.GetComponent<BasicTowerAI>();
        IceTowerAI iceTowerAI = tower.GetComponent<IceTowerAI>();
        FireTowerAI fireTowerAI = tower.GetComponent<FireTowerAI>();
        ShockTowerAI shockTowerAI = tower.GetComponent<ShockTowerAI>();
        SpikeTowerAI spikeTowerAI = tower.GetComponent<SpikeTowerAI>();
        MegaTowerAI megaTowerAI = tower.GetComponent<MegaTowerAI>();

        TowerStats currentStats = null;
        TowerStats upgradedStats = null;
        Sprite towerSprite = tower.GetComponent<SpriteRenderer>().sprite;

        if (basicTowerAI != null)
        {
            currentStats = new TowerStats
            {
                AttackCooldown = basicTowerAI.attackCooldown,
                ProjectileDamage = basicTowerAI.projectileDamage,
                ProjectileLifetime = basicTowerAI.projectileLifetime,
                Level = basicTowerAI.level,
                BaseUpgradeCost = basicTowerAI.baseUpgradeCost,
                RefundCost = basicTowerAI.refundCost,
                UpgradeCostMultiplier = basicTowerAI.upgradeCostMultiplier
            };

            upgradedStats = new TowerStats
            {
                AttackCooldown = currentStats.AttackCooldown,
                ProjectileDamage = currentStats.ProjectileDamage + stoneDamageUpgrade,
                ProjectileLifetime = currentStats.ProjectileLifetime,
                Level = currentStats.Level + 1,
                BaseUpgradeCost = currentStats.BaseUpgradeCost,
                RefundCost = currentStats.RefundCost,
                UpgradeCostMultiplier = currentStats.UpgradeCostMultiplier
            };
        }
        else if (iceTowerAI != null)
        {
            currentStats = new TowerStats
            {
                AttackCooldown = iceTowerAI.attackCooldown,
                ProjectileDamage = iceTowerAI.projectileDamage,
                ProjectileLifetime = iceTowerAI.projectileLifetime,
                Level = iceTowerAI.level,
                BaseUpgradeCost = iceTowerAI.baseUpgradeCost,
                RefundCost = iceTowerAI.refundCost,
                UpgradeCostMultiplier = iceTowerAI.upgradeCostMultiplier
            };

            upgradedStats = new TowerStats
            {
                AttackCooldown = currentStats.AttackCooldown,
                ProjectileDamage = currentStats.ProjectileDamage + iceDamageUpgrade,
                ProjectileLifetime = currentStats.ProjectileLifetime,
                Level = currentStats.Level + 1,
                BaseUpgradeCost = currentStats.BaseUpgradeCost,
                RefundCost = currentStats.RefundCost,
                UpgradeCostMultiplier = currentStats.UpgradeCostMultiplier
            };
        }
        else if (fireTowerAI != null)
        {
            currentStats = new TowerStats
            {
                AttackCooldown = fireTowerAI.attackCooldown,
                ProjectileDamage = fireTowerAI.projectileDamage,
                ProjectileLifetime = fireTowerAI.projectileLifetime,
                Level = fireTowerAI.level,
                BaseUpgradeCost = fireTowerAI.baseUpgradeCost,
                RefundCost = fireTowerAI.refundCost,
                UpgradeCostMultiplier = fireTowerAI.upgradeCostMultiplier
            };

            upgradedStats = new TowerStats
            {
                AttackCooldown = currentStats.AttackCooldown,
                ProjectileDamage = currentStats.ProjectileDamage + fireDamageUpgrade,
                ProjectileLifetime = currentStats.ProjectileLifetime,
                Level = currentStats.Level + 1,
                BaseUpgradeCost = currentStats.BaseUpgradeCost,
                RefundCost = currentStats.RefundCost,
                UpgradeCostMultiplier = currentStats.UpgradeCostMultiplier
            };
        }
        else if (shockTowerAI != null)
        {
            currentStats = new TowerStats
            {
                AttackCooldown = shockTowerAI.attackCooldown,
                ProjectileDamage = shockTowerAI.projectileDamage,
                ProjectileLifetime = shockTowerAI.projectileLifetime,
                Level = shockTowerAI.level,
                BaseUpgradeCost = shockTowerAI.baseUpgradeCost,
                RefundCost = shockTowerAI.refundCost,
                UpgradeCostMultiplier = shockTowerAI.upgradeCostMultiplier
            };

            upgradedStats = new TowerStats
            {
                AttackCooldown = currentStats.AttackCooldown,
                ProjectileDamage = currentStats.ProjectileDamage + shockDamageUpgrade,
                ProjectileLifetime = currentStats.ProjectileLifetime,
                Level = currentStats.Level + 1,
                BaseUpgradeCost = currentStats.BaseUpgradeCost,
                RefundCost = currentStats.RefundCost,
                UpgradeCostMultiplier = currentStats.UpgradeCostMultiplier
            };
        }
        else if (spikeTowerAI != null)
        {
            currentStats = new TowerStats
            {
                AttackCooldown = spikeTowerAI.attackCooldown,
                ProjectileDamage = spikeTowerAI.projectileDamage,
                ProjectileLifetime = spikeTowerAI.projectileLifetime,
                Level = spikeTowerAI.level,
                BaseUpgradeCost = spikeTowerAI.baseUpgradeCost,
                RefundCost = spikeTowerAI.refundCost,
                UpgradeCostMultiplier = spikeTowerAI.upgradeCostMultiplier
            };

            upgradedStats = new TowerStats
            {
                AttackCooldown = currentStats.AttackCooldown,
                ProjectileDamage = currentStats.ProjectileDamage + spikeDamageUpgrade,
                ProjectileLifetime = currentStats.ProjectileLifetime,
                Level = currentStats.Level + 1,
                BaseUpgradeCost = currentStats.BaseUpgradeCost,
                RefundCost = currentStats.RefundCost,
                UpgradeCostMultiplier = currentStats.UpgradeCostMultiplier
            };
        }
        else if (megaTowerAI != null)
        {
            currentStats = new TowerStats
            {
                AttackCooldown = megaTowerAI.attackCooldown,
                ProjectileDamage = megaTowerAI.projectileDamage,
                ProjectileLifetime = megaTowerAI.projectileLifetime,
                Level = megaTowerAI.level,
                BaseUpgradeCost = megaTowerAI.baseUpgradeCost,
                RefundCost = megaTowerAI.refundCost,
                UpgradeCostMultiplier = megaTowerAI.upgradeCostMultiplier
            };

            upgradedStats = new TowerStats
            {
                AttackCooldown = currentStats.AttackCooldown,
                ProjectileDamage = currentStats.ProjectileDamage + megaDamageUpgrade,
                ProjectileLifetime = currentStats.ProjectileLifetime,
                Level = currentStats.Level + 1,
                BaseUpgradeCost = currentStats.BaseUpgradeCost,
                RefundCost = currentStats.RefundCost,
                UpgradeCostMultiplier = currentStats.UpgradeCostMultiplier
            };
        }
        

        if (currentStats != null && upgradedStats != null)
        {
            int currentGold = goldSystem.currentGold;
            int upgradeCost = Mathf.RoundToInt(currentStats.BaseUpgradeCost * Mathf.Pow(currentStats.UpgradeCostMultiplier, currentStats.Level));
            statsUI.Show(currentStats, upgradedStats, towerSprite, currentGold);
        }
    }
    void DeselectTower()
    {
        selectedTower = null;
        if (selectionCircle != null)
        {
            Destroy(selectionCircle);
            DestroyHighlight();
            selectionCircle = null;
        }
        IsSelectingTower = false; 
        statsUI.HideUI();
    }

    GameObject GetTowerAtPosition(Vector2Int gridPosition)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition2D, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Tower"))
            {
                // Check if the collider is a BoxCollider2D, excluding the circle collider
                if (hit.collider is BoxCollider2D && !(hit.collider is CircleCollider2D))
                {
                    return hit.collider.gameObject;
                }
            }
        }

        return null;
    }

    GameObject GetPropAtPosition(Vector2Int gridPosition)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition2D, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Prop"))
            {
                // Check if the collider is a BoxCollider2D, excluding the circle collider
                if (hit.collider is BoxCollider2D && !(hit.collider is CircleCollider2D))
                {
                    return hit.collider.gameObject;
                }
            }
        }

        return null;
    }

    void ShowHighlight()
    {
        if (highlightPrefab != null && selectedTower != null)
        {
            GameObject highlight = Instantiate(highlightPrefab, selectedTower.transform.position, Quaternion.identity);
            highlight.transform.SetParent(selectedTower.transform);
        }
    }

    void DestroyHighlight()
    {
        // Find and destroy any instantiated highlight prefab
        GameObject[] highlights = GameObject.FindGameObjectsWithTag("Highlight");
        foreach (GameObject highlight in highlights)
        {
            Destroy(highlight);
        }
    }

    void ShowSelectionCircle()
    {
        if (selectedTower == null) return;

        CircleCollider2D circleCollider = selectedTower.GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            if (selectionCircle != null)
            {
                Destroy(selectionCircle);
            }

            selectionCircle = Instantiate(circlePrefab, selectedTower.transform.position, Quaternion.identity);
            selectionCircle.transform.localScale = new Vector3(circleCollider.radius * 2, circleCollider.radius * 2, 1);
        }
    }


    void FindSpawnAndTargetPoints()
    {
        spawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Spawn"));
        targetPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Target"));
    }

    bool IsPathPossible(Vector2Int towerPosition)
    {
        if (spawnPoints == null || targetPoints == null || spawnPoints.Count == 0 || targetPoints.Count == 0)
        {
            return false;
        }

        Pathfinding pathfinding = new Pathfinding(gameGrid);
        
        foreach (GameObject spawn in spawnPoints)
        {
            Vector2Int spawnPosition = new Vector2Int(Mathf.RoundToInt(spawn.transform.position.x), Mathf.RoundToInt(spawn.transform.position.y));

            foreach (GameObject target in targetPoints)
            {
                Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(target.transform.position.x), Mathf.RoundToInt(target.transform.position.y));
                
                // temporarily makes node unwalkable
                Node towerNode = gameGrid.GetNode(towerPosition.x, towerPosition.y);
                towerNode.walkable = false;

                List<Node> path = pathfinding.FindPath(spawnPosition, targetPosition);
                
                // makes node walkable again
                towerNode.walkable = true;

                if (path == null || path.Count == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void ApplyUpgrade()
    {
        if (selectedTower != null)
        {
            BasicTowerAI basicTowerAI = selectedTower.GetComponent<BasicTowerAI>();
            IceTowerAI iceTowerAI = selectedTower.GetComponent<IceTowerAI>();
            FireTowerAI fireTowerAI = selectedTower.GetComponent<FireTowerAI>();
            ShockTowerAI shockTowerAI = selectedTower.GetComponent<ShockTowerAI>();
            SpikeTowerAI spikeTowerAI = selectedTower.GetComponent<SpikeTowerAI>();
            MegaTowerAI megaTowerAI = selectedTower.GetComponent<MegaTowerAI>();

            if (basicTowerAI != null)
            {
                TowerStats currentStats = new TowerStats
                {
                    Level = basicTowerAI.level,
                    BaseUpgradeCost = basicTowerAI.baseUpgradeCost,
                    UpgradeCostMultiplier = basicTowerAI.upgradeCostMultiplier,
                    RefundCost = basicTowerAI.refundCost
                };

                int upgradeCost = Mathf.RoundToInt(currentStats.BaseUpgradeCost * Mathf.Pow(currentStats.UpgradeCostMultiplier, currentStats.Level));

                if (goldSystem.SpendGold(upgradeCost))
                {
                    basicTowerAI.projectileDamage += stoneDamageUpgrade;
                    basicTowerAI.level += 1;

                    SelectTower(selectedTower);
                }
                else
                {
                    screenShake.TriggerShake();
                    Debug.LogWarning("Not enough gold to upgrade the tower.");
                }
            }
            else if (iceTowerAI != null)
            {
                TowerStats currentStats = new TowerStats
                {
                    Level = iceTowerAI.level,
                    BaseUpgradeCost = iceTowerAI.baseUpgradeCost,
                    UpgradeCostMultiplier = iceTowerAI.upgradeCostMultiplier,
                    RefundCost = iceTowerAI.refundCost
                };

                int upgradeCost = Mathf.RoundToInt(currentStats.BaseUpgradeCost * Mathf.Pow(currentStats.UpgradeCostMultiplier, currentStats.Level));

                if (goldSystem.SpendGold(upgradeCost))
                {
                    iceTowerAI.projectileDamage += iceDamageUpgrade;
                    iceTowerAI.level += 1;

                    SelectTower(selectedTower);
                }
                else
                {
                    screenShake.TriggerShake();
                    Debug.LogWarning("Not enough gold to upgrade the tower.");
                }
            }
            else if (fireTowerAI != null)
            {
                TowerStats currentStats = new TowerStats
                {
                    Level = fireTowerAI.level,
                    BaseUpgradeCost = fireTowerAI.baseUpgradeCost,
                    UpgradeCostMultiplier = fireTowerAI.upgradeCostMultiplier,
                    RefundCost = fireTowerAI.refundCost
                };

                int upgradeCost = Mathf.RoundToInt(currentStats.BaseUpgradeCost * Mathf.Pow(currentStats.UpgradeCostMultiplier, currentStats.Level));

                if (goldSystem.SpendGold(upgradeCost))
                {
                    fireTowerAI.projectileDamage += fireDamageUpgrade;
                    fireTowerAI.level += 1;

                    SelectTower(selectedTower);
                }
                else
                {
                    screenShake.TriggerShake();
                    Debug.LogWarning("Not enough gold to upgrade the tower.");
                }
            }
            else if (shockTowerAI != null)
            {
                TowerStats currentStats = new TowerStats
                {
                    Level = shockTowerAI.level,
                    BaseUpgradeCost = shockTowerAI.baseUpgradeCost,
                    UpgradeCostMultiplier = shockTowerAI.upgradeCostMultiplier,
                    RefundCost = shockTowerAI.refundCost
                };

                int upgradeCost = Mathf.RoundToInt(currentStats.BaseUpgradeCost * Mathf.Pow(currentStats.UpgradeCostMultiplier, currentStats.Level));

                if (goldSystem.SpendGold(upgradeCost))
                {
                    shockTowerAI.projectileDamage += shockDamageUpgrade;
                    shockTowerAI.level += 1;

                    SelectTower(selectedTower);
                }
                else
                {
                    screenShake.TriggerShake();
                    Debug.LogWarning("Not enough gold to upgrade the tower.");
                }
            }
            else if (spikeTowerAI != null)
            {
                TowerStats currentStats = new TowerStats
                {
                    Level = spikeTowerAI.level,
                    BaseUpgradeCost = spikeTowerAI.baseUpgradeCost,
                    UpgradeCostMultiplier = spikeTowerAI.upgradeCostMultiplier,
                    RefundCost = spikeTowerAI.refundCost
                };

                int upgradeCost = Mathf.RoundToInt(currentStats.BaseUpgradeCost * Mathf.Pow(currentStats.UpgradeCostMultiplier, currentStats.Level));

                if (goldSystem.SpendGold(upgradeCost))
                {
                    spikeTowerAI.projectileDamage += spikeDamageUpgrade;
                    spikeTowerAI.level += 1;

                    SelectTower(selectedTower);
                }
                else
                {
                    screenShake.TriggerShake();
                    Debug.LogWarning("Not enough gold to upgrade the tower.");
                }
            }
            else if (megaTowerAI != null)
            {
                TowerStats currentStats = new TowerStats
                {
                    Level = megaTowerAI.level,
                    BaseUpgradeCost = megaTowerAI.baseUpgradeCost,
                    UpgradeCostMultiplier = megaTowerAI.upgradeCostMultiplier,
                    RefundCost = megaTowerAI.refundCost
                };

                int upgradeCost = Mathf.RoundToInt(currentStats.BaseUpgradeCost * Mathf.Pow(currentStats.UpgradeCostMultiplier, currentStats.Level));

                if (goldSystem.SpendGold(upgradeCost))
                {
                    megaTowerAI.projectileDamage += megaDamageUpgrade;
                    megaTowerAI.level += 1;

                    SelectTower(selectedTower);
                }
                else
                {
                    screenShake.TriggerShake();
                    Debug.LogWarning("Not enough gold to upgrade the tower.");
                }
            }
        }
        else
        {
            screenShake.TriggerShake();
            Debug.LogWarning("No tower selected to upgrade.");
        }
    }

    public void RefundTower()
    {
        if (selectedTower != null)
        {
            BasicTowerAI basicTowerAI = selectedTower.GetComponent<BasicTowerAI>(); 
            FireTowerAI fireTowerAI = selectedTower.GetComponent<FireTowerAI>();
            IceTowerAI iceTowerAI = selectedTower.GetComponent<IceTowerAI>();
            ShockTowerAI shockTowerAI = selectedTower.GetComponent<ShockTowerAI>();
            SpikeTowerAI spikeTowerAI = selectedTower.GetComponent<SpikeTowerAI>();
            MegaTowerAI megaTowerAI = selectedTower.GetComponent<MegaTowerAI>();

            int sellAmount = 0;

            if (basicTowerAI != null)
            {
                sellAmount = Mathf.RoundToInt((basicTowerAI.baseUpgradeCost * Mathf.Pow(basicTowerAI.upgradeCostMultiplier, basicTowerAI.level)) * 0.5f);
            }
            else if (fireTowerAI != null)
            {
                sellAmount = Mathf.RoundToInt((fireTowerAI.baseUpgradeCost * Mathf.Pow(fireTowerAI.upgradeCostMultiplier, fireTowerAI.level)) * 0.5f);
            }
            else if (iceTowerAI != null)
            {
                sellAmount = Mathf.RoundToInt((iceTowerAI.baseUpgradeCost * Mathf.Pow(iceTowerAI.upgradeCostMultiplier, iceTowerAI.level)) * 0.5f);
            }
            else if (shockTowerAI != null)
            {
                sellAmount = Mathf.RoundToInt((shockTowerAI.baseUpgradeCost * Mathf.Pow(shockTowerAI.upgradeCostMultiplier, shockTowerAI.level) * 0.5f));
            }
            else if (spikeTowerAI != null)
            {
                sellAmount = Mathf.RoundToInt((spikeTowerAI.baseUpgradeCost * Mathf.Pow(spikeTowerAI.upgradeCostMultiplier, spikeTowerAI.level)) * 0.5f);
            }
            else if (megaTowerAI != null)
            {
                sellAmount = Mathf.RoundToInt((megaTowerAI.baseUpgradeCost * Mathf.Pow(megaTowerAI.upgradeCostMultiplier, megaTowerAI.level)) * 0.5f);
            }

            goldSystem.AddGold(sellAmount);

            Vector3 towerPosition = selectedTower.transform.position;
            Vector2Int gridCoordinates = gameGrid.WorldToGrid(towerPosition); // Using existing method
            Node node = gameGrid.GetNode(gridCoordinates.x, gridCoordinates.y);
            if (node != null)
            {
                node.buildable = true;
                node.walkable = true;
            }
            else
            {
                Debug.LogError("Node at the given position is null.");
            }

            Destroy(selectedTower);
            DeselectTower();
        }
        else
        {
            Debug.LogWarning("No tower selected to refund.");
        }
    }
}
