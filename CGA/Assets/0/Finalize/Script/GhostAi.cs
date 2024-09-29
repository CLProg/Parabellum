using UnityEngine;

public class GhostAI : MonoBehaviour
{
    // Public variables for customization
    public float speed = 3f;
    public float changeDirectionInterval = 2f;
    public float patrolRange = 5f;
    public LayerMask obstacleLayer;

    // Private variables for internal tracking
    private Rigidbody rb;
    private Vector2 initialPosition;
    private Vector2 direction;
    private float timer;

    // Reference to the child object's animator
    private Animator childAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        direction = Random.insideUnitCircle.normalized;
        timer = 0f;

        // Find the child object's animator component
        childAnimator = GetComponentInChildren<Animator>();

        // If the animator is not found, log a warning
        if (childAnimator == null)
        {
            Debug.LogWarning("Animator component not found on child object.");
        }
    }

    void Update()
    {
        // Update timer
        timer += Time.deltaTime;

        // Check if it's time to change direction
        if (timer >= changeDirectionInterval)
        {
            // Randomly choose a new direction within the patrol range, avoiding obstacles
            Vector2 newDirection = Random.insideUnitCircle.normalized;
            while (Physics2D.Raycast(transform.position, newDirection, patrolRange, obstacleLayer))
            {
                newDirection = Random.insideUnitCircle.normalized;
            }
            direction = Vector2.Lerp(direction, newDirection, Time.deltaTime * 5f); // Smooth transition
            timer = 0f;
        }

        // Calculate movement based on direction and speed
        Vector2 movement = direction * speed * Time.deltaTime;

        // Check if the ghost has moved too far from its initial position
        if (Vector2.Distance(transform.position, initialPosition) >= patrolRange)
        {
            // Reverse direction to keep the ghost within the patrol range
            direction = -direction;
        }

        // Apply movement to the rigidbody
        rb.velocity = movement;

        // Update the child animator's parameters (assuming you have "Speed" and "Direction" parameters)
        if (childAnimator != null)
        {
            childAnimator.SetFloat("Speed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y));
            childAnimator.SetFloat("Direction", rb.velocity.x > 0 ? 1 : rb.velocity.x < 0 ? -1 : 0);
        }
    }
}