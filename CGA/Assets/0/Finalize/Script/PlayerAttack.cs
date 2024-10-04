using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int baseDamage = 20;
    public float critChance = 0.2f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    public float attackAnimationDuration = 0.5f; // Duration of the attack animation

    [Header("References")]
    public LayerMask enemyLayer;
    public Transform attackPoint;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip hitSound;
    [Range(0f, 1f)]
    public float attackVolume = 0.5f;
    [Range(0f, 1f)]
    public float hitVolume = 0.5f;

    private float nextAttackTime = 0f;
    private float attackAnimationEndTime = 0f;
    private Animator animator;
    private AudioSource audioSource;
    private bool isAttacking = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime && !isAttacking)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
            attackAnimationEndTime = Time.time + attackAnimationDuration;
            isAttacking = true;
        }

        if (isAttacking && Time.time >= attackAnimationEndTime)
        {
            isAttacking = false;
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
        PlayAttackSound();

        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        Debug.Log($"Enemies hit: {enemiesHit.Length}");

        bool hitEnemy = false;

        foreach (Collider enemyCollider in enemiesHit)
        {
            Debug.Log($"Detected: {enemyCollider.name}");
            if (enemyCollider.TryGetComponent(out GhostHealth ghostHealth))
            {
                if (!ghostHealth.IsInvulnerable()) // Check if the enemy is invulnerable
                {
                    int damage = CalculateDamage();
                    ghostHealth.TakeDamage(damage);
                    Debug.Log($"Attacked {ghostHealth.name} for {damage} damage.");
                    hitEnemy = true;
                }
                else
                {
                    Debug.Log($"{ghostHealth.name} is invulnerable. No damage dealt.");
                }
            }

            if (enemyCollider.TryGetComponent(out KamatayanHP kamatayanHealth))
            {
                if (!kamatayanHealth.IsInvulnerable())
                {
                    int damage = CalculateDamage();
                    kamatayanHealth.TakeDamage(damage);
                    Debug.Log($"Attacked {kamatayanHealth.name} for {damage} damage.");
                    hitEnemy = true;
                }
                else
                {
                    Debug.Log($"{kamatayanHealth.name} is invulnerable. No damage dealt.");
                }
            }
            if (enemyCollider.TryGetComponent(out SulyapHP sulyapHealth))
            {
                if (!sulyapHealth.IsInvulnerable())
                {
                    int damage = CalculateDamage();
                    sulyapHealth.TakeDamage(damage);
                    Debug.Log($"Attacked {sulyapHealth.name} for {damage} damage.");
                    hitEnemy = true;
                }
                else
                {
                    Debug.Log($"{sulyapHealth.name} is invulnerable. No damage dealt.");
                }
            }
        }

        if (hitEnemy)
        {
            PlayHitSound();
        }
    }


    private void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound, attackVolume);
        }
        else
        {
            Debug.LogWarning("Attack sound is not assigned!");
        }
    }

    private void PlayHitSound()
    {
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, hitVolume);
        }
        else
        {
            Debug.LogWarning("Hit sound is not assigned!");
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
