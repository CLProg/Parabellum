using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // Include SceneManagement namespace

public class KamatayanHP : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float hurtDelay = 0.5f;

    [Header("Audio Settings")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip bossSound; // Sound to play when the player is near the boss
    [SerializeField] private float hurtSoundDelay = 0.3f;
    [SerializeField] private float bossSoundMaxVolume = 1f; // Maximum volume of the boss sound
    [SerializeField] private float detectionRange = 10f; // Range at which the sound reaches maximum volume
    private AudioSource audioSource;
    private AudioSource bossAudioSource; // Separate AudioSource for the boss sound

    [Header("Animation Settings")]
    public Animator animator;
    public string hurtAnimationTrigger = "Hurt";
    public string deathAnimationTrigger = "Die";

    [Header("Monster Spawn Settings")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float spawnInterval = 4f;

    [Header("UI Settings")]
    [SerializeField] private GameObject endScreenCanvas; // Reference to your end screen Canvas object

    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform; // Reference to the player

    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;

    private int currentHealth;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;
    private Coroutine spawnCoroutine;
    private Renderer ghostRenderer;
    private Color originalColor;

    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        ghostRenderer = GetComponent<Renderer>();
        originalColor = ghostRenderer.material.color;

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // Create a separate AudioSource for the boss sound and set it to loop
        bossAudioSource = gameObject.AddComponent<AudioSource>();
        bossAudioSource.clip = bossSound;
        bossAudioSource.loop = true;
        bossAudioSource.volume = 0f; // Start at 0 volume
        bossAudioSource.playOnAwake = false;
        bossAudioSource.spatialBlend = 1f; // Make the sound 3D (optional)
        bossAudioSource.maxDistance = detectionRange; // Adjust max range for 3D sound (optional)
        bossAudioSource.Play(); // Start playing but volume will be adjusted dynamically

        animator = GetComponent<Animator>();
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

        if (endScreenCanvas != null)
        {
            endScreenCanvas.SetActive(false); // Ensure the end screen is hidden at the start
        }
    }

    private void Update()
    {
        HandleInvulnerabilityTimer();
        AdjustBossSoundBasedOnDistance();
    }

    private void AdjustBossSoundBasedOnDistance()
    {
        if (playerTransform == null || bossAudioSource == null) return;

        // Calculate distance between player and boss
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Calculate volume based on distance (the closer the louder)
        float volume = Mathf.Clamp01(1 - (distanceToPlayer / detectionRange)) * bossSoundMaxVolume;

        // Apply the volume to the bossAudioSource
        bossAudioSource.volume = volume;
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
        SetGhostColor(Color.red);
        SetGhostOpacity(1f);
        PlaySound(hurtSound);
        animator.SetTrigger(hurtAnimationTrigger);

        StartCoroutine(ResetGhostColor());
    }

    private IEnumerator ResetGhostColor()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        SetGhostColor(originalColor);
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

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        StartCoroutine(EndGame());
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
        Destroy(gameObject); // This will now only happen after the fade-out is complete
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

    private IEnumerator EndGame()
    {
        Debug.Log("Game Ended: Victory or Defeat");

        // Wait for a short period before transitioning
        yield return new WaitForSeconds(1f); // Optional delay for effect

        // Load the end scene
        SceneManager.LoadScene("ENDING"); // Ensure this matches the name of your end scene
    }
}
