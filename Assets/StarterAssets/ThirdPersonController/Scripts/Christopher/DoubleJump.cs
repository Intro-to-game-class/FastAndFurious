
using UnityEngine;
using StarterAssets;
using System.Reflection;

public class DoubleJump_Christopher : MonoBehaviour
{
    // --- EXISTING ---
    public AudioClip doubleJumpSound;
    public int maxJumpCount = 1;
    public float jumpForce = 10f;

    // --- NEW: Plug your jetpack particle system here in the Inspector ---
    [Header("Jetpack Visuals (Double Jump)")]
    public ParticleSystem jetpackDoubleJumpFX;

    private int _jumpCount = 0;
    private ThirdPersonController _controller;
    private FieldInfo _verticalVelocityField;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _verticalVelocityField = typeof(ThirdPersonController)
            .GetField("_verticalVelocity", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void Update()
    {
        // Reset extra jump count when grounded
        if (_controller.Grounded)
        {
            _jumpCount = 0;
        }

        // Only handle airborne extra jump(s)
        if (Input.GetButtonDown("Jump") && !_controller.Grounded && _jumpCount < maxJumpCount)
        {
            // --- SOUND (existing) ---
            if (doubleJumpSound != null)
            {
                AudioSource.PlayClipAtPoint(doubleJumpSound, transform.position, 1.0f);
            }

            // --- VISUAL FX (new) ---
            if (jetpackDoubleJumpFX != null)
            {
                // Optional: clear to avoid residual particles, then play burst
                jetpackDoubleJumpFX.Clear(true);
                jetpackDoubleJumpFX.Play(true);
            }

            // --- APPLY JUMP VELOCITY (existing logic) ---
            float jumpVelocity = Mathf.Sqrt(jumpForce * -2f * _controller.Gravity); // Gravity comes from ThirdPersonController
            _verticalVelocityField.SetValue(_controller, jumpVelocity);

            _jumpCount++;
        }
    }
}
