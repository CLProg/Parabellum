using UnityEngine;
using TMPro; // For TextMeshPro UI elements

public class Portal : MonoBehaviour
{
    [Header("References")]
    public GameObject portalCube; // The 3D object representing the portal
    public GameObject promptMessage; // UI element to display the prompt
    public GameObject interactionButton; // Reference to the interaction button
    public NPC npc; // Reference to the NPC script to check the quest objectives

    [Header("Settings")]
    public float interactionDistance = 3f; // Distance within which the player can interact with the portal
    public KeyCode interactionKey = KeyCode.E; // Key to interact with the portal

    private Transform playerTransform; // Player's transform
    private bool isPlayerInRange = false; // Whether the player is in range to interact
    private bool isPortalOpen = false; // Whether the portal is open
    private TextMeshProUGUI promptText; // Reference to the TextMeshPro component

    private void Awake()
    {
        playerTransform = Camera.main.transform; // Assuming the main camera represents the player
        promptMessage.SetActive(false); // Hide the prompt message initially
        interactionButton.SetActive(false); // Hide the interaction button initially

        // Get the TextMeshProUGUI component from the promptMessage GameObject
        promptText = promptMessage.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        CheckPlayerDistance(); // Check if player is within interaction distance
        HandleInput(); // Handle user input for interaction
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
        // Change the portal cube color from gray to white
        Renderer renderer = portalCube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white; // Change color to white
        }

        Debug.Log("Portal opened!");

        // Mark the portal as open and hide the prompt and button
        isPortalOpen = true; // Set the portal state to open
        HidePromptMessage(); // Hide the prompt message
        HideInteractionButton(); // Hide the interaction button

        // Add additional logic for what happens when the portal is opened
    }
}
