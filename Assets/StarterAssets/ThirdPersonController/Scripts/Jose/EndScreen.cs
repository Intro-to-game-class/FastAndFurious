
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour
{
    public static EndScreenController Instance;

    [Header("UI")]
    public GameObject endScreenPanel; // Drag your end screen panel here

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false); // Hide on start
        }
        else
        {
            Debug.LogWarning("EndScreenController: 'endScreenPanel' is not assigned.");
        }
    }

    /// <summary>
    /// Show the end screen panel.
    /// </summary>
    public void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("EndScreenController: endScreenPanel is null in ShowEndScreen.");
        }
    }

    /// <summary>
    /// Reload the current scene (Play Again button).
    /// </summary>
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Quit the game (Quit button).
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit (won't close in Editor)");
    }
}
