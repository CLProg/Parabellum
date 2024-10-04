using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KamatayanHP : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float hurtDelay = 0.5f;

    [Header("Audio Settings")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    [SerializeField] private float hurtSoundDelay = 0.3f;
    private AudioSource audioSource;

    [Header("Animation Settings")]
    public Animator animator; // Reference to the Animator component
    public string hurtAnimationTrigger = "Hurt"; // Animator trigger for hurt animation
    public string deathAnimationTrigger = "Die"; // Animator trigger for death animation

    [Header("Monster Spawn Settings")]
    [SerializeField] private GameObject monsterPrefab; // The monster prefab to spawn
    [SerializeField] private Transform spawnPosition; // Position where the monster will spawn
    [SerializeField] private float spawnInterval = 4f; // Time interval for spawning monsters

    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;

    private int currentHealth;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;
    private Coroutine spawnCoroutine; // To track the monster spawning coroutine
    private Renderer ghostRenderer;
    private Color originalColor;

    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        ghostRenderer = GetComponent<Renderer>();
        originalColor = ghostRenderer.material.color;

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        animator = GetComponent<Animator>(); // Ensure the animator is set
    }

    private void Start()
    {
        if (spawnPosition != null && monsterPrefab != null)
        {
            spawnCoroutine = StartCoroutine(SpawnMonsters());
        }
        else
        {
            Debug.LogWarning("Spawn position or monster prefab is not assigned.");
        }
    }

    private void Update()
    {
        HandleInvulnerabilityTimer();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || invulnerabilityTimer > 0) return;

        StartCoroutine(DelayedDamage(damage));
    }

    private IEnumerator DelayedDamage(int damage)
    {
        yield return new WaitForSeconds(hurtDelay);

        currentHealth -= damage;
        invulnerabilityTimer = invulnerabilityTime;

        OnDamaged?.Invoke();
        Debug.Log($"Kamatayan took {damage} damage. Current health: {currentHealth}");

        TriggerHurtEffects();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void TriggerHurtEffects()
    {
        SetGhostColor(Color.red); // Change color to red
        SetGhostOpacity(1f); // Set opacity to 1
        PlaySound(hurtSound);
        animator.SetTrigger(hurtAnimationTrigger);

        // Reset color after invulnerability period
        StartCoroutine(ResetGhostColor());
    }

    private IEnumerator ResetGhostColor()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        SetGhostColor(originalColor); // Reset color to original
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
        Debug.Log($"{name} has died.");

        PlaySound(deathSound);
        animator.SetTrigger(deathAnimationTrigger);
        StartCoroutine(FadeOutAndDestroy());

        // Stop spawning monsters when dead
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnMonsters()
    {
        while (!isDead)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMonster()
    {
        if (monsterPrefab != null && spawnPosition != null)
        {
            Instantiate(monsterPrefab, spawnPosition.position, Quaternion.identity);
            Debug.Log($"Spawned a monster at position: {spawnPosition.position}");
        }
        else
        {
            Debug.LogWarning("No monster prefab or spawn position assigned!");
        }
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
        const float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newOpacity = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            SetGhostOpacity(newOpacity);
            yield return null;
        }

        SetGhostOpacity(0f);
        Destroy(gameObject);
    }

    private void HandleInvulnerabilityTimer()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;

            if (invulnerabilityTimer <= 0)
            {
                SetGhostColor(originalColor);
            }
        }
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

    public float GetHealthPercentage() => (float)currentHealth / maxHealth;

    public bool IsInvulnerable() => invulnerabilityTimer > 0;
}
