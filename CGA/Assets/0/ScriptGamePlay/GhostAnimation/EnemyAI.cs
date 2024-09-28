using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Reference to the player
    public Transform hpBarCanvas; // Reference to the health bar canvas
    public float moveSpeed = 3f; // Enemy movement speed
    public float detectionRange = 10f; // Distance at which the enemy detects the player
    public float stoppingDistance = 2f; // Distance at which the enemy stops moving towards the player
    public int damage = 10; // Damage the enemy deals to the player
    public float attackCooldown = 2f; // Time between attacks

    private Animator animator;
    private Rigidbody rb;
    private float lastAttackTime; // Tracks time since last attack
    private PlayerHealth playerHealth; // Reference to the player's health script

    private bool walkingLeft = false; // Track walking left state
    private bool walkingRight = false; // Track walking right state

    void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        playerHealth = player.GetComponent<PlayerHealth>(); // Get the PlayerHealth component from the player
    }

    void Update()
    {
        // Check the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange && distanceToPlayer > stoppingDistance)
        {
            MoveTowardsPlayer(); // Move towards the player
        }
        else if (distanceToPlayer <= stoppingDistance)
        {
            AttackPlayer(); // Attack the player
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", false); // Stop walking animation when close to the player
        }
        else
        {
            rb.velocity = Vector3.zero; // Stop moving if the player is out of range
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isWalkingRight", false); // Set idle animation
        }

        // Update HP bar position if exists
        if (hpBarCanvas != null)
        {
            hpBarCanvas.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            hpBarCanvas.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation
        }
    }

    // Method to move the enemy towards the player
    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

        // Check direction and play the appropriate walking animation
        if (direction.x < 0) // Moving left
        {
            walkingLeft = true;
            walkingRight = false;
            animator.SetBool("isWalkingLeft", true);
            animator.SetBool("isWalkingRight", false);
        }
        else if (direction.x > 0) // Moving right
        {
            walkingRight = true;
            walkingLeft = false;
            animator.SetBool("isWalkingRight", true);
            animator.SetBool("isWalkingLeft", false);
        }
    }

    // Method to attack the player
    void AttackPlayer()
    {
        // If the cooldown time has passed, attack the player
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time; // Reset the attack timer
            playerHealth.TakeDamage(damage); // Damage the player
            Debug.Log($"{gameObject.name} attacked the player for {damage} damage.");
        }
    }

    // New method to check if enemy is moving
    public bool IsMoving()
    {
        return walkingLeft || walkingRight; // Returns true if enemy is moving
    }

    // New method to check if enemy is walking left
    public bool IsWalkingLeft()
    {
        return walkingLeft;
    }

    // New method to check if enemy is walking right
    public bool IsWalkingRight()
    {
        return walkingRight;
    }
}
