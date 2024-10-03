using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyInteract : MonoBehaviour
{
    [SerializeField] private float pickupRange = 2f; // Range within which the player can pick up the key
    private bool isPlayerInRange = false;

    private Image eKeyImage; // Reference to the 'E' image
    private AudioSource audioSource; // Reference to the AudioSource for playing sounds
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound; // The sound to play on pickup

    private Camera mainCamera; // Reference to the main camera
    private NPC npc; // Reference to the NPC script

    public TextMeshProUGUI keyCollectObjectiveText; // Reference to the key collection objective text
    public Color completedObjectiveColor = Color.green; // Color to indicate completion

    public bool HasKey { get; private set; } = false; // Track if the player has the key

    private void Awake()
    {
        eKeyImage = GetComponentInChildren<Image>();
        if (eKeyImage != null)
        {
            eKeyImage.gameObject.SetActive(false); // Hide the 'E' image initially
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not present
        }

        mainCamera = Camera.main;
        npc = FindObjectOfType<NPC>(); // Ensure your NPC is in the scene
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUpKey();
        }

        // Billboard effect for the 'E' image
        if (eKeyImage != null && mainCamera != null)
        {
            eKeyImage.transform.LookAt(mainCamera.transform);
            eKeyImage.transform.Rotate(0, 180, 0); // Optional: Rotate to face the camera properly
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (eKeyImage != null)
            {
                eKeyImage.gameObject.SetActive(true); // Show the 'E' image
            }
            Debug.Log("Player is within pickup range of the key.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (eKeyImage != null)
            {
                eKeyImage.gameObject.SetActive(false); // Hide the 'E' image
            }
            Debug.Log("Player left the pickup range of the key.");
        }
    }

    private void PickUpKey()
    {
        Debug.Log("Key has been picked up!");
        PlayPickupSound();
        UpdateKeyCollectObjective(); // Call the method to update the key collection objective
        HasKey = true; // Set HasKey to true when the key is picked up
        StartCoroutine(DestroyKeyAfterDelay(0.5f)); // Wait 0.5 seconds before destroying
    }

    private IEnumerator DestroyKeyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); // Destroy the key after the delay
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Debug.Log("Playing pickup sound.");
        }
    }

    // Call UpdateKeyCollectObjective from the NPC instance
    private void UpdateKeyCollectObjective()
    {
        if (npc != null)
        {
            npc.UpdateKeyCollectObjective(); // Call the method from the NPC instance
        }
    }
}
