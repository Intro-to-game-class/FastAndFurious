using UnityEngine;
using StarterAssets;
using System.Reflection;

public class DoubleJump_Christopher : MonoBehaviour
{
    // The sound clip to play specifically for the double jump (the 2nd jump)
    public AudioClip doubleJumpSound;

    // Set this to 1 in the Inspector if you want exactly one extra jump (the double jump).
    public int maxJumpCount = 1;
    public float jumpForce = 10f;

    // We start the counter at 0, representing 0 extra jumps used.
    private int _jumpCount = 0;
    private ThirdPersonController _controller;
    private FieldInfo _verticalVelocityField;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        // Using Reflection to access the private _verticalVelocity field
        _verticalVelocityField = typeof(ThirdPersonController).GetField("_verticalVelocity", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void Update()
    {
        // Reset jump count when grounded. This lets the player use their extra jumps again.
        if (_controller.Grounded)
        {
            _jumpCount = 0;
        }

        // Check for jump input and if the player is currently airborne AND has extra jumps remaining.
        // The check for !Grounded ensures this script ONLY handles the double jump and ignores the first grounded jump.
        if (Input.GetButtonDown("Jump") && !_controller.Grounded && _jumpCount < maxJumpCount)
        {
            // --- DEBUG LOG: Check if the jump input is being detected and processed ---
            Debug.Log($"Double Jump detected! Current Extra Jumps Used: {_jumpCount}.");

            // CRITICAL CHECK: We play the sound effect before applying the jump and incrementing the counter.
            if (doubleJumpSound != null)
            {
                // Play the sound effect at the character's position, ensuring full volume (1.0f).
                AudioSource.PlayClipAtPoint(doubleJumpSound, transform.position, 1.0f);

                // --- DEBUG LOG: Confirm sound logic was reached ---
                Debug.Log("Playing Double Jump Sound!");
            }

            // Apply the upward velocity (the jump logic)
            float jumpVelocity = Mathf.Sqrt(jumpForce * -2f * _controller.Gravity);
            _verticalVelocityField.SetValue(_controller, jumpVelocity);

            // Increment the jump counter, showing one extra jump has been used.
            _jumpCount++;
        }
    }
}