using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    // Var to hold horizontal and vertical value
    float horizontal;
    float vertical;
    // Initialize an Animator variable
    Animator animator;
    // Boolean variable to test if facing right
    bool facingRight;

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

    void Awake()
    {
        // Link the GameObject Animator component to the animator variable
        animator = GetComponent<Animator>();

        // Store the original position of the ghost
        originalPosition = transform.position;

        // Initialize enemy movement variables
        currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        moveDirection = Random.insideUnitCircle.normalized;

        // Find the player GameObject
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // Check if the enemy's movement time is up
        if (currentMoveTime <= 0)
        {
            // Choose a new random direction
            moveDirection = Random.insideUnitCircle.normalized;
            currentMoveTime = Random.Range(minMoveTime, maxMoveTime);
        }

        // If the player is found, calculate the direction towards the player
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            float distanceToPlayer = direction.magnitude; // Get the distance to the player

            // Normalize the direction
            direction.Normalize();

            // Check if the player is within the aggro range
            if (distanceToPlayer < aggroRange)
            {
                // Move towards the player
                moveDirection = new Vector2(direction.x, direction.z); // Use direction.z for movement along the z-axis
            }
            else
            {
                // If the player is outside the aggro range, return to the original position
                direction = originalPosition - transform.position;
                direction.Normalize();
                moveDirection = new Vector2(direction.x, direction.z);
            }
        }

        // Calculate the horizontal and vertical movement based on the direction
        horizontal = moveDirection.x;
        vertical = moveDirection.y;

        // Set the Speed parameter in the animator component
        animator.SetFloat("Speed", Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        // Set the Direction parameter based on the input
        animator.SetFloat("Direction", horizontal > 0 ? 1 : horizontal < 0 ? -1 : 0);
    }

    void FixedUpdate()
    {
        // Move the enemy in the chosen direction
        Vector3 newPosition = transform.position;
        newPosition.x += horizontal * moveSpeed * Time.fixedDeltaTime;
        newPosition.z += vertical * moveSpeed * Time.fixedDeltaTime; // Move along the z-axis
        transform.position = newPosition;

        // Function for changing the character facing direction
        Flip(horizontal);

        // Decrement the current move time
        currentMoveTime -= Time.fixedDeltaTime;
    }

    private void Flip(float horizontal)
    {
        // Check where the character is currently facing and adjust the graphics direction
        if (horizontal < 0 && !facingRight || horizontal > 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
