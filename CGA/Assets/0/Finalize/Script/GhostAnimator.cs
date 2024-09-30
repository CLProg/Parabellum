using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    // Enemy movement variables
    public float moveSpeed = 3f;
    public float minMoveTime = 2f;
    public float maxMoveTime = 5f;
    private float currentMoveTime;
    private Vector2 moveDirection;

    // Reference to the player
    public GameObject player;

    // Aggro area for the ghost
    public float aggroRange = 5f;
    private Vector3 originalPosition;

    // Attack-related variables
    public float attackRange = 3f;
    public int attackDamage = 10;
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    // Attack point and player layer
    public Transform attackPoint; // Position from where attack originates
    public float attackRadius = 0.5f; // Radius of attack area
    public LayerMask playerLayer; // Layer for the player or target to hit

    // Facing direction
    private bool facingRight = true;
    Animator animator;
    void Awake()
    {
        // Initialize variables
        animator = GetComponent<Animator>();
        originalPosition = transform.position;
        currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        moveDirection = Random.insideUnitCircle.normalized;
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // Enemy movement logic
        currentMoveTime -= Time.deltaTime;
        if (currentMoveTime <= 0)
        {
            moveDirection = Random.insideUnitCircle.normalized;
            currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        }

        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            float distanceToPlayer = direction.magnitude;
            direction.Normalize();

            if (distanceToPlayer < aggroRange)
            {
                moveDirection = new Vector2(direction.x, direction.z);
            }
            else
            {
                direction = originalPosition - transform.position;
                direction.Normalize();
                moveDirection = new Vector2(direction.x, direction.z);
            }
        }

        animator.SetFloat("Speed", Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.y));

        // Update attack timer
        attackTimer -= Time.deltaTime;

        // Attack logic
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= attackRange && attackTimer <= 0)
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        // Move the enemy
        Vector3 newPosition = transform.position;
        newPosition.x += moveDirection.x * moveSpeed * Time.fixedDeltaTime;
        newPosition.z += moveDirection.y * moveSpeed * Time.fixedDeltaTime;
        transform.position = newPosition;

        // Flip character direction
        Flip(moveDirection.x);
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        attackTimer = attackCooldown;
    }

    void Flip(float horizontal)
    {
        if (horizontal < 0 && facingRight || horizontal > 0 && !facingRight)
        {
            facingRight = !facingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize attack range
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}