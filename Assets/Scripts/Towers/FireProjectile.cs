using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private GameObject ExplosionEffect;
    [SerializeField] private float splashRange = 1f;
    private float speed;
    private int damage;
    private Transform target;
    public float maxLifetime;
    private float lifetime = 0f;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void SetMaxLifetime(float newMaxLifetime)
    {
        maxLifetime = newMaxLifetime;
    }
    
    void Update()
    {
        if (target == null || lifetime >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // move to target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        // update the lifetime
        lifetime += Time.deltaTime;
    }

     private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision is with an enemy
        if (collision.collider.CompareTag("Enemy"))
        {
            var enemy = collision.collider.GetComponent<Stats>();
            if (enemy != null)
            {
                ApplyDamage(enemy);

                // If splashRange is greater than 0, apply splash damage
                if (splashRange > 0)
                {
                    ApplySplashDamage();
                }
            }
            else
            {
                Debug.LogWarning("Enemy object without Stats component: " + collision.collider.name);
            }
        }
        else
        {
            Debug.Log("Collision with non-enemy object: " + collision.collider.name);
        }

        // Destroy the projectile
        if (ExplosionEffect != null)
        {
            Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void ApplyDamage(Stats enemy)
    {
        if (enemy != null && enemy.CompareTag("Enemy"))
        { 
            enemy.TakeDamage(damage);
        }
    }

    private void ApplySplashDamage()
    {
        // Get all colliders within the splash range
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, splashRange);
        foreach (var hitCollider in hitColliders)
        {
            // Check if the collider belongs to an enemy and has the Stats component
            if (hitCollider.CompareTag("Enemy"))
            {
                var enemyHit = hitCollider.GetComponent<Stats>();
                if (enemyHit != null)
                {
                    // Calculate damage based on distance
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);
                    var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                    int damageAmount = Mathf.RoundToInt(damagePercent * damage);

                    // Apply damage to the enemy
                    enemyHit.TakeDamage(damageAmount);
                }
                else
                {
                    Debug.LogWarning("Hit enemy object in splash range without Stats component: " + hitCollider.name);
                }
            }
        }
    }
}
