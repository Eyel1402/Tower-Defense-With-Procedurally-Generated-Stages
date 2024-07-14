using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxinProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;

    private float maxLifetime = 5f; // how long the projectile stays
    private float lifetime = 0f;
    private Transform target;
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
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
            // deal damage to the target and destroy the projectile
            DealDamage(target.gameObject);
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
