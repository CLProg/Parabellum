using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class KamatayanHealthbar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Vector3 offset = Vector3.up * 2f;

    private Canvas canvas;
    private Camera mainCamera;
    private KamatayanHP kamatayanHealth;
    private RectTransform rectTransform;
    private Kamatayan kamatayanBehavior;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        kamatayanHealth = GetComponentInParent<KamatayanHP>();
        kamatayanBehavior = GetComponentInParent<Kamatayan>();

        if (kamatayanHealth == null)
        {
            Debug.LogError("BillboardingHealthBar: No GhostHealth component found in parent!");
        }

        if (kamatayanBehavior == null)
        {
            Debug.LogError("BillboardingHealthBar: No GhostBehavior component found in parent!");
        }

        // Set the initial local position
        transform.localPosition = offset;
    }

    private void LateUpdate()
    {
        if (mainCamera == null || kamatayanHealth == null || kamatayanBehavior == null) return;

        // Calculate the direction from the camera to the health bar
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        directionToCamera.y = 0; // Keep the health bar upright

        // Set the rotation to face the camera
        if (directionToCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }

        // Ensure the health bar scale is correct and matches the ghost's facing direction
        Vector3 scale = transform.localScale;
        scale.x = kamatayanBehavior.facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;

        // Update health bar fill
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = kamatayanHealth.GetHealthPercentage();
        }

        // Hide health bar if ghost is dead
        canvas.enabled = !kamatayanHealth.IsDead;
    }
}