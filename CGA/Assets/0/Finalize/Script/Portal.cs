using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("References")]
    public GameObject portalCube;
    public GameObject promptMessage;
    public GameObject interactionButton;
    public NPC npc;

    [Header("Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public string destinationSceneName; // Name of the scene to teleport to
    public float teleportDelay = 2f; // Delay before teleporting

    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private bool isPortalOpen = false;
    private TextMeshProUGUI promptText;

    private void Awake()
    {
        playerTransform = Camera.main.transform;
        promptMessage.SetActive(false);
        interactionButton.SetActive(false);
        promptText = promptMessage.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        CheckPlayerDistance();
        HandleInput();
    }

    private void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer <= interactionDistance)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true; // Player is in range
                ShowPromptMessage(); // Show prompt message
                ShowInteractionButton(); // Show the interaction button
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false; // Player is out of range
                HidePromptMessage(); // Hide prompt message
                HideInteractionButton(); // Hide the interaction button
            }
        }
    }

    private void ShowPromptMessage()
    {
        // Only show the prompt message if the portal is not open
        if (!isPortalOpen)
        {
            promptMessage.SetActive(true); // Activate the prompt message UI element
            // Check if the key objective is completed
            bool hasKey = npc.IsKeyCollected(); // Check if the key is collected
            // Update prompt message text based on key status using TextMeshPro
            promptText.text = hasKey ? "Press 'E' to open the portal." : "You need a key to open this.";
        }
    }

    private void HidePromptMessage()
    {
        promptMessage.SetActive(false); // Deactivate the prompt message UI element
    }

    private void ShowInteractionButton()
    {
        interactionButton.SetActive(true); // Activate the interaction button UI element
    }

    private void HideInteractionButton()
    {
        interactionButton.SetActive(false); // Deactivate the interaction button UI element
    }

    private void HandleInput()
    {
        // Check for interaction input if the player is in range
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            TryOpenPortal(); // Try to open the portal
        }
    }

    private void TryOpenPortal()
    {
        // Check if the key objective is completed
        if (npc.IsKeyCollected()) // Check if the key is collected
        {
            OpenPortal(); // Open the portal
        }
        else
        {
            Debug.Log("You need a key to open this."); // Log if key is not collected
        }
    }

    private void OpenPortal()
    {
        Renderer renderer = portalCube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }

        Debug.Log("Portal opened!");

        isPortalOpen = true;
        HidePromptMessage();
        HideInteractionButton();

        // Complete the portal unlock objective (assuming you've added this method to the NPC script)
        npc.CompletePortalUnlockObjective();

        // Start the teleportation process
        StartCoroutine(TeleportPlayer());
    }
    private IEnumerator TeleportPlayer()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(teleportDelay);

        // Check if the destination scene name is set
        if (string.IsNullOrEmpty(destinationSceneName))
        {
            Debug.LogError("Destination scene name is not set!");
            yield break;
        }

        // Teleport the player to the new scene
        SceneManager.LoadScene(destinationSceneName);
    }
}
