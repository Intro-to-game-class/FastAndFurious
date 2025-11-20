using UnityEngine;
using TMPro;

public class ShowTextOnStart : MonoBehaviour
{
    public float displayTime = 3f; // Time in seconds
    private TMP_Text tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        tmpText.enabled = true; // Show text
        Invoke("HideText", displayTime); // Hide after delay
    }

    private void HideText()
    {
        tmpText.enabled = false; // Hide text
    }
}
