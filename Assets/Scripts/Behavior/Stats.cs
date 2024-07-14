using UnityEngine;
using System;
using Unity.VisualScripting;

public class Stats : MonoBehaviour
{
    public int baseHealth = 100;
    public int maxHealth;
    public int currentHealth;
    public int attackDamage = 10;
    public int goldEarned = 2;
    [SerializeField] private GameObject SlimeDeathEffect;
    [SerializeField] private GameObject healthBarPrefab;
    public float goldMultiplier = 0.1f;
    public float healthIncreaseRate = 0.1f;
    private GoldSystem goldSystem;
    private HealthBar healthBarInstance;
    private SlimeSpawner slimeSpawner; 
    public event Action<GameObject> OnSlimeDestroyed;
    public event Action<GameObject> OnSlimeDeath;

    private bool isDead = false; 

    void Update()
    {
        // If needed, logic for updating each frame can be added here
    }

    private void Start()
    {
        maxHealth = baseHealth;
        currentHealth = maxHealth;
        goldSystem = FindFirstObjectByType<GoldSystem>();
        slimeSpawner = FindFirstObjectByType<SlimeSpawner>();
        
        if (slimeSpawner == null)
        {
            Debug.LogError("SlimeSpawner not found in the scene.");
        }
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, transform).GetComponent<HealthBar>();
            healthBarInstance.SetTarget(transform); // Set the target for the health bar
        }
        ApplyMultipliersBasedOnWave();
    }

    private void ApplyMultipliersBasedOnWave()
    {
        if (slimeSpawner != null)
        {
            int waveNumber = slimeSpawner.WaveNumber;
        
            if (waveNumber > 1)
            {
                maxHealth = Mathf.CeilToInt(baseHealth * (1 + waveNumber * healthIncreaseRate));
                currentHealth = maxHealth;
            }
            else
            {
                maxHealth = baseHealth;
                currentHealth = maxHealth;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBarInstance?.UpdateHealthBar(currentHealth, maxHealth); // Update health bar
        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void Die()
    {
        isDead = true; 
        goldSystem.AddGold(goldEarned);
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject); // Destroy the health bar along with the agent
        }

        OnSlimeDeath?.Invoke(gameObject);
        
        if (SlimeDeathEffect != null)
        {
            Instantiate(SlimeDeathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    public int GetGoldEarned()
    {
        return Mathf.CeilToInt(goldEarned * slimeSpawner.WaveNumber * goldMultiplier);
    }

    void OnDestroy()
    {
        isDead = false;
        GoldSystem goldSystem = FindFirstObjectByType<GoldSystem>();
        if (goldSystem != null)
        {
            goldSystem.AddGold(GetGoldEarned());
        }
        OnSlimeDestroyed?.Invoke(gameObject);
    }
}
     
