using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    public GameObject questWindow; // Reference to the quest window Panel
    public GameObject currentQuestCanvas; // Reference to the current quest canvas
    public float interactionDistance = 3f; // Distance within which the player can interact
    public Transform playerTransform; // Reference to the player's transform
    public Transform npcTransform; // Reference to the NPC's transform

    private bool isPlayerInRange = false; // To track if player is in range of NPC
    private Canvas questWindowCanvas;
    private Canvas interactionButtonCanvas;

    private void Awake()
    {
        questWindowCanvas = questWindow.GetComponent<Canvas>();
        interactionButtonCanvas = GetComponentInChildren<Canvas>();

        if (questWindowCanvas == null || interactionButtonCanvas == null)
        {
            Debug.LogError("Canvas components not found. Please check the setup.");
        }
    }

    private void Start()
    {
        HideQuestWindow(); // Ensure the quest window is hidden at the start
        HideCurrentQuestCanvas(); // Ensure the current quest canvas is hidden at the start
    }

    private void Update()
    {
        // Check distance between player and NPC
        float distanceToNPC = Vector3.Distance(playerTransform.position, npcTransform.position);

        if (distanceToNPC <= interactionDistance)
        {
            isPlayerInRange = true;
            ShowInteractionPrompt();
            EnableCanvases(true);
        }
        else
        {
            isPlayerInRange = false;
            HideInteractionPrompt();
            EnableCanvases(false);

            // Only hide the quest window if the current quest canvas is not active
            if (!currentQuestCanvas.activeSelf)
            {
                HideQuestWindow(); // Ensure quest window is closed when out of range
            }
        }

        // Check for E key press to show quest window when in range
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleQuestWindow();
        }

        // Check for the space key press to accept the quest when the quest window is open
        if (questWindow.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            AcceptQuest();
        }
    }

    private void ShowInteractionPrompt()
    {
        // Optionally show a prompt to the player (like a UI text or icon)
        Debug.Log("Press 'E' to interact with the Quest Giver.");
    }

    private void HideInteractionPrompt()
    {
        // Optionally hide the interaction prompt
        Debug.Log("Out of range of Quest Giver.");
    }

    private void ToggleQuestWindow()
    {
        if (questWindow.activeSelf)
        {
            HideQuestWindow();
        }
        else
        {
            ShowQuestWindow();
        }
    }

    private void ShowQuestWindow()
    {
        questWindow.SetActive(true); // Show the quest window
    }

    private void HideQuestWindow()
    {
        questWindow.SetActive(false); // Hide the quest window
    }

    private void EnableCanvases(bool enable)
    {
        if (questWindowCanvas != null)
        {
            questWindowCanvas.enabled = enable;
        }

        if (interactionButtonCanvas != null)
        {
            interactionButtonCanvas.enabled = enable;
        }
    }

    private void AcceptQuest()
    {
        Debug.Log("Quest accepted!"); // Debug message for acceptance
        ShowCurrentQuestCanvas(); // Show the current quest canvas
        // Do not hide the quest window here, so it remains open.
    }

    private void ShowCurrentQuestCanvas()
    {
        if (currentQuestCanvas != null)
        {
            currentQuestCanvas.SetActive(true); // Show the current quest canvas
            Debug.Log("Current quest canvas shown."); // Add this line
        }
        else
        {
            Debug.LogError("Current quest canvas not assigned. Please check the setup.");
        }
    }

    private void HideCurrentQuestCanvas()
    {
        if (currentQuestCanvas != null)
        {
            currentQuestCanvas.SetActive(false); // Hide the current quest canvas
        }
    }
}
