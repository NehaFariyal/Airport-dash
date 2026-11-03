using UnityEngine;

/// <summary>
/// AirplaneFlyover: Smoothly flies aeroplanes across the airport sky
/// or animates taxiing aeroplanes on the runway apron.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class AirplaneFlyover : MonoBehaviour
{
    [Header("Flight Parameters")]
    public float flightSpeed = 35f;
    public Vector3 flightDirection = Vector3.forward;
    public float climbSpeed = 2f;
    public float bankAngle = 10f;

    [Header("Despawn")]
    public float maxDistance = 500f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Move plane forward & climb slightly
        Vector3 movement = (flightDirection.normalized * flightSpeed) + (Vector3.up * climbSpeed);
        transform.position += movement * Time.deltaTime;

        // Reset if it flies too far
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            transform.position = startPosition;
        }
    }
}

