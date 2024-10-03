using UnityEngine;

public class TestSound : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip testSound; // Assign a sound clip in the Inspector

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Press Space to play sound
        {
            PlayTestSound();
        }
    }

    private void PlayTestSound()
    {
        if (audioSource != null && testSound != null)
        {
            audioSource.PlayOneShot(testSound);
        }
    }
}
