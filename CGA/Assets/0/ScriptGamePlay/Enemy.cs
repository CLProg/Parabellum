using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 50; // Current enemy health
    public int maxHealth = 50; // Maximum enemy health
    public HealthBar healthBar; // Reference to the health bar
    public float damageDelay = 1f; // Delay for health bar update
    public float damageTextShowDelay = 0.5f; // Delay before showing damage text
    public float damageTextDisplayDuration = 1f; // Duration for how long damage text is displayed
    public GameObject damageTextPrefab; // Reference to the normal damage text prefab
    public Vector3 offset = new Vector3(0, 2, 0); // Default offset

    private void Start()
    {
        UpdateHealthBar(); // Initialize the health bar with full health
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(HandleDamage(damage));
    }

    private IEnumerator HandleDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {health}");

        // Show damage text after a delay
        yield return new WaitForSeconds(damageTextShowDelay);
        ShowDamageText(damage);

        // Delay before updating the health bar
        yield return new WaitForSeconds(damageDelay);

        UpdateHealthBar(); // Update the health bar

        if (health <= 0)
        {
            Die();
        }
    }

    private void ShowDamageText(int damage)
    {
        // Instantiate damage text prefab
        GameObject textInstance = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);

        // Set the damage text
        TextMesh textMesh = textInstance.GetComponent<TextMesh>();
        textMesh.text = damage.ToString();

        // Apply the offset to the local position of the damage text
        textInstance.transform.localPosition += offset;

        // Start coroutine to destroy the text after damageTextDisplayDuration
        StartCoroutine(DestroyTextAfterDelay(textInstance, damageTextDisplayDuration));
    }

    private IEnumerator DestroyTextAfterDelay(GameObject textInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(textInstance); // Destroy the text instance
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = (float)health / maxHealth;
        healthBar.UpdateHealthBar(healthPercentage);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Handle enemy death (e.g., play animation, destroy object)
        Destroy(gameObject);
    }
}
