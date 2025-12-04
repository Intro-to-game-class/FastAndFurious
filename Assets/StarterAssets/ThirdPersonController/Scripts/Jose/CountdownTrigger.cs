using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTrigger : MonoBehaviour
{
    public TextMeshProUGUI countdownText;   // assign in Inspector
    public float countdownTime = 10f;       // powerup lasts 10 seconds

    private bool isCountingDown = false;
    public static bool isPoweredUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCountingDown)
        {
            // Start countdown FIRST so the coroutine is not interrupted
            StartCoroutine(StartCountdown());

            // Hide sphere visual + collider AFTER coroutine begins
            if (TryGetComponent<MeshRenderer>(out var mr))
                mr.enabled = false;

            if (TryGetComponent<Collider>(out var col))
                col.enabled = false;
        }
    }

    private IEnumerator StartCountdown()
    {
        isCountingDown = true;
        isPoweredUp = true;

        float timeLeft = countdownTime;
        countdownText.gameObject.SetActive(true);

        // Loop until timer reaches zero
        while (timeLeft > 0)
        {
            countdownText.text = "Powerup: " + timeLeft.ToString("0");
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        // Hide text and reset state
        countdownText.gameObject.SetActive(false);
        isPoweredUp = false;
        isCountingDown = false;
    }
}
