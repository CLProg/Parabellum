using UnityEngine;
using TMPro; // Use this if you're using TextMeshPro

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float duration = 1f;

    private void Start()
    {
        Destroy(gameObject, duration); // Destroy after the duration
    }

    private void Update()
    {
        // Move the text upwards
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    public void SetText(string damageText)
    {
        // If using TextMeshPro, use this line
        GetComponent<TextMeshProUGUI>().text = damageText;
        // If using Unity's UI Text, use this line
        // GetComponent<Text>().text = damageText;
    }
}
