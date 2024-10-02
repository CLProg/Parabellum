using System.Collections; // For IEnumerator
using UnityEngine;
using UnityEngine.Events;

public class GhostHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float hurtDelay = 0.5f; // Delay before applying damage

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    [SerializeField] private float hurtSoundDelay = 0.3f; // Delay before playing the hurt sound
    private AudioSource audioSource;

    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;

    private int currentHealth;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;

    public bool IsDead => isDead;

    private Renderer ghostRenderer;
    private Color originalColor; // To store the original color

    private void Awake()
    {
        currentHealth = maxHealth;
        ghostRenderer = GetComponent<Renderer>();
        originalColor = ghostRenderer.material.color; // Store the original color

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;

            // Change back to the original color after invulnerability ends
            if (invulnerabilityTimer <= 0)
            {
                SetGhostColor(originalColor);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || invulnerabilityTimer > 0) return;

        // Start a coroutine to delay the damage application
        StartCoroutine(DelayedDamage(damage));
    }

    private IEnumerator DelayedDamage(int damage)
    {
        yield return new WaitForSeconds(hurtDelay); // Introduce the delay before applying damage

        currentHealth -= damage;
        invulnerabilityTimer = invulnerabilityTime; // Reset invulnerability timer

        OnDamaged?.Invoke();
        Debug.Log($"Ghost took {damage} damage. Current health: {currentHealth}");

        // Change color to red and set opacity to 1
        SetGhostColor(Color.red);
        SetGhostOpacity(1f);

        // Start a coroutine to delay the hurt sound
        StartCoroutine(DelayedHurtSound());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DelayedHurtSound()
    {
        yield return new WaitForSeconds(hurtSoundDelay); // Introduce the delay before playing the hurt sound
        PlaySound(hurtSound);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
        Debug.Log($"{name} has died.");

        PlaySound(deathSound);
        // Set opacity to zero and wait for it to disappear
        StartCoroutine(FadeOutAndDestroy());
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 1f; // Adjust duration as needed
        float startOpacity = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startOpacity, 0f, elapsedTime / fadeDuration);
            SetGhostOpacity(newOpacity);
            yield return null;
        }

        // Ensure opacity is exactly 0 before destroying
        SetGhostOpacity(0f);
        Destroy(gameObject);
    }

    private void SetGhostColor(Color color)
    {
        if (ghostRenderer != null && ghostRenderer.material.HasProperty("_Color"))
        {
            ghostRenderer.material.color = color;
        }
    }

    private void SetGhostOpacity(float opacity)
    {
        if (ghostRenderer != null && ghostRenderer.material.HasProperty("_Color"))
        {
            Color color = ghostRenderer.material.color;
            color.a = opacity;
            ghostRenderer.material.color = color;
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public bool IsInvulnerable()
    {
        return invulnerabilityTimer > 0;
    }
}
