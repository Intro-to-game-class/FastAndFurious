using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1;
    public AudioClip collectSound; // Drag your sound clip here in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(value);

            // Play sound at collectible's position
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Destroy immediately
            Destroy(gameObject);
        }
    }
}