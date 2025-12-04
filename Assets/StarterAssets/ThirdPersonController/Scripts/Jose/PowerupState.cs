using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    public float powerupDuration = 10f;
    public bool isPoweredUp = false; // Public so other scripts can check

    public delegate void PowerupEvent(float timeRemaining);
    public static event PowerupEvent OnPowerupTick;

    public delegate void PowerupState(bool active);
    public static event PowerupState OnPowerupStateChanged;

    public void ActivatePowerup()
    {
        if (!isPoweredUp)
            StartCoroutine(PowerupRoutine());
    }

    private IEnumerator PowerupRoutine()
    {
        isPoweredUp = true;
        OnPowerupStateChanged?.Invoke(true);

        float timer = powerupDuration;
        while (timer > 0f)
        {
            OnPowerupTick?.Invoke(timer);
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        isPoweredUp = false;
        OnPowerupStateChanged?.Invoke(false);
    }
}