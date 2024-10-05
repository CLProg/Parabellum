using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homobono : MonoBehaviour
{
    [Header("Movement Settings")]
    float horizontal;
    float vertical;
    bool facingRight = true;
    public float moveSpeed = 2f;

    [Header("Attack Settings")]
    public int baseDamage = 20;
    public float critChance = 0.2f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    public float attackAnimationDuration = 0.5f;

    [Header("References")]
    public LayerMask enemyLayer;
    public Transform attackPoint;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip hitSound;
    [Range(0f, 1f)] public float attackVolume = 0.5f;
    [Range(0f, 1f)] public float hitVolume = 0.5f;

    private float nextAttackTime = 0f;
    private float attackAnimationEndTime = 0f;
    private Animator animator;
    private AudioSource audioSource;
    private bool isAttacking = false;
    private bool attackOnCooldown = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>(); // Initialize SpriteRenderer
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    private void HandleMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Log the input values
        Debug.Log($"Horizontal: {horizontal}, Vertical: {vertical}");

        // Calculate movement speed
        float speed = new Vector2(horizontal, vertical).magnitude;
        animator.SetFloat("Speed", speed);

        // Apply the movement to the transform
        Vector3 move = new Vector3(horizontal, 0, vertical);
        transform.Translate(move * Time.deltaTime * moveSpeed);

        // Check for character flipping continuously based on horizontal input
        Flip(horizontal);
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime && !isAttacking && !attackOnCooldown)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
            attackAnimationEndTime = Time.time + attackAnimationDuration;
            isAttacking = true;
            StartCoroutine(AttackCooldown());
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
            if (enemyCollider.TryGetComponent(out GhostHealth ghostHealth))
            {
                if (!ghostHealth.IsInvulnerable())
                {
                    int damage = CalculateDamage();
                    ghostHealth.TakeDamage(damage);
                    Debug.Log($"Attacked {ghostHealth.name} for {damage} damage.");
                    hitEnemy = true;
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
            }
        }

        if (hitEnemy)
        {
            PlayHitSound();
        }
    }

    private IEnumerator AttackCooldown()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(attackRate);
        attackOnCooldown = false;
    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight)
        {
            facingRight = true;
            FlipCharacter();
        }
        else if (horizontal < 0 && facingRight)
        {
            facingRight = false;
            FlipCharacter();
        }
    }

    private void FlipCharacter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;  // Flip the sprite based on facingRight
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}
