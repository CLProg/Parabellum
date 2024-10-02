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

    [Header("Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode acceptQuestKey = KeyCode.Space;
    public Color completedObjectiveColor = Color.green;

    private bool isPlayerInRange = false;
    private Canvas questWindowCanvas;
    private Canvas interactionButtonCanvas;
    private List<bool> objectiveCompleted;

    private void Awake()
    {
        questWindowCanvas = questWindow.GetComponent<Canvas>();
        interactionButtonCanvas = GetComponentInChildren<Canvas>();

        if (questWindowCanvas == null || interactionButtonCanvas == null)
        {
            Debug.LogError("Canvas components not found. Please check the setup.");
        }

        // Initialize objective completion status
        objectiveCompleted = new List<bool>(new bool[objectiveTexts.Count]);
    }

    private void Start()
    {
        HideQuestWindow();
        HideCurrentQuestCanvas();
    }

    private void Update()
    {
        CheckPlayerDistance();
        HandleInputs();
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

                if (!currentQuestCanvas.activeSelf)
                {
                    HideQuestWindow();
                }
            }
        }
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

        // For testing: Press 1-4 to complete objectives
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                CompleteObjective(i);
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
        Debug.Log("Quest accepted!");
        ShowCurrentQuestCanvas();
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

    public void CompleteObjective(int index)
    {
        if (index >= 0 && index < objectiveTexts.Count)
        {
            objectiveCompleted[index] = true;
            objectiveTexts[index].color = completedObjectiveColor;
            Debug.Log($"Objective {index + 1} completed!");
        }
        else
        {
            Debug.LogError($"Invalid objective index: {index}");
        }
    }

    public bool IsQuestCompleted()
    {
        return objectiveCompleted.TrueForAll(completed => completed);
    }
}