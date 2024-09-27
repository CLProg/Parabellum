using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int baseDamage = 20;   // Base damage for normal attacks
    public float critChance = 0.2f; // Chance for a critical hit (20%)
    public float attackRange = 1.5f; // Attack range
    public LayerMask enemyLayer;   // Layer to identify enemies
    public Transform attackPoint;   // Point where the attack occurs

    private SphereCollider attackCollider;

    void Awake()
    {
        attackCollider = gameObject.AddComponent<SphereCollider>();
        attackCollider.radius = attackRange; // Set the attack range
        attackCollider.isTrigger = true; // Make it a trigger
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Check for enemies within the collider
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackCollider.radius, enemyLayer);
        foreach (Collider enemyCollider in enemiesHit)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                int damage = CalculateDamage();
                enemy.TakeDamage(damage);
                Debug.Log($"Attacked {enemy.name} for {damage} damage.");
            }
        }
    }

    private int CalculateDamage()
    {
        int damage = baseDamage;
        if (Random.value < critChance)
        {
            damage *= 2; // Double damage for crit
            Debug.Log("Critical Hit!");
        }
        return damage;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange); // Visualize the attack area
    }
}
