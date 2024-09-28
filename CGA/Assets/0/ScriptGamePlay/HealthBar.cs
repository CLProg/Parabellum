using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;

    public void UpdateHealthBar(float healthPercentage)
    {
        healthBarImage.fillAmount = healthPercentage; // Assuming you're using a filled image type
    }
}
