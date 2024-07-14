using UnityEngine;

public class SpawnOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject smallerMonsterPrefab;
    [SerializeField] private int numberOfMonstersToSpawn = 3;
    [SerializeField] private float spawnRadius = 0.25f; // Radius for random spawn positions

    private Stats stats;

    private void Start()
    {
        stats = GetComponent<Stats>();
        if (stats != null)
        {
            stats.OnSlimeDeath += HandleOnDeath;
        }
    }

    private void HandleOnDeath(GameObject monster)
    {
        for (int i = 0; i < numberOfMonstersToSpawn; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(smallerMonsterPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = new Vector3(transform.position.x + randomOffset.x, transform.position.y, transform.position.z + randomOffset.y);
        return spawnPosition;
    }
}
