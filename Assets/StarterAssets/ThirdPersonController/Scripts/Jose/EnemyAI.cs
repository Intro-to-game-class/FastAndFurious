
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Units per second.")]
    public float moveSpeed = 3f;

    [Tooltip("Seconds between random cardinal direction changes.")]
    public float directionChangeInterval = 2f;

    [Header("Turning")]
    [Tooltip("Higher = snappier rotation.")]
    public float turnSpeed = 8f;

    [Tooltip("Yaw-only (keeps the enemy level on X/Z plane).")]
    public bool keepLevel = true;

    [Tooltip("Use (0, 180, 0) if your mesh faces the wrong way; (0, 90, 0) if your mesh faces +X.")]
    public Vector3 modelForwardOffset = Vector3.zero;

    [Tooltip("Optional: rotate a visual child instead of the root (recommended if root holds colliders).")]
    public Transform visual; // assign your mesh child if you have one

    private Rigidbody rb;
    private Vector3 moveDirection; // current cardinal direction
    private float directionTimer;

    // ----------------- Lifecycle -----------------

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Prevent tipping (freeze X/Z rotation) but allow yaw (Y) so we can face movement
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        PickRandomDirection();
        directionTimer = directionChangeInterval;
    }

    void FixedUpdate()
    {
        // Change direction on interval
        directionTimer -= Time.fixedDeltaTime;
        if (directionTimer <= 0f)
        {
            PickRandomDirection();
            directionTimer = directionChangeInterval;
        }

        // Move using physics
        Vector3 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        // Face the movement direction (smooth yaw)
        FaceMovementDirection(moveDirection);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Hit a wall? Pick a new direction
        if (collision.gameObject.CompareTag("Wall"))
            PickRandomDirection();
    }

    // ----------------- Behavior -----------------

    private void PickRandomDirection()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: moveDirection = Vector3.forward; break;
            case 1: moveDirection = Vector3.back; break;
            case 2: moveDirection = Vector3.left; break;
            case 3: moveDirection = Vector3.right; break;
        }
    }

    private void FaceMovementDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return;

        if (keepLevel)
        {
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;
            dir.Normalize();
        }

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        // Optional forward-axis correction (e.g., if mesh faces +X, use (0, 90, 0); if backwards, (0, 180, 0))
        if (modelForwardOffset != Vector3.zero)
            targetRot *= Quaternion.Euler(modelForwardOffset);

        // Rotate the visual child if assigned, else the root
        Transform t = (visual != null) ? visual : transform;
        t.rotation = Quaternion.Slerp(t.rotation, targetRot, turnSpeed * Time.deltaTime);
    }
}
