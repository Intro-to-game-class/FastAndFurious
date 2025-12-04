using UnityEngine;
using UnityEngine.InputSystem; // for PlayerInput

public class EnemyCollisionUI : MonoBehaviour
{
    public GameObject gameOverPanel; // assign in Inspector

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (CountdownTrigger.isPoweredUp)
            {
                // Player is powered up → destroy enemy instead of showing panel
                Destroy(gameObject);
            }
            else
            {
                // Normal behavior → show panel and disable player input
                if (gameOverPanel != null)
                    gameOverPanel.SetActive(true);

                var input = collision.gameObject.GetComponent<PlayerInput>();
                if (input != null)
                    input.enabled = false;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}