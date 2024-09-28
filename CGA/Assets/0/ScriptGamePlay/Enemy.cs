using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 50; // Current enemy health
    public int maxHealth = 50; // Maximum enemy health
    public HealthBar healthBar; // Reference to the health bar
    public float damageDelay = 1f; // Delay for health bar update
    public GameObject damageTextPrefab; // Reference to the normal damage text prefab
    public Vector3 damageTextOffset = new Vector3(0, -0.3f, 0); // Default offset for damage text
    public Transform hpBarCanvas; // Reference to the health bar canvas

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

        // Show damage text
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
        TextMesh textMesh = textInstance.GetComponent<TextMesh>();
        textMesh.text = damage.ToString();
        textInstance.transform.localPosition += damageTextOffset;
        StartCoroutine(DestroyTextAfterDelay(textInstance, 1f));
    }

    private IEnumerator DestroyTextAfterDelay(GameObject textInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(textInstance); // Destroy the text instance
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = (float)health / maxHealth;
        healthBar.UpdateHealthBar(healthPercentage); // Call the method to update health bar
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Handle enemy death (e.g., play animation, destroy object)
        Destroy(hpBarCanvas.gameObject); // Destroy the health bar canvas
        Destroy(gameObject); // Destroy the enemy object
    }

    private void Update()
    {
        // Update the position of the health bar canvas above the enemy
        hpBarCanvas.position = new Vector3(transform.position.x, transform.position.y + 0.9f, transform.position.z);
    }
}
