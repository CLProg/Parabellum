using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    [Header("References")]
    public GameObject portalCube;
    public GameObject promptMessage;
    public GameObject interactionButton;
    public NPC npc;
    public Animator canvasAnimator; // Reference to Animator controlling fade animations

    [Header("Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public string destinationSceneName; // Name of the scene to teleport to
    public float teleportDelay = 2f; // Delay before teleporting

    [Header("Child Objects")]
    public List<GameObject> childObjectsToToggle; // List of child objects to hide/show
    public GameObject[] canvasesToHide; // Array of canvases to hide

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
        HideToggleableChildObjects(); // Hide specified child objects on start
        // Hide specified canvases on start
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
            UpdatePromptText();
        }
    }

    private void UpdatePromptText()
    {
        bool hasKey = npc.IsKeyCollected();
        bool isSoulObjectiveCompleted = IsSoulObjectiveCompleted();

        if (hasKey && isSoulObjectiveCompleted)
        {
            promptText.text = "Press 'E' to open the portal.";
        }
        else if (!hasKey && !isSoulObjectiveCompleted)
        {
            promptText.text = "You need the Golden Key and defeat all souls to open this.";
        }
        else if (!hasKey)
        {
            promptText.text = "You need the Golden Key to open this.";
        }
        else // !isSoulObjectiveCompleted
        {
            promptText.text = "You need to defeat all souls to open this.";
        }
    }

    private bool IsSoulObjectiveCompleted()
    {
        // Check if the soul defeat objective text is green
        return npc.soulDefeatObjectiveText.color == npc.completedObjectiveColor;
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
        bool hasKey = npc.IsKeyCollected();
        bool isSoulObjectiveCompleted = IsSoulObjectiveCompleted();

        if (hasKey && isSoulObjectiveCompleted)
        {
            OpenPortal(); // Open the portal
        }
        else
        {
            if (!hasKey && !isSoulObjectiveCompleted)
            {
                Debug.Log("You need the Golden Key and to defeat all souls to open this.");
            }
            else if (!hasKey)
            {
                Debug.Log("You need the Golden Key to open this.");
            }
            else // !isSoulObjectiveCompleted
            {
                Debug.Log("You need to defeat all souls to open this.");
            }
            UpdatePromptText(); // Update the prompt text to reflect the current status
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

        // Show specified child objects when the portal is opened
        ShowToggleableChildObjects();

        // Complete the portal unlock objective
        npc.CompletePortalUnlockObjective();

        // Start the teleportation process
        StartCoroutine(TeleportPlayer());
    }

    private IEnumerator TeleportPlayer()
    {
        // Hide the specified canvases before the fade-out animation
        HideCanvases();

        // Play the fade-out animation
        canvasAnimator.SetTrigger("Start");

        // Wait for the duration of the fade-out animation
        yield return new WaitForSeconds(1.0f); // Replace with the actual duration if known

        // Check if the destination scene name is set
        if (string.IsNullOrEmpty(destinationSceneName))
        {
            Debug.LogError("Destination scene name is not set!");
            yield break;
        }

        // Load the new scene
        SceneManager.LoadScene(destinationSceneName);
    }


    private void HideToggleableChildObjects()
    {
        foreach (GameObject child in childObjectsToToggle)
        {
            if (child != null)
            {
                child.SetActive(false);
            }
        }
    }

    private void ShowToggleableChildObjects()
    {
        foreach (GameObject child in childObjectsToToggle)
        {
            if (child != null)
            {
                child.SetActive(true);
            }
        }
    }

    private void HideCanvases()
    {
        foreach (GameObject canvas in canvasesToHide)
        {
            if (canvas != null)
            {
                canvas.SetActive(false); // Hide each specified canvas
            }
        }
    }
}
