using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFillImage; // Reference to the health fill Image

    // Call this method to update the health bar
    public void UpdateHealthBar(float healthPercentage)
    {
        healthFillImage.fillAmount = healthPercentage; // Update the fill amount
    }
}
