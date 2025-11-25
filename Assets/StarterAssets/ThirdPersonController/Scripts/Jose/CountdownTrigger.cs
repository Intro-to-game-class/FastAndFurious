using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTrigger : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // assign in Inspector
    public float countdownTime = 5f;

    private bool isCountingDown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCountingDown)
        {
            // Hide the sphere visuals + collider, but keep script alive
            GetComponent<Collider>().enabled = false;
            if (GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().enabled = false;

            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        isCountingDown = true;
        float timeLeft = countdownTime;

        // Show the text when countdown starts
        countdownText.gameObject.SetActive(true);

        while (timeLeft > 0)
        {
            countdownText.text = "Powerup: " + timeLeft.ToString("F0");
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        // Hide the text when countdown ends
        countdownText.gameObject.SetActive(false);

        isCountingDown = false;
    }
}