using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int baseDamage = 20;
    public float critChance = 0.2f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;

    [Header("References")]
    public LayerMask enemyLayer;
    public Transform attackPoint;

    private float nextAttackTime = 0f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");

        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        Debug.Log($"Enemies hit: {enemiesHit.Length}");

        foreach (Collider enemyCollider in enemiesHit)
        {
            Debug.Log($"Detected: {enemyCollider.name}");
            if (enemyCollider.TryGetComponent(out GhostHealth ghostHealth))
            {
                int damage = CalculateDamage();
                ghostHealth.TakeDamage(damage);
                Debug.Log($"Attacked {ghostHealth.name} for {damage} damage.");
            }
        }
    }

    private int CalculateDamage()
    {
        int damage = baseDamage;
        if (Random.value < critChance)
        {
            damage *= 2;
            Debug.Log("Critical Hit!");
        }
        return damage;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}