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
    public string animatorChildName = "Graphics";
    public string takeDamageAnimTrigger = "TakeDamage";
    public string deathAnimTrigger = "Die";
    public string respawnAnimTrigger = "Respawn";
    public string attackAnimTrigger = "Attack";
    public float hurtAnimationDelay = 0.5f;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip respawnSound;
    public float hurtSoundDelay = 0.3f; // Delay before playing hurt sound
    private AudioSource audioSource;

    private bool isDead = false;
    private bool isInvulnerable = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider capsuleCollider;
    private bool isDying = false;

    // Variables to hold movement values
    float horizontal;
    float vertical;
    bool facingRight = true;

    // Reference to the player's movement script
    private PlayerMovement playerMovement;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        FindAnimator();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerMovement = GetComponent<PlayerMovement>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found in child objects.");
        }
        if (capsuleCollider == null)
        {
            Debug.LogWarning("CapsuleCollider not found.");
        }
        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMovement script not found.");
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
        if (isDead || isDying) return; // Skip movement and attack input if dead or dying

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
        if (horizontal < 0 && facingRight)
        {
            facingRight = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = true;
            }
        }
        else if (horizontal > 0 && !facingRight)
        {
            facingRight = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable || isDying) return;

        // Start coroutine to delay damage application
        StartCoroutine(DelayedDamage(damage));
    }

    private IEnumerator DelayedDamage(int damage)
    {
        // Wait for the delay before applying damage
        yield return new WaitForSeconds(hurtAnimationDelay);

        // Apply the damage
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null && !isDead && !isDying)
            {
                animator.SetTrigger(takeDamageAnimTrigger); // Trigger hurt animation
            }
            StartInvulnerability();
            StartCoroutine(DelayedHurtSound()); // Play sound with delay
        }

        StartCoroutine(UpdateHealthBarWithDelay(healthBarUpdateDelay));
    }

    private IEnumerator DelayedHurtSound()
    {
        // Delay the hurt sound by the specified time
        yield return new WaitForSeconds(hurtSoundDelay);
        PlaySound(hurtSound);
    }

    void Die()
    {
        if (isDead || isDying) return;

        isDying = true;
        isDead = true;
        Debug.Log("Player has died!");

        PlaySound(deathSound);
        // Disable movement
        DisableMovement();

        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
            Debug.Log("Capsule collider disabled.");
        }

        if (animator != null)
        {
            animator.SetTrigger(deathAnimTrigger);
        }

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        isDying = false;
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthBar();

        PlaySound(respawnSound);

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        Debug.Log("Player has respawned at the respawn point.");

        if (animator != null)
        {
            animator.SetTrigger(respawnAnimTrigger);
        }

        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
            Debug.Log("Capsule collider enabled.");
        }

        // Re-enable movement
        EnableMovement();

        StartInvulnerability();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void DisableMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    void EnableMovement()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
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
        animator.SetTrigger(attackAnimTrigger);
    }
}
