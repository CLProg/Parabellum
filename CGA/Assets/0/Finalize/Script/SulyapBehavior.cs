using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SulyapBehavior : MonoBehaviour
{
    [System.Serializable]
    public class MovementSettings
    {
        public float moveSpeed = 3f;
        public float minIdleTime = 2f;
        public float maxIdleTime = 5f;
        public float aggroRange = 5f;
        public float deaggroRange = 8f;
    }

    [System.Serializable]
    public class AttackSettings
    {
        public float attackRange = 3f;
        public int baseDamage = 10;
        public float damageVariance = 0.2f; // 20% variance
        public float attackCooldown = 1.5f;
        public Transform attackPoint;
        public float attackRadius = 0.5f;
        public LayerMask playerLayer;
        public float criticalHitChance = 0.1f; // 10% chance of critical hit
        public float criticalHitMultiplier = 1.5f;
    }
    private float lastAttackTime;
    [Header("Movement")]
    public MovementSettings movement;

    [Header("Attack")]
    public AttackSettings attack;

    [Header("References")]
    public GameObject player;
    public ParticleSystem attackEffect;

    [Header("Audio")]
    public AudioClip attackSound;
    private AudioSource audioSource;

    private Vector3 originalPosition;
    private Vector3 moveDirection;
    private float currentIdleTime;
    private float attackTimer;
    public bool facingRight = true; // Made public

    private Animator animator;
    private Rigidbody rb;
    private SulyapHP health;

    private enum GhostState { Idle, Patrolling, Chasing, Attacking, Returning }
    private GhostState currentState = GhostState.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<SulyapHP>();

        originalPosition = transform.position;
        currentIdleTime = Random.Range(movement.minIdleTime, movement.maxIdleTime);
        player = GameObject.FindWithTag("Player");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (health.IsDead) return; // Stop updating if dead

        UpdateState();
        UpdateAttackTimer();
    }

    private void FixedUpdate()
    {
        if (health.IsDead) return; // Stop moving if dead

        Move();
    }

    private void UpdateState()
    {
        if (health.IsDead) return;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player not found!");
                return;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        Debug.Log($"Current State: {currentState}, Distance to Player: {distanceToPlayer}");

        switch (currentState)
        {
            case GhostState.Idle:
                HandleIdleState(distanceToPlayer);
                break;
            case GhostState.Patrolling:
                HandlePatrollingState(distanceToPlayer);
                break;
            case GhostState.Chasing:
                HandleChasingState(distanceToPlayer);
                break;
            case GhostState.Attacking:
                HandleAttackingState(distanceToPlayer);
                break;
            case GhostState.Returning:
                HandleReturningState();
                break;
        }
    }

    private void HandleIdleState(float distanceToPlayer)
    {
        currentIdleTime -= Time.deltaTime;
        if (currentIdleTime <= 0)
        {
            currentState = GhostState.Patrolling;
            SetRandomPatrolDirection();
            Debug.Log("Transitioning to Patrolling state");
        }
        else if (distanceToPlayer <= movement.aggroRange)
        {
            currentState = GhostState.Chasing;
            Debug.Log("Player detected! Transitioning to Chasing state");
        }
    }

    private void HandlePatrollingState(float distanceToPlayer)
    {
        if (distanceToPlayer <= movement.aggroRange)
        {
            currentState = GhostState.Chasing;
            Debug.Log("Player in range! Transitioning to Chasing state");
        }
        else if (Vector3.Distance(transform.position, originalPosition) > movement.deaggroRange)
        {
            currentState = GhostState.Returning;
            Debug.Log("Too far from original position. Returning.");
        }
    }

    private void HandleChasingState(float distanceToPlayer)
    {
        if (distanceToPlayer <= attack.attackRange)
        {
            currentState = GhostState.Attacking;
            Debug.Log("In attack range! Transitioning to Attacking state");
        }
        else if (distanceToPlayer > movement.deaggroRange)
        {
            currentState = GhostState.Returning;
            Debug.Log("Player too far. Returning to original position.");
        }
        else
        {
            moveDirection = (player.transform.position - transform.position).normalized;
            moveDirection.y = 0; // Restrict vertical movement
        }
    }

    private void HandleAttackingState(float distanceToPlayer)
    {
        if (distanceToPlayer <= attack.attackRange)
        {
            Attack();
        }
        else
        {
            currentState = GhostState.Chasing;
            Debug.Log("Player out of attack range. Resuming chase.");
        }
    }

    private void HandleReturningState()
    {
        moveDirection = (originalPosition - transform.position).normalized;
        moveDirection.y = 0; // Restrict vertical movement
        if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
        {
            currentState = GhostState.Idle;
            currentIdleTime = Random.Range(movement.minIdleTime, movement.maxIdleTime);
            Debug.Log("Returned to original position. Transitioning to Idle state.");
        }
    }

    private void Move()
    {
        Vector3 movement = moveDirection * this.movement.moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        Flip(moveDirection.x);

        Debug.Log($"Moving: Direction = {moveDirection}, Speed = {this.movement.moveSpeed}");
    }
    private void SetRandomPatrolDirection()
    {
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, 0).normalized;
    }

    private void Attack()
    {
        if (health.IsDead) return;

        float currentTime = Time.time;
        if (currentTime - lastAttackTime < attack.attackCooldown)
        {
            return; // Attack is still on cooldown
        }

        animator.SetTrigger("Attack");
        PlaySound(attackSound);

        if (attackEffect != null)
        {
            attackEffect.Play();
        }

        if (attack.attackPoint != null)
        {
            Collider[] hitPlayers = Physics.OverlapSphere(attack.attackPoint.position, attack.attackRadius, attack.playerLayer);
            foreach (Collider playerCollider in hitPlayers)
            {
                PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    int damage = CalculateDamage();
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Dealt {damage} damage to player");
                }
            }
        }
        else
        {
            Debug.LogError("Attack point is not assigned in the AttackSettings!");
        }

        lastAttackTime = currentTime;
    }
    private int CalculateDamage()
    {
        float variance = Random.Range(-attack.damageVariance, attack.damageVariance);
        float damageWithVariance = attack.baseDamage * (1 + variance);

        if (Random.value < attack.criticalHitChance)
        {
            damageWithVariance *= attack.criticalHitMultiplier;
            Debug.Log("Critical Hit!");
        }

        return Mathf.RoundToInt(damageWithVariance);
    }
    private void UpdateAttackTimer()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private void Flip(float horizontal)
    {
        if (horizontal != 0)
        {
            bool shouldFlip = (horizontal < 0 && facingRight) || (horizontal > 0 && !facingRight);
            if (shouldFlip)
            {
                facingRight = !facingRight;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        rb.velocity = Vector3.zero;

        // Disable all colliders so the ghost cannot interact with the player
        Collider[] colliders = GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        enabled = false; // Disable movement and other behaviors
        StartCoroutine(DestroyAfterDelay(2f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, movement.aggroRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, movement.deaggroRange);
        if (attack.attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attack.attackPoint.position, attack.attackRadius);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
