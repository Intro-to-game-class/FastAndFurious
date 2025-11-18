using UnityEngine;
using StarterAssets;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepClips; // Assign in Inspector
    public float stepInterval = 0.5f; // Time between steps
    public float walkSpeedThreshold = 0.1f;

    private AudioSource audioSource;
    private ThirdPersonController controller;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<ThirdPersonController>();
    }

    void Update()
    {
        // Check if player is moving and grounded
        if (controller.Grounded && controller.MoveSpeed > walkSpeedThreshold)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer when not moving
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            audioSource.PlayOneShot(footstepClips[index]);
        }
    }
}