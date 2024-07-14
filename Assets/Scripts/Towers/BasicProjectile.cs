using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    [SerializeField] private GameObject ProjectileHitEffect;
    private float speed;
    private int damage;
    private Transform target;
    private float maxLifetime;
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
        
        Vector3 direction = (target.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        lifetime += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision is with an enemy
        var enemy = collision.collider.GetComponent<Stats>();
        if (enemy != null)
        {
            // Apply regular damage to the enemy
            enemy.TakeDamage(damage);

            // Instantiate hit effect if any
            if (ProjectileHitEffect != null)
            {
                Instantiate(ProjectileHitEffect, transform.position, Quaternion.identity);
            }

            // Destroy the projectile
            Destroy(gameObject);
        }
    }
}
