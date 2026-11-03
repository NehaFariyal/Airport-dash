using System.Collections;
using UnityEngine;

/// <summary>
/// CollectCoin: Handles coin pickup in Airport Dash.
/// - Adds +1 coin to MasterLevelInfo
/// - Plays pickup sound
/// - Shrinks, rises, and disappears smoothly
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class CollectCoin : MonoBehaviour
{
    [Header("Coin Properties")]
    public int coinValue = 1;
    public AudioClip pickupSound;
    public ParticleSystem collectionVfx;

    private bool isCollected = false;
    private Collider itemCollider;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            isCollected = true;
            if (itemCollider != null) itemCollider.enabled = false;

            // Add coin to level info
            if (MasterLevelInfo.Instance != null)
            {
                MasterLevelInfo.Instance.AddCoin(coinValue);
            }

            // Spawn optional particles
            if (collectionVfx != null)
            {
                Instantiate(collectionVfx, transform.position, Quaternion.identity);
            }

            // Animate shrink and rise, then destroy
            StartCoroutine(AnimatePickupAndDestroy());
        }
    }

    private IEnumerator AnimatePickupAndDestroy()
    {
        // Stop spinning script if present during pickup animation
        var rotator = GetComponent<CollectableRotate>();
        if (rotator != null) rotator.enabled = false;

        float duration = 0.25f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Rises upward
            transform.position = startPosition + Vector3.up * (t * 0.8f);
            // Shrinks down to zero
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}

