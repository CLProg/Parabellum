using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health the player can have
    public int currentHealth; // Current health of the player
    public Transform respawnPoint; // The respawn location
    public float respawnDelay = 2f; // Time delay before respawning the player
    public float invulnerabilityDuration = 2f; // Duration of invulnerability after taking damage or respawning

    private bool isDead = false; // Check if the player is dead
    private bool isInvulnerable = false; // Check if the player is currently invulnerable

    void Start()
    {
        currentHealth = maxHealth; // Set the player's health to max at the start
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return; // Do not take damage if the player is dead or invulnerable

        currentHealth -= damage; // Reduce the player's health by the damage amount
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        // Check if the player's health has reached zero
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartInvulnerability();
        }
    }

    // Method to handle player's death
    void Die()
    {
        if (isDead) return; // Prevent multiple death calls
        isDead = true; // Mark player as dead
        Debug.Log("Player has died!");
        // Trigger death animation or any additional effects (optional)
        // Delay respawn to allow for any death animations or effects
        Invoke(nameof(Respawn), respawnDelay);
    }

    // Method to respawn the player at the respawn point
    void Respawn()
    {
        isDead = false; // Reset death state
        currentHealth = maxHealth; // Restore full health
        // Move the player to the respawn point
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        Debug.Log("Player has respawned at the respawn point.");
        StartInvulnerability();
    }

    // Method to heal the player (optional, in case you need healing functionality)
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // Method to start invulnerability
    void StartInvulnerability()
    {
        isInvulnerable = true;
        Debug.Log("Player is now invulnerable.");
        Invoke(nameof(EndInvulnerability), invulnerabilityDuration);
    }

    // Method to end invulnerability
    void EndInvulnerability()
    {
        isInvulnerable = false;
        Debug.Log("Player is no longer invulnerable.");
    }
}
