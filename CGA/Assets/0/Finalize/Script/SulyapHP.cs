using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SulyapHP : MonoBehaviour
{

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float hurtDelay = 0.5f;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    [SerializeField] private float hurtSoundDelay = 0.3f;
    private AudioSource audioSource;

    [Header("Animation")]
    public Animator animator; // Reference to the Animator component
    public string hurtAnimationTrigger = "Hurt"; // Animator trigger for hurt animation
    public string deathAnimationTrigger = "Die"; // Animator trigger for death animation

    [Header("Key Drop")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Vector3 keyDropOffset = Vector3.zero;

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

        animator = GetComponent<Animator>(); // Ensure the animator is set
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
        Debug.Log($"Kamatayan took {damage} damage. Current health: {currentHealth}");

        SetGhostColor(Color.red); // Change color to red
        SetGhostOpacity(1f); // Set opacity to 1

        PlaySound(hurtSound);
        animator.SetTrigger(hurtAnimationTrigger);

        // Reset color after invulnerability period
        yield return new WaitForSeconds(invulnerabilityTime);
        SetGhostColor(originalColor); // Reset color to original

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
        Debug.Log($"{name} has died.");

        // Play death sound and animation
        PlaySound(deathSound);
        animator.SetTrigger(deathAnimationTrigger);
        StartCoroutine(FadeOutAndDestroy());

        // Trigger the mob killed event
        GameEvents.MobKilled();

        // Drop the key
        DropKey();
    }

    private void DropKey()
    {
        if (keyPrefab != null)
        {
            Vector3 dropPosition = transform.position + keyDropOffset;
            Instantiate(keyPrefab, dropPosition, Quaternion.identity);
            Debug.Log("Key dropped at position: " + dropPosition);
        }
        else
        {
            Debug.LogWarning("No key prefab assigned!");
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
