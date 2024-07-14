using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    [SerializeField] private GameObject ProjectileHitEffect;
    [SerializeField] private float slowPercentage = 0.5f;
    [SerializeField] private float slowDuration = 2f;
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DealDamage(collision.gameObject);
            ApplySlowEffect(collision.gameObject);

            // Instantiate hit effect if any
            if (ProjectileHitEffect != null)
            {
                Instantiate(ProjectileHitEffect, transform.position, Quaternion.identity);
            }

            // Destroy the projectile
            Destroy(gameObject);
        }
    }

    void DealDamage(GameObject enemy)
    {
        Stats stats = enemy.GetComponent<Stats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
        }
    }

    void ApplySlowEffect(GameObject enemy)
    {
        AgentBehavior agentBehavior = enemy.GetComponent<AgentBehavior>();
        if (agentBehavior != null)
        {
            agentBehavior.ApplySlow(slowPercentage, slowDuration);
        }
        else
        {
            //none
        }
        FlyingSlimeBehavior flyingSlimeBehavior = enemy.GetComponent<FlyingSlimeBehavior>();
        if (flyingSlimeBehavior != null)
        {
            flyingSlimeBehavior.ApplySlow(slowPercentage, slowDuration);
        }
        else
        {
            //none
        }
    }
}