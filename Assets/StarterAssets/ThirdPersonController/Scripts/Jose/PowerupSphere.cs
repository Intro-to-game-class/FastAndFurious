using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerupManager pm = other.GetComponent<PowerupManager>();
            if (pm != null)
            {
                pm.ActivatePowerup();
            }
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;

        }
    }
}