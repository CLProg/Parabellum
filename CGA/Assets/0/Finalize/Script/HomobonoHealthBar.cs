using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HomobonoHealthBar : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    [Header("Invulnerability Settings")]
    public float invulnerabilityDuration = 2f;

    [Header("UI Settings")]
    public Image healthBar;
    public float healthBarUpdateDelay = 0.5f;

    [Header("Animation")]
    public string takeDamageAnimTrigger = "TakeDamage";
    public string deathAnimTrigger = "Die";
    public string respawnAnimTrigger = "Respawn";
    public float hurtAnimationDelay = 0.5f;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip respawnSound;
    public float hurtSoundDelay = 0.3f;
    private AudioSource audioSource;

    private bool isDead = false;
    private bool isInvulnerable = false;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private bool isDying = false;

    // Reference to the player's controller script
    private Homobono playerController;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        animator = GetComponent<Animator>(); // Animator is on the parent
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerController = GetComponent<Homobono>();

        if (capsuleCollider == null)
        {
            Debug.LogWarning("CapsuleCollider not found.");
        }
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController script not found.");
        }
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // No movement or attack handling here
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
            animator?.SetTrigger(takeDamageAnimTrigger);
            StartInvulnerability();
            StartCoroutine(DelayedHurtSound());
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

        capsuleCollider.enabled = false;
        Debug.Log("Capsule collider disabled.");

        animator?.SetTrigger(deathAnimTrigger);

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

        animator?.SetTrigger(respawnAnimTrigger);

        capsuleCollider.enabled = true;
        Debug.Log("Capsule collider enabled.");

        // Re-enable movement
        EnableMovement();

        StartInvulnerability();
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void DisableMovement()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    void EnableMovement()
    {
        if (playerController != null)
        {
            playerController.enabled = true;
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
}
