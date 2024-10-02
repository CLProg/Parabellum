using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestSystem : MonoBehaviour
{
    [System.Serializable]
    public class Quest
    {
        public string questName;
        public int mobsToKill;
        public int mobsKilled;
        public bool stoneCollected;
        public bool isCompleted;
    }

    public Quest currentQuest;
    public Text questUIText;           // Text for quest status on screen
    public GameObject questGiver;      // Reference to the quest giver GameObject
    public GameObject bossRoomTeleporter; // Reference to the boss room teleporter
    public GameObject questWindow;      // Reference to the quest window panel
    public Text questInfoText;          // Reference to the text that displays quest information

    private void Start()
    {
        InitializeQuest();
        UpdateQuestUI();
        bossRoomTeleporter.SetActive(false);
        HideQuestWindow(); // Ensure the quest window is hidden at the start
    }

    private void Update()
    {
        // Check for E key press to toggle quest window
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleQuestWindow();
        }
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
        UpdateQuestInfo();           // Update the quest info text
    }

    private void HideQuestWindow()
    {
        questWindow.SetActive(false); // Hide the quest window
    }

    private void UpdateQuestInfo()
    {
        questInfoText.text = $"Quest: {currentQuest.questName}\n" +
                             $"Description: Kill 2 mobs and obtain a stone to open the portal to the boss room.\n" +
                             $"Status: {(currentQuest.isCompleted ? "Completed" : "In Progress")}\n" +
                             $"Mobs Killed: {currentQuest.mobsKilled}/{currentQuest.mobsToKill}\n" +
                             $"Stone Collected: {(currentQuest.stoneCollected ? "Yes" : "No")}";
    }

    private void InitializeQuest()
    {
        currentQuest = new Quest
        {
            questName = "Defeat the Enemies",
            mobsToKill = 2,
            mobsKilled = 0,
            stoneCollected = false,
            isCompleted = false
        };
    }

    public void KillMob(bool dropsStone)
    {
        if (currentQuest.mobsKilled < currentQuest.mobsToKill)
        {
            currentQuest.mobsKilled++;

            if (dropsStone && !currentQuest.stoneCollected)
            {
                currentQuest.stoneCollected = true;
            }

            CheckQuestCompletion();
            UpdateQuestUI();
        }
    }

    private void CheckQuestCompletion()
    {
        if (currentQuest.mobsKilled >= currentQuest.mobsToKill && currentQuest.stoneCollected)
        {
            currentQuest.isCompleted = true;
            bossRoomTeleporter.SetActive(true);
        }
    }

    private void UpdateQuestUI()
    {
        string questStatus = currentQuest.isCompleted ? "Completed" : "In Progress";
        questUIText.text = $"Quest: {currentQuest.questName}\n" +
                           $"Status: {questStatus}\n" +
                           $"Mobs Killed: {currentQuest.mobsKilled}/{currentQuest.mobsToKill}\n" +
                           $"Stone Collected: {(currentQuest.stoneCollected ? "Yes" : "No")}";
    }

    public void TeleportToBossRoom()
    {
        if (currentQuest.isCompleted)
        {
            SceneManager.LoadScene("BossRoom"); // Replace "BossRoom" with your actual scene name
        }
    }
}
