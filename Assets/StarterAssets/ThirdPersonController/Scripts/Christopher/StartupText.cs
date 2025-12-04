

using UnityEngine;

using TMPro;

public class StartupTextTMP : MonoBehaviour

{

    [Header("Message")]

    [SerializeField] private TMP_Text messageText;          // Drag your TMP_Text here

    [SerializeField] private string message = "Welcome!";

    [SerializeField] private float showDurationSeconds = 5f;

    [Header("Fade")]

    [SerializeField] private bool fadeOut = true;

    [SerializeField] private float fadeOutSeconds = 0.75f;

    private float _startAlpha = 1f;

    private void Awake()

    {

        if (messageText == null)

        {

            Debug.LogWarning("StartupTextTMP: Assign a TMP_Text in the Inspector.", this);

            enabled = false;

            return;

        }

        // Initialize text

        messageText.text = message;

        _startAlpha = messageText.color.a;

        messageText.gameObject.SetActive(true);

    }

    private void Start()

    {

        StartCoroutine(HideRoutine());

    }

    private System.Collections.IEnumerator HideRoutine()

    {

        // Show for N seconds

        yield return new WaitForSeconds(showDurationSeconds);

        if (!fadeOut)

        {

            messageText.gameObject.SetActive(false);

            yield break;

        }

        // Fade out

        float t = 0f;

        Color baseColor = messageText.color;

        while (t < fadeOutSeconds)

        {

            t += Time.deltaTime;

            float a = Mathf.Lerp(_startAlpha, 0f, t / fadeOutSeconds);

            messageText.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);

            yield return null;

        }

        messageText.gameObject.SetActive(false);

        // Restore alpha if you re-enable later

        messageText.color = new Color(baseColor.r, baseColor.g, baseColor.b, _startAlpha);

    }

}

