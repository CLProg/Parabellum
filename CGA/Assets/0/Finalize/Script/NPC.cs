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
    public TextMeshProUGUI keyCollectObjectiveText;
    public TextMeshProUGUI portalUnlockObjectiveText;

    [Header("Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode acceptQuestKey = KeyCode.Space;
    public Color completedObjectiveColor = Color.green;

    [Header("Soul Defeat Objective")]
    public int requiredSoulDefeats = 3;

    [Header("Mob Objects")]
    public GameObject[] mobObjects; // Array of mob GameObjects to enable

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
        UpdatePortalUnlockObjective();

        // Subscribe to the mob death event
        GameEvents.OnMobKilled += HandleMobKilled;

        // Disable all mob objects at start
        DisableAllMobObjects();
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

        // Enable mob objects instead of spawning
        EnableMobObjects();
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
        return objectiveCompleted.TrueForAll(completed => completed)
            && currentSoulDefeats >= requiredSoulDefeats
            && IsKeyCollected()
            && portalUnlockObjectiveText.color == completedObjectiveColor;
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

    public void UpdateKeyCollectObjective()
    {
        if (keyCollectObjectiveText != null)
        {
            keyCollectObjectiveText.color = completedObjectiveColor;
            keyCollectObjectiveText.text = "- You got the Golden Key from Sulyap.";
            Debug.Log("Key collection objective completed!");
        }
    }

    public void UpdatePortalUnlockObjective()
    {
        if (portalUnlockObjectiveText != null)
        {
            portalUnlockObjectiveText.text = "- Use the Golden Key to unlock the portal.";
        }
    }

    public void CompletePortalUnlockObjective()
    {
        if (portalUnlockObjectiveText != null)
        {
            portalUnlockObjectiveText.color = completedObjectiveColor;
            portalUnlockObjectiveText.text = "- Used the Golden Key to unlock the portal.";
            Debug.Log("Portal unlock objective completed!");
        }
    }

    public bool IsKeyCollected()
    {
        return keyCollectObjectiveText.color == completedObjectiveColor;
    }

    // New method to disable all mob objects at start
    private void DisableAllMobObjects()
    {
        foreach (GameObject mobObject in mobObjects)
        {
            if (mobObject != null)
            {
                mobObject.SetActive(false);
            }
        }
        Debug.Log("All mob objects disabled at start.");
    }

    // New method to enable mob objects when the quest is accepted
    private void EnableMobObjects()
    {
        int enabledCount = 0;
        foreach (GameObject mobObject in mobObjects)
        {
            if (mobObject != null)
            {
                mobObject.SetActive(true);
                enabledCount++;
            }
        }
        Debug.Log($"Enabled {enabledCount} mob objects.");
    }
}