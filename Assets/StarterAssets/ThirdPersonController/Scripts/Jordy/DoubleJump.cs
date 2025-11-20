using UnityEngine;
using StarterAssets;
using System.Reflection;
public class DoubleJump_Jordy : MonoBehaviour
{
    public int maxJumpCount = 2;
    public float jumpForce = 10f;
    private int _jumpCount = 0;
    private ThirdPersonController _controller;
    private FieldInfo _verticalVelocityField;
    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _verticalVelocityField = typeof(ThirdPersonController).GetField("_verticalVelocity", BindingFlags.NonPublic | BindingFlags.Instance);
    }
    void Update()
    {
        if (_controller.Grounded)
        {
            _jumpCount = 0;
        }
        if (Input.GetButtonDown("Jump") && _jumpCount < maxJumpCount)
        {
            float jumpVelocity = Mathf.Sqrt(jumpForce * -2f * _controller.Gravity);
            _verticalVelocityField.SetValue(_controller, jumpVelocity);
            _jumpCount++;
        }
    }
}


