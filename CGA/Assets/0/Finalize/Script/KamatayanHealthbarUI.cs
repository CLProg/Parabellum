using UnityEngine;
using UnityEngine.UI;

public class KamatayanHealthbar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

    private KamatayanHP kamatayanHealth;

    private void Awake()
    {
        kamatayanHealth = GetComponentInParent<KamatayanHP>(); // Use GetComponentInParent

        if (kamatayanHealth == null)
            Debug.LogError("KamatayanHealthbar: No KamatayanHP component found in parent!");
    }

    private void LateUpdate()
    {
        if (kamatayanHealth == null) return;

        // Update health bar fill
        if (healthBarFill != null)
            healthBarFill.fillAmount = kamatayanHealth.GetHealthPercentage();

        // Hide health bar if dead
        gameObject.SetActive(!kamatayanHealth.IsDead);
    }
}
