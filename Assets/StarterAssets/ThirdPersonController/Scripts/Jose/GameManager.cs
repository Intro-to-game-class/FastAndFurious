
using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TMP_Text scoreText;
    public GameObject winMessage;
    public EndScreenController endScreen;

    [Header("Game Settings")]
    public int scoreThreshold = 10;
    public float endScreenDelay = 5f;

    private int score = 0;
    private bool endShown = false;

    [Header("Player")]
    public GameObject player; // Drag your player GameObject here
    private MonoBehaviour playerController; // Replace with your movement script type if known

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (player != null)
        {
            // If using Starter Assets, replace MonoBehaviour with StarterAssets.ThirdPersonController
            playerController = player.GetComponent<MonoBehaviour>();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (scoreText != null) scoreText.text = "Score: " + score;

        if (!endShown && score >= scoreThreshold)
        {
            endShown = true;

            if (winMessage != null)
            {
                winMessage.SetActive(true);
                StartCoroutine(HideWinMessageAfterDelay(5f));
            }

            StartCoroutine(ShowEndScreenAfterDelay(endScreenDelay));
        }
    }

    private IEnumerator ShowEndScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Disable player controls
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Unlock and show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show end screen
        if (endScreen != null)
        {
            endScreen.ShowEndScreen();
        }
        else
        {
            Debug.LogWarning("GameManager: EndScreenController reference is missing.");
        }
    }

    private IEnumerator HideWinMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (winMessage != null) winMessage.SetActive(false);
    }
}
