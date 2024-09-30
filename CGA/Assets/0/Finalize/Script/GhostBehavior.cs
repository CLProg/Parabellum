using UnityEngine;
using System.Collections;

public class GhostBehavior : MonoBehaviour
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
        public int attackDamage = 10;
        public float attackCooldown = 1f;
        public Transform attackPoint;
        public float attackRadius = 0.5f;
        public LayerMask playerLayer;
    }

    [Header("Movement")]
    public MovementSettings movement;

    [Header("Attack")]
    public AttackSettings attack;

    [Header("References")]
    public GameObject player;
    public ParticleSystem attackEffect;

    private Vector3 originalPosition;
    private Vector3 moveDirection;
    private float currentIdleTime;
    private float attackTimer;
    private bool facingRight = true;

    private Animator animator;
    private Rigidbody rb;
    private GhostHealth health;

    private enum GhostState { Idle, Patrolling, Chasing, Attacking, Returning }
    private GhostState currentState = GhostState.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<GhostHealth>();

        originalPosition = transform.position;
        currentIdleTime = Random.Range(movement.minIdleTime, movement.maxIdleTime);
        player = GameObject.FindWithTag("Player");
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
        if (health.IsDead) return; // Ensure no state changes if dead

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

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
                HandleAttackingState();
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
        }
        else if (distanceToPlayer <= movement.aggroRange)
        {
            currentState = GhostState.Chasing;
        }
    }

    private void HandlePatrollingState(float distanceToPlayer)
    {
        if (distanceToPlayer <= movement.aggroRange)
        {
            currentState = GhostState.Chasing;
        }
        else if (Vector3.Distance(transform.position, originalPosition) > movement.deaggroRange)
        {
            currentState = GhostState.Returning;
        }
    }

    private void HandleChasingState(float distanceToPlayer)
    {
        if (distanceToPlayer <= attack.attackRange)
        {
            currentState = GhostState.Attacking;
        }
        else if (distanceToPlayer > movement.deaggroRange)
        {
            currentState = GhostState.Returning;
        }
        else
        {
            moveDirection = (player.transform.position - transform.position).normalized;
            moveDirection.y = 0; // Restrict vertical movement
        }
    }

    private void HandleAttackingState()
    {
        if (attackTimer <= 0)
        {
            Attack();
        }
        currentState = GhostState.Chasing;
    }

    private void HandleReturningState()
    {
        moveDirection = (originalPosition - transform.position).normalized;
        moveDirection.y = 0; // Restrict vertical movement
        if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
        {
            currentState = GhostState.Idle;
            currentIdleTime = Random.Range(movement.minIdleTime, movement.maxIdleTime);
        }
    }

    private void Move()
    {
        Vector3 movement = moveDirection * this.movement.moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        Flip(moveDirection.x);
    }

    private void SetRandomPatrolDirection()
    {
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, 0).normalized;
    }

    private void Attack()
    {
        if (health.IsDead) return; // Prevent attacks if dead

        animator.SetTrigger("Attack");
        if (attackEffect != null)
        {
            attackEffect.Play();
        }

        Collider[] hitPlayers = Physics.OverlapSphere(attack.attackPoint.position, attack.attackRadius, attack.playerLayer);

        foreach (Collider player in hitPlayers)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(attack.attackDamage);
        }

        attackTimer = attack.attackCooldown;
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
}
