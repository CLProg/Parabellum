using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    [Header("References")]
    public GameObject questWindow;
    public GameObject currentQuestCanvas;
    public Transform playerTransform;
    public Transform npcTransform;

    [Header("Quest Objectives")]
    public List<TextMeshProUGUI> objectiveTexts;
    public TextMeshProUGUI soulDefeatObjectiveText;
    public TextMeshProUGUI keyCollectObjectiveText; // Reference to the key collection objective text

    [Header("Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode acceptQuestKey = KeyCode.Space;
    public Color completedObjectiveColor = Color.green;

    [Header("Soul Defeat Objective")]
    public int requiredSoulDefeats = 3;

    private bool isPlayerInRange = false;
    private Canvas questWindowCanvas;
    private Canvas interactionButtonCanvas;
    private List<bool> objectiveCompleted;
    private int currentSoulDefeats = 0;
    private bool questAccepted = false;

    private void Awake()
    {
        questWindowCanvas = questWindow.GetComponent<Canvas>();
        interactionButtonCanvas = GetComponentInChildren<Canvas>();

        if (questWindowCanvas == null || interactionButtonCanvas == null)
        {
            Debug.LogError("Canvas components not found. Please check the setup.");
        }

        objectiveCompleted = new List<bool>(new bool[objectiveTexts.Count]);
    }

    private void Start()
    {
        HideQuestWindow();
        HideCurrentQuestCanvas();
        UpdateSoulDefeatObjective();
        // Remove the call to UpdateKeyCollectObjective here

        // Subscribe to the mob death event
        GameEvents.OnMobKilled += HandleMobKilled;
    }

    private void OnDisable()
    {
        // Unsubscribe from the mob death event when the script is disabled
        GameEvents.OnMobKilled -= HandleMobKilled;
    }

    private void Update()
    {
        CheckPlayerDistance();
        HandleInputs();
    }

    private void HandleInputs()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            ToggleQuestWindow();
        }

        if (questWindow.activeSelf && Input.GetKeyDown(acceptQuestKey))
        {
            AcceptQuest();
        }
    }

    private void CheckPlayerDistance()
    {
        float distanceToNPC = Vector3.Distance(playerTransform.position, npcTransform.position);

        if (distanceToNPC <= interactionDistance)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                ShowInteractionPrompt();
                EnableCanvases(true);
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                HideInteractionPrompt();
                EnableCanvases(false);

                if (!questAccepted)
                {
                    HideQuestWindow();
                }
            }
        }
    }

    private void ShowInteractionPrompt()
    {
        Debug.Log($"Press '{interactionKey}' to interact with the Quest Giver.");
    }

    private void HideInteractionPrompt()
    {
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
        questWindow.SetActive(true);
    }

    private void HideQuestWindow()
    {
        questWindow.SetActive(false);
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
        questAccepted = true;
        Debug.Log("Quest accepted!");
        ShowCurrentQuestCanvas();
        HideQuestWindow();
    }

    private void ShowCurrentQuestCanvas()
    {
        if (currentQuestCanvas != null)
        {
            currentQuestCanvas.SetActive(true);
            Debug.Log("Current quest canvas shown.");
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
            currentQuestCanvas.SetActive(false);
        }
    }

    public bool IsQuestCompleted()
    {
        return objectiveCompleted.TrueForAll(completed => completed) && currentSoulDefeats >= requiredSoulDefeats;
    }

    private void HandleMobKilled()
    {
        if (questAccepted)
        {
            UpdateSoulDefeats(1);
        }
    }

    private void UpdateSoulDefeats(int defeats)
    {
        currentSoulDefeats += defeats;
        if (currentSoulDefeats > requiredSoulDefeats)
        {
            currentSoulDefeats = requiredSoulDefeats;
        }
        UpdateSoulDefeatObjective();

        if (currentSoulDefeats >= requiredSoulDefeats)
        {
            CompleteSoulDefeatObjective();
        }
    }

    private void UpdateSoulDefeatObjective()
    {
        if (soulDefeatObjectiveText != null)
        {
            soulDefeatObjectiveText.text = $"- Defeat all souls in the area. ({currentSoulDefeats}/{requiredSoulDefeats})";
        }
    }

    private void CompleteSoulDefeatObjective()
    {
        if (soulDefeatObjectiveText != null)
        {
            soulDefeatObjectiveText.color = completedObjectiveColor;
        }
        Debug.Log("Soul defeat objective completed!");
    }

    // New method to update key collection objective
    public void UpdateKeyCollectObjective()
    {
        if (keyCollectObjectiveText != null)
        {
            keyCollectObjectiveText.color = completedObjectiveColor; // Change color to green
            keyCollectObjectiveText.text = "- You got the Golden Key from Sulyap."; // Update the text to indicate completion
            Debug.Log("Key collection objective completed!");
        }
    }
    public bool IsKeyCollected()
    {
        // Assuming the text is set to indicate completion in UpdateKeyCollectObjective
        return keyCollectObjectiveText.color == completedObjectiveColor;
    }
}
