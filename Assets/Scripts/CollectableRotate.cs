using UnityEngine;

/// <summary>
/// CollectableRotate: Smoothly spins collectibles (Coins, Gems) so they are easily spotted.
/// Also provides subtle vertical floating bobbing.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class CollectableRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotateSpeed = 120f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Hover / Bobbing Settings")]
    public bool enableBobbing = true;
    public float bobbingSpeed = 2.5f;
    public float bobbingHeight = 0.15f;

    private Vector3 startLocalPosition;
    private float randomOffset;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        // Continuous rotation
        transform.Rotate(rotationAxis * (rotateSpeed * Time.deltaTime), Space.Self);

        // Hover bobbing
        if (enableBobbing)
        {
            float newY = startLocalPosition.y + Mathf.Sin(Time.time * bobbingSpeed + randomOffset) * bobbingHeight;
            Vector3 pos = transform.localPosition;
            pos.y = newY;
            transform.localPosition = pos;
        }
    }
}

