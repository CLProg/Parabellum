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
    public float attackRate = 1f;   // Attacks per second
    private float nextAttackTime = 1f; // Time when the player can attack again
    private Animator animator; // Reference to the Animator component

    void Awake()
    {
        // Find the Animator on the child GameObject
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Handle player attack input and cooldown
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate; // Set the next attack time
        }
    }

    private void Attack()
    {
        // Trigger the attack animation
        animator.SetTrigger("Attack");

        // Check for enemies within attack range
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        // Apply damage to each enemy hit
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

    // Calculate the damage with potential critical hits
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

    // Visualize the attack range in the editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
