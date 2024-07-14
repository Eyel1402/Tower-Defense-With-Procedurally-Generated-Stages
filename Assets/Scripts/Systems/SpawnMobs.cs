using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SlimeSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SlimeType
    {
        public GameObject prefab;
        public float spawnProbability; // Probability (0 to 1) to spawn this slime
    }

    public List<SlimeType> slimeTypes; // Prefab of the Slime agent
    public SlimeType bossSlime; // Boss Slime prefab
    public int initialSlimesPerWave = 10; // Initial number of slimes to spawn per wave
    public float initialWaveTimer = 15.0f;
    public float spawnInterval = 1.0f; // Interval between each slime spawn within a wave
    public float waveInterval = 5.0f; // Default interval between each wave
    public TextMeshProUGUI waveNumberText; // Reference to the TextMesh Pro UI element for wave number
    public TextMeshProUGUI waveIntervalText; // Reference to the TextMesh Pro UI element for wave interval
    public WinUIManager winUIManager; // Reference to the WinUIManager
    public LifeSystem lifeSystem; // Reference to the LifeSystem

    private Transform[] spawnPoints; // Array to store spawn points
    private int waveNumber = 0; // Tracks the current wave number
    public int WaveNumber => waveNumber;
    private List<GameObject> activeSlimes = new List<GameObject>(); // List to keep track of active slimes
    private int slimesPerWave; // Number of slimes to spawn per wave
    private int maxWaves; // Maximum number of waves based on the game mode
    private int bossCount = 0; // Tracks the number of bosses spawned per wave
    public float goldMultiplier = 0.1f; // Tracks gold multiplier
    public float healthIncreaseRate = 0.1f;
    private const int maxSlimesLimit = 300; // Maximum number of active slimes allowed

    void Start()
    {
        // Find all GameObjects with the "Spawn" tag and store their transforms in the spawnPoints array
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("Spawn");
        spawnPoints = new Transform[spawnPointObjects.Length];
        
        for (int i = 0; i < spawnPointObjects.Length; i++)
        {
            spawnPoints[i] = spawnPointObjects[i].transform;
        }

        // Initialize slimesPerWave
        slimesPerWave = initialSlimesPerWave;

        // Get the game mode from PlayerPrefs
        maxWaves = PlayerPrefs.GetInt("GameMode", 0); // 0 for endless mode

        // Start spawning waves of slimes
        StartCoroutine(StartInitialCountdown(initialWaveTimer));

        // Subscribe to the SlimeDestroyed event from AgentBehavior
        AgentBehavior.SlimeDestroyed += OnSlimeDestroyed;
    }

    private IEnumerator StartInitialCountdown(float initialInterval)
    {
        float countdown = initialInterval;
        while (countdown > 0)
        {
            UpdateWaveIntervalText(countdown);
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }
        UpdateWaveIntervalText(0);
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (maxWaves == 0 || waveNumber < maxWaves) // Continue spawning waves until the max waves is reached or indefinitely if maxWaves is 0 (endless mode)
        {
            waveNumber++; // Increment the wave number
            UpdateWaveNumberText(); // Update the wave number display

            // Adjust the wave interval based on the wave number
            float currentWaveInterval = (waveNumber % 5 == 0) ? 10.0f : waveInterval;

            // Adjust the number of slimes per wave based on the wave number
            slimesPerWave = Mathf.CeilToInt(initialSlimesPerWave * Mathf.Pow(1.035f, waveNumber - 1));

            // Increment boss count every 5 waves, up to a maximum of 5
            if (waveNumber % 5 == 0)
            {
                bossCount = Mathf.Min(bossCount + 1, 5);
            }

            // Increase gold multiplier every 10 waves and player HP
            if (waveNumber % 10 == 0)
            {
                goldMultiplier *= 1.1f; //INCREASE GOLD RECEIVE
                lifeSystem.AddLifePoints(5); // Add 5 HP every 10 waves
            }

            // Spawn slimes for the current wave
            yield return StartCoroutine(SpawnSlimesWithInterval());

            // Spawn bosses only every 5 waves
            if (waveNumber % 5 == 0)
            {
                for (int i = 0; i < bossCount; i++)
                {
                    SpawnSlime(bossSlime.prefab, ChooseRandomSpawnPoint());
                }
            }

            // Start hidden timer of 30 seconds
            yield return StartCoroutine(HiddenTimer(30.0f));

            // Wait until all slimes are destroyed before starting the next wave
            yield return new WaitUntil(() => activeSlimes.Count == 0);

            // Start countdown for the next wave
            yield return StartCoroutine(StartWaveCountdown(currentWaveInterval));
        }

        // Handle end of the game for non-endless mode
        if (maxWaves != 0 && waveNumber >= maxWaves)
        {
            // Show the win UI
            winUIManager.ShowWinUI(waveNumber);
        }
    }

    private IEnumerator SpawnSlimesWithInterval()
    {
        int slimesSpawned = 0; // Keep track of the number of slimes spawned

        while (slimesSpawned < slimesPerWave)
        {
            // Check if the active slimes count has reached the maximum limit
            if (activeSlimes.Count >= maxSlimesLimit)
            {
                yield return new WaitForSeconds(spawnInterval); // Wait before trying again
                continue;
            }

            // Spawn a slime at each spawn point
            for (int i = 0; i < spawnPoints.Length && slimesSpawned < slimesPerWave; i++)
            {
                // Check again before spawning each slime
                if (activeSlimes.Count >= maxSlimesLimit)
                {
                    break;
                }

                // Spawn a slime at the current spawn point
                var slime = SpawnSlime(ChooseSlimePrefab(), spawnPoints[i].position);
                slimesSpawned++;
            }

            // Wait for the specified interval before spawning the next set of slimes
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator HiddenTimer(float interval)
    {
        float countdown = interval;
        while (countdown > 0)
        {
            // Check if all slimes are destroyed before the timer ends
            if (activeSlimes.Count == 0)
            {
                yield break; // Exit the coroutine if all slimes are destroyed
            }
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }
    }

    private IEnumerator StartWaveCountdown(float interval)
    {
        float countdown = interval;
        while (countdown > 0)
        {
            UpdateWaveIntervalText(countdown);
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }
        UpdateWaveIntervalText(0);
    }

    GameObject SpawnSlime(GameObject slimePrefab, Vector3 spawnPosition)
    {
        if (slimePrefab == null)
        {
            Debug.LogError("No slime prefab selected to spawn!");
            return null;
        }

        // Instantiate a slime at the specified spawn position
        GameObject newSlime = Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
        activeSlimes.Add(newSlime); // Add the new slime to the list of active slimes

        // Set the parent of the instantiated slime to the "Enemies" object
        GameObject enemiesParent = GameObject.Find("Enemies");
        if (enemiesParent != null)
        {
            newSlime.transform.parent = enemiesParent.transform;
        }
        else
        {
            Debug.LogError("Enemies parent object not found!");
        }
        Stats slimeStats = newSlime.GetComponent<Stats>();
        if (slimeStats != null)
        {
            slimeStats.OnSlimeDestroyed += OnSlimeDestroyed;
        }

        return newSlime;
    }

    Vector3 ChooseRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }

    GameObject ChooseSlimePrefab()
    {
        List<SlimeType> availableSlimeTypes = GetAvailableSlimeTypes();

        float totalProbability = 0f;
        foreach (var slimeType in availableSlimeTypes)
        {
            totalProbability += slimeType.spawnProbability;
        }

        float randomPoint = Random.value * totalProbability;
        foreach (var slimeType in availableSlimeTypes)
        {
            if (randomPoint < slimeType.spawnProbability)
            {
                return slimeType.prefab;
            }
            else
            {
                randomPoint -= slimeType.spawnProbability;
            }
        }
        return null; // In case no slime is selected, which should not happen
    }

    List<SlimeType> GetAvailableSlimeTypes()
    {
        // Calculate the maximum index of the slime types based on the wave number, adding one each multiple of 3 waves.
        int maxIndex = Mathf.Min(waveNumber / 3, slimeTypes.Count) - 1;
        
        // Ensure maxIndex is non-negative
        maxIndex = Mathf.Max(maxIndex, 0);

        List<SlimeType> availableSlimeTypes = new List<SlimeType>();

        for (int i = 0; i <= maxIndex; i++)
        {
            availableSlimeTypes.Add(slimeTypes[i]);
        }

        return availableSlimeTypes;
    }

    void OnSlimeDestroyed(GameObject slime)
    {
        activeSlimes.Remove(slime); // Remove the slime from the list of active slimes
    }

    void UpdateWaveNumberText()
    {
        if (waveNumberText != null)
        {
            waveNumberText.text = "Wave: " + waveNumber;
        }
        else
        {
            Debug.LogError("WaveNumberText reference is not set!");
        }
    }

    void UpdateWaveIntervalText(float interval)
    {
        if (waveIntervalText != null)
        {
            waveIntervalText.text = "Next Wave In: " + interval.ToString("F1") + "s";
        }
        else
        {
            Debug.LogError("WaveIntervalText reference is not set!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed to prevent memory leaks
        AgentBehavior.SlimeDestroyed -= OnSlimeDestroyed;
    }
}
