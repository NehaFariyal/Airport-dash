using UnityEngine;

/// <summary>
/// CameraFollow: Smoothly tracks the player down the airport concourse track.
/// Maintains third-person runner perspective.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0f, 3.8f, -6.5f);
    public float lookAtHeightOffset = 1.2f;

    [Header("Smoothness")]
    public float smoothSpeed = 10f;
    public bool lockX = false; // When true, camera stays centered on the track

    private void Start()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        if (lockX)
        {
            targetPos.x = offset.x; // Keep camera centered on track
        }
        else
        {
            // Gentle dampening on X to reduce motion sickness
            targetPos.x = Mathf.Lerp(transform.position.x, target.position.x * 0.4f, Time.deltaTime * 5f);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // Look slightly above the player's root
        Vector3 lookPoint = target.position + Vector3.up * lookAtHeightOffset;
        lookPoint.x = transform.position.x * 0.5f;
        transform.LookAt(lookPoint);
    }
}

