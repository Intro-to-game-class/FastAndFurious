using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class Portal : MonoBehaviour
{
    [Header("Pairing")]
    [Tooltip("Assign the other portal in the pair.")]
    public Portal otherPortal;

    [Header("Exit Placement")]
    [Tooltip("Optional child transform on the other portal to control exit positioning. If null, uses otherPortal.transform.")]
    public Transform exitPointOnOther;
    [Tooltip("Push the player slightly out of the exit so they don't sit inside the trigger.")]
    [Min(0f)]
    public float exitOffset = 0.6f;

    [Header("Behavior")]
    [Tooltip("Rotate player to match the exit portal's forward direction.")]
    public bool matchExitForward = true;
    [Tooltip("Preserve velocity across portals for Rigidbody players.")]
    public bool preserveVelocity = true;
    [Tooltip("Cooldown to avoid instant re-entry loops.")]
    [Min(0f)]
    public float cooldown = 0.3f;
    [Tooltip("Only teleport when entering from the front side of this portal.")]
    public bool requireFrontEntry = true;
    [Range(0f, 1f)]
    public float frontDotThreshold = 0.1f;

    [Header("Filtering")]
    [Tooltip("Only objects on these layers can trigger teleport.")]
    public LayerMask playerLayers = ~0; // default: Everything
    [Tooltip("Optional tag check (leave empty to skip).")]
    public string playerTag = "Player";

    [Header("Audio")]
    [Tooltip("AudioSource on this portal (3D). If null, one will be added at runtime.")]
    public AudioSource audioSource;
    [Tooltip("Clip to play when entering this portal.")]
    public AudioClip enterClip;
    [Tooltip("Clip to play at the exit portal when the player appears.")]
    public AudioClip exitClip;
    [Range(0f, 1f)] public float enterVolume = 1f;
    [Range(0f, 1f)] public float exitVolume = 1f;

    private void Awake()
    {
        // Ensure we have a trigger collider
        var col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning($"[{name}] No Collider found. Adding BoxCollider and setting IsTrigger.");
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = true;

        // Ensure an AudioSource exists (3D)
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;  // 3D sound
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTeleportCandidate(other)) return;
        if (otherPortal == null)
        {
            Debug.LogWarning($"Portal '{name}': otherPortal is not assigned.");
            return;
        }

        // Prevent immediate re-trigger loops via a per-player cooldown flag
        var flag = other.GetComponent<TeleportCooldownFlag>();
        if (flag != null && flag.IsOnCooldown) return;

        // Only teleport if approaching from the front side (optional)
        if (requireFrontEntry)
        {
            Vector3 toCollider = (other.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, toCollider);
            if (dot < frontDotThreshold) return;
        }

        // Play enter sound at this portal
        if (enterClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(enterClip, enterVolume);
        }

        Teleport(other);
    }

    private bool IsTeleportCandidate(Collider other)
    {
        // Layer filter
        if ((playerLayers.value & (1 << other.gameObject.layer)) == 0) return false;

        // Optional tag filter
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag)) return false;

        return true;
    }

    private void Teleport(Collider playerCol)
    {
        Transform player = playerCol.transform;
        Transform exitRef = exitPointOnOther != null ? exitPointOnOther : otherPortal.transform;

        // Target position & rotation at exit
        Vector3 targetPos = exitRef.position + exitRef.forward * exitOffset;
        Quaternion targetRot = matchExitForward
            ? Quaternion.LookRotation(exitRef.forward, Vector3.up)
            : player.rotation;

        // Gather components
        var rb = player.GetComponent<Rigidbody>();
        var cc = player.GetComponent<CharacterController>();

        // Save incoming velocity for Rigidbody
        Vector3 incomingVelocity = Vector3.zero;
        if (rb != null) incomingVelocity = rb.linearVelocity; // <-- correct property

        // Disable CharacterController to avoid collision resolution fighting the warp
        bool ccWasEnabled = false;
        if (cc != null)
        {
            ccWasEnabled = cc.enabled;
            cc.enabled = false;
        }

        // Move + rotate
        player.SetPositionAndRotation(targetPos, targetRot);

        // Re-enable CharacterController
        if (cc != null) cc.enabled = ccWasEnabled;

        // Remap velocity relative to portal orientation (keeps run direction consistent)
        if (preserveVelocity && rb != null)
        {
            Vector3 localIncoming = transform.InverseTransformDirection(incomingVelocity);
            Vector3 remapped = otherPortal.transform.TransformDirection(localIncoming);
            rb.linearVelocity = remapped; // <-- correct property
        }

        // Play exit sound at the other portal (spatialized)
        if (otherPortal != null && otherPortal.audioSource != null && otherPortal.exitClip != null)
        {
            otherPortal.audioSource.PlayOneShot(otherPortal.exitClip, otherPortal.exitVolume);
        }

        // Start cooldown on player to prevent immediate re-entry loop
        var flag = player.GetComponent<TeleportCooldownFlag>();
        if (flag == null) flag = player.gameObject.AddComponent<TeleportCooldownFlag>();
        flag.StartCooldown(cooldown);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 1.0f);

        if (otherPortal != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, otherPortal.transform.position);

            Transform exitRef = exitPointOnOther != null ? exitPointOnOther : otherPortal.transform;
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Vector3 previewPos = exitRef.position + exitRef.forward * exitOffset;
            Gizmos.DrawSphere(previewPos, 0.15f);
            Gizmos.DrawRay(exitRef.position, exitRef.forward * 0.8f);
        }
    }
#endif
}

/* ---------------- Helper component in the same file ---------------- */

public class TeleportCooldownFlag : MonoBehaviour
{
    public bool IsOnCooldown { get; private set; }

    public void StartCooldown(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        IsOnCooldown = true;
        yield return new WaitForSeconds(duration);
        IsOnCooldown = false;
    }
}
