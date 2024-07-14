using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaTowerAI : MonoBehaviour
{
    private CircleCollider2D attackRangeCollider;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab; 
    // stat parameters
    public float attackCooldown = 5f;
    public float timeSinceLastAttack = 0f;
    public float projectileSpeed = 10f;
    public int projectileDamage = 50;
    public float projectileLifetime = 5f;
    // upgrade parameters
    public int level = 1;
    public int baseUpgradeCost = 100;
    public int refundCost;
    public float upgradeCostMultiplier = 3f;

    void Start()
    {
        if (attackRangeCollider == null)
        {
            attackRangeCollider = gameObject.GetComponent<CircleCollider2D>();
            if (attackRangeCollider == null)
            {
                attackRangeCollider = gameObject.AddComponent<CircleCollider2D>();
            }
        }

        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.parent = transform;
            firePoint.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        GameObject target = FindClosestTarget();

        if (target != null)
        {
            RotateTowardsTarget(target.transform);

            if (Time.time - timeSinceLastAttack > attackCooldown)
            {
                Attack(target);
                timeSinceLastAttack = Time.time;
            }
        }
    }

    GameObject FindClosestTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRangeCollider.radius);

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);

                if (distance < attackRangeCollider.radius && distance < closestDistance)
                {
                    closestTarget = collider.gameObject;
                    closestDistance = distance;
                }
            }
        }

        return closestTarget;
    }

    void Attack(GameObject target)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        BasicProjectile script = projectile.GetComponent<BasicProjectile>();

        if (script != null)
        {
            script.SetTarget(target.transform);
            script.SetSpeed(projectileSpeed);
            script.SetDamage(projectileDamage);
            script.SetMaxLifetime(projectileLifetime);
        }
    }
    void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }

    public TowerStats GetCurrentStats()
    {
        return new TowerStats
        {
            AttackCooldown = attackCooldown,
            ProjectileDamage = projectileDamage,
            ProjectileLifetime = projectileLifetime,
        };
    }

    //=======================//
    public TowerStats GetUpgradedStats(TowerStats upgradeValues)
    {
        return new TowerStats
        {
            AttackCooldown = attackCooldown - upgradeValues.AttackCooldown,
            ProjectileDamage = projectileDamage + upgradeValues.ProjectileDamage,
            ProjectileLifetime = projectileLifetime + upgradeValues.ProjectileLifetime,
        };
    }

    public void UpgradeTower(TowerStats upgradeValues)
    {
        attackCooldown -= upgradeValues.AttackCooldown;
        projectileDamage += upgradeValues.ProjectileDamage;
        projectileLifetime += upgradeValues.ProjectileLifetime;
    }

    public int CalculateUpgradeCost(int currentLevel)
    {
        int totalUpgradeCost = baseUpgradeCost;

        for (int i = 1; i < currentLevel; i++)
        {
            totalUpgradeCost += Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(upgradeCostMultiplier, i));
        }

        return totalUpgradeCost;
    }

    public int CalculateRefundCost(int currentLevel)
    {
        for (int i = 1; i < currentLevel; i++)
        {
            int refundCost = Mathf.RoundToInt((baseUpgradeCost * Mathf.Pow(upgradeCostMultiplier, i)) * 0.5f);
        }
        return refundCost;
    }
}
