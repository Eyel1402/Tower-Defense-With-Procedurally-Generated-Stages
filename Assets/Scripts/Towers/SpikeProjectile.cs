using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeProjectile : MonoBehaviour
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

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 0.1f)
        {
            DealDamage(target.gameObject);
            if (ProjectileHitEffect != null)
            {
                Instantiate(ProjectileHitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        lifetime += Time.deltaTime;
    }

    void DealDamage(GameObject enemy)
    {
        if (enemy.CompareTag("Enemy"))
        {
            Stats stats = enemy.GetComponent<Stats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }
        }
    }
}
