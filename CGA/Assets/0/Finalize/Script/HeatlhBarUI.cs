using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthBarImage;

    private void Start()
    {
        // Assign playerHealth if not already assigned
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth component not found in the scene.");
                return;
            }
        }

        // Assign healthBarImage if not already assigned
        if (healthBarImage == null)
        {
            healthBarImage = GetComponent<Image>();
            if (healthBarImage == null)
            {
                Debug.LogError("HealthBar Image component is missing.");
                return;
            }
        }

        // Link the health bar in the PlayerHealth script
        playerHealth.healthBar = healthBarImage;
    }
}
