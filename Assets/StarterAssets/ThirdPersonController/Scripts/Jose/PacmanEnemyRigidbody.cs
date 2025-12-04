
using UnityEngine;

public class PacmanEnemyRigidbody : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float directionChangeInterval = 2f;

    [Header("Turning")]
    public float turnSpeed = 8f;
    public bool keepLevel = true;
    public Vector3 modelForwardOffset = Vector3.zero;
    public Transform visual;

    // --- VFX on death ---
    [Header("Death VFX")]
    [Tooltip("Drag your particle effect prefab here (from the Asset Store package).")]
    [SerializeField] private GameObject deathEffectPrefab;
    [Tooltip("Parent the effect to the world (null) or to the enemy's parent.")]
    [SerializeField] private bool parentToEnemyParent = false;
    [Tooltip("Prevent multiple spawns when pooling disables/enables the enemy.")]
    [SerializeField] private bool onlyWhenDestroyed = true;

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

        Vector3 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        FaceMovementDirection(moveDirection);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            PickRandomDirection();
        }
    }

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

        if (modelForwardOffset != Vector3.zero)
            targetRot *= Quaternion.Euler(modelForwardOffset);

        Transform t = (visual != null) ? visual : transform;
        t.rotation = Quaternion.Slerp(t.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    // --- call this when you kill the enemy (or let OnDestroy do it) ---
    public void Die()
    {
        // Optional: any score, drops, etc. here
        SpawnDeathEffect();
        Destroy(gameObject); // this will also trigger OnDestroy
    }

    // --- auto-spawn when Destroy(gameObject) is called ---
    private void OnDestroy()
    {
        // If object is being truly destroyed, spawn the effect.
        // When using pooling, OnDisable might fire instead of OnDestroy.
        SpawnDeathEffect();
    }

    // --- optional for object pooling (disable instead of destroy) ---
    private void OnDisable()
    {
        if (!onlyWhenDestroyed)
        {
            // Spawn even when disabled (e.g., pooled despawn)
            SpawnDeathEffect();
        }
    }

    // --- core effect spawn helper ---
    private void SpawnDeathEffect()
    {
        if (deathEffectPrefab == null) return;

        // Spawn at enemy position with current rotation (or upright if you prefer Quaternion.identity)
        Transform parent = null;
        if (parentToEnemyParent && transform.parent != null)
            parent = transform.parent;

        GameObject fx = Instantiate(deathEffectPrefab, transform.position, transform.rotation, parent);

        // Handle both cases: effect root is ParticleSystem, or contains multiple systems
        // Auto-destroy the spawned effect after its longest lifetime
        float autoDestroyDelay = 2f; // fallback
        var psRoot = fx.GetComponent<ParticleSystem>();
        if (psRoot != null)
        {
            var main = psRoot.main;
            float duration = main.duration;
            float maxStartLifetime =
                main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants ? main.startLifetime.constantMax :
                (main.startLifetime.mode == ParticleSystemCurveMode.Constant ? main.startLifetime.constant : duration); // approximate for curves

            autoDestroyDelay = duration + maxStartLifetime;
            psRoot.Play();
        }
        else
        {
            // If multiple systems, compute the longest duration among them
            var all = fx.GetComponentsInChildren<ParticleSystem>();
            float longest = 0f;
            foreach (var ps in all)
            {
                var m = ps.main;
                float dur = m.duration;
                float life =
                    m.startLifetime.mode == ParticleSystemCurveMode.TwoConstants ? m.startLifetime.constantMax :
                    (m.startLifetime.mode == ParticleSystemCurveMode.Constant ? m.startLifetime.constant : dur);

                longest = Mathf.Max(longest, dur + life);
                ps.Play();
            }
            if (longest > 0f) autoDestroyDelay = longest;
        }

        Destroy(fx, autoDestroyDelay);
    }
}
