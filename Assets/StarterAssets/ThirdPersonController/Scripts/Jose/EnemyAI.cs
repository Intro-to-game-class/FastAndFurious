using UnityEngine;

public class PacmanEnemyRigidbody : MonoBehaviour
{
    public float moveSpeed = 3f;                // Movement speed
    public float directionChangeInterval = 2f;  // Time between random direction changes

    private Rigidbody rb;
    private Vector3 moveDirection;
    private float directionTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tipping over
        PickRandomDirection();
        directionTimer = directionChangeInterval;
    }

    void FixedUpdate()
    {
        // Countdown until next direction change
        directionTimer -= Time.fixedDeltaTime;
        if (directionTimer <= 0f)
        {
            PickRandomDirection();
            directionTimer = directionChangeInterval;
        }

        // Move the cube using physics
        Vector3 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    void PickRandomDirection()
    {
        // Choose a random cardinal direction (X/Z plane)
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: moveDirection = Vector3.forward; break;
            case 1: moveDirection = Vector3.back; break;
            case 2: moveDirection = Vector3.left; break;
            case 3: moveDirection = Vector3.right; break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the cube hits a wall, pick a new direction
        if (collision.gameObject.CompareTag("Wall"))
        {
            PickRandomDirection();
        }
    }
}