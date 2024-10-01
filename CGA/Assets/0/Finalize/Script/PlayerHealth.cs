using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    [Header("Invulnerability Settings")]
    public float invulnerabilityDuration = 2f;

    [Header("UI Settings")]
    public Image healthBar;
    public float healthBarUpdateDelay = 0.5f;

    [Header("Animation")]
    public string animatorChildName = "Graphics"; // Name of the child object with the Animator
    public string takeDamageAnimTrigger = "TakeDamage";
    public string deathAnimTrigger = "Die";
    public string respawnAnimTrigger = "Respawn";
    public string attackAnimTrigger = "Attack";
    public float hurtAnimationDelay = 0.5f; // Delay before playing hurt animation

    private bool isDead = false;
    private bool isInvulnerable = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider capsuleCollider;
    private bool isDying = false;

    // Variables to hold movement values
    float horizontal;
    float vertical;
    bool facingRight = true; // Initialize as true if the character starts facing right

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Find the Animator component in the child object
        FindAnimator();

        // Find the SpriteRenderer component in the child object
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found in child objects.");
        }

        // Find the CapsuleCollider component
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            Debug.LogWarning("CapsuleCollider not found.");
        }
    }

    void FindAnimator()
    {
        Transform childTransform = transform.Find(animatorChildName);
        if (childTransform != null)
        {
            animator = childTransform.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"Animator component not found on child '{animatorChildName}'");
            }
        }
        else
        {
            Debug.LogWarning($"Child object '{animatorChildName}' not found");
        }
    }

    void Update()
    {
        // Get input for movement
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Calculate movement speed based on both axes
        float speed = new Vector2(horizontal, vertical).magnitude;

        // Set the Speed parameter in the animator component
        animator.SetFloat("Speed", speed);

        // Check for attack input
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        // Flip character based on horizontal input
        FlipCharacter(horizontal);
    }

    private void FlipCharacter(float horizontal)
    {
        // Check if A key is pressed to flip left
        if (horizontal < 0 && facingRight)
        {
            facingRight = false; // Set facing to left
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = true; // Flip the sprite to the left
            }
        }
        // Check if D key is pressed to flip right
        else if (horizontal > 0 && !facingRight)
        {
            facingRight = true; // Set facing to right
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false; // Flip the sprite to the right
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable || isDying) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HandleHurtAnimation());
            StartInvulnerability();
        }

        StartCoroutine(UpdateHealthBarWithDelay(healthBarUpdateDelay));
    }

    private IEnumerator HandleHurtAnimation()
    {
        yield return new WaitForSeconds(hurtAnimationDelay); // Wait for the specified delay
        // Trigger take damage animation only if not dead or dying
        if (animator != null && !isDead && !isDying)
        {
            animator.SetTrigger(takeDamageAnimTrigger);
        }
    }

    void Die()
    {
        if (isDead || isDying) return;

        isDying = true; // Set the flag to indicate the player is dying
        isDead = true;
        Debug.Log("Player has died!");

        // Disable the capsule collider to prevent further interactions
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
            Debug.Log("Capsule collider disabled.");
        }

        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger(deathAnimTrigger);
        }

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        isDying = false; // Reset the dying flag
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthBar();

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        Debug.Log("Player has respawned at the respawn point.");

        // Trigger respawn animation
        if (animator != null)
        {
            animator.SetTrigger(respawnAnimTrigger);
        }

        // Re-enable the collider
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
            Debug.Log("Capsule collider enabled.");
        }

        StartInvulnerability();
    }

    IEnumerator UpdateHealthBarWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void StartInvulnerability()
    {
        isInvulnerable = true;
        Debug.Log("Player is now invulnerable.");
        Invoke(nameof(EndInvulnerability), invulnerabilityDuration);
    }

    void EndInvulnerability()
    {
        isInvulnerable = false;
        Debug.Log("Player is no longer invulnerable.");
    }

    private void Attack()
    {
        // Trigger the attack animation
        animator.SetTrigger(attackAnimTrigger);
    }
}