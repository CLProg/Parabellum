using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GhostHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float hurtDelay = 0.5f;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    [SerializeField] private float hurtSoundDelay = 0.3f;
    private AudioSource audioSource;

    public UnityEvent OnDamaged;
    public UnityEvent OnDeath;

    private int currentHealth;
    private float invulnerabilityTimer = 0f;
    private bool isDead = false;

    public bool IsDead => isDead;

    private Renderer ghostRenderer;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = maxHealth;
        ghostRenderer = GetComponent<Renderer>();
        originalColor = ghostRenderer.material.color;

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

            if (invulnerabilityTimer <= 0)
            {
                SetGhostColor(originalColor);
            }
        }
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
        Debug.Log($"Ghost took {damage} damage. Current health: {currentHealth}");

        SetGhostColor(Color.red);
        SetGhostOpacity(1f);

        StartCoroutine(DelayedHurtSound());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DelayedHurtSound()
    {
        yield return new WaitForSeconds(hurtSoundDelay);
        PlaySound(hurtSound);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
        Debug.Log($"{name} has died.");

        PlaySound(deathSound);
        StartCoroutine(FadeOutAndDestroy());

        // Trigger the mob killed event
        GameEvents.MobKilled();
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
        float fadeDuration = 1f;
        float startOpacity = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newOpacity = Mathf.Lerp(startOpacity, 0f, elapsedTime / fadeDuration);
            SetGhostOpacity(newOpacity);
            yield return null;
        }

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
