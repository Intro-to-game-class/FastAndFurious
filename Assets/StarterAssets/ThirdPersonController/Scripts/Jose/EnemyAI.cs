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
    public Transform visual;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private float directionTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Start()
    {
        PickRandomDirection();
        directionTimer = directionChangeInterval;
    }

    void FixedUpdate()
    {
        directionTimer -= Time.fixedDeltaTime;
        if (directionTimer <= 0f)
        {
            PickRandomDirection();
            directionTimer = directionChangeInterval;
        }

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        FaceMovementDirection(moveDirection);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            PickNewDirectionOnCollision();
    }

    // ----------------- Behavior -----------------

    private void PickRandomDirection()
    {
        moveDirection = GetRandomCardinalDirection();
    }

    private void PickNewDirectionOnCollision()
    {
        Vector3 oldDir = moveDirection;
        Vector3 newDir = oldDir;

        // Ensure new direction is different
        while (newDir == oldDir)
            newDir = GetRandomCardinalDirection();

        moveDirection = newDir;
    }

    private Vector3 GetRandomCardinalDirection()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: return Vector3.forward;
            case 1: return Vector3.back;
            case 2: return Vector3.left;
            case 3: return Vector3.right;
        }
        return Vector3.forward;
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

        if (modelForwardOffset != Vector3.zero)
            targetRot *= Quaternion.Euler(modelForwardOffset);

        Transform t = (visual != null) ? visual : transform;
        t.rotation = Quaternion.Slerp(t.rotation, targetRot, turnSpeed * Time.deltaTime);
    }
}