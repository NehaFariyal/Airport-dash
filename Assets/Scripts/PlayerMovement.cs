using UnityEngine;

/// <summary>
/// PlayerMovement: 3-Lane Endless Runner Controller for Airport Dash.
/// - The player moves strictly through 3 paths/tracks:
///     * Lane 0: Left Track   (X = -3.0)
///     * Lane 1: Middle Track (X =  0.0) [Default Start]
///     * Lane 2: Right Track  (X = +3.0)
/// - Pressing A or Left Arrow shifts one track to the left.
/// - Pressing D or Right Arrow shifts one track to the right.
/// - Smoothly and crisply moves into the exact center of each track.
/// - Spacebar jumps over obstacles while locked into the lane.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Forward Speed")]
    [Tooltip("Automatic forward running speed along Z")]
    public float forwardSpeed = 12f;
    [Tooltip("Maximum speed as distance increases")]
    public float maxSpeed = 24f;
    [Tooltip("Acceleration rate")]
    public float speedIncreaseRate = 0.05f;

    [Header("3-Lane Movement (Subway Surfers Style)")]
    [Tooltip("Distance from center to side tracks (Middle = 0, Left = -laneDistance, Right = +laneDistance)")]
    public float laneDistance = 3.0f;
    [Tooltip("Speed of sliding smoothly between tracks")]
    public float laneChangeSpeed = 22f;

    [Tooltip("Current track index: 0 = Left (-3.0), 1 = Middle (0.0), 2 = Right (+3.0)")]
    [Range(0, 2)]
    public int currentLane = 1; // Starts in the middle track!

    [Header("Jumping & Gravity")]
    [Tooltip("Jump velocity")]
    public float jumpForce = 8.5f;
    [Tooltip("Downwards gravity")]
    public float gravity = 24f;

    [Header("Visual Tilt / Roll")]
    public Transform visualModel;
    public float leanAngle = 14f;
    public float leanSpeed = 12f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;

    [Header("State")]
    public bool canMove = true;
    public bool isGrounded = true;

    private CharacterController controller;
    private Vector3 verticalVelocity = Vector3.zero;
    private float initialZ;
    private float targetX = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    private void Start()
    {
        initialZ = transform.position.z;

        // Ensure player starts firmly in the middle track (X = 0)
        currentLane = 1;
        targetX = 0f;
        Vector3 startPos = transform.position;
        startPos.x = 0f;
        transform.position = startPos;
    }

    private void Update()
    {
        if (!canMove) return;

        // 1. Forward Acceleration over time
        if (forwardSpeed < maxSpeed)
        {
            forwardSpeed += speedIncreaseRate * Time.deltaTime;
        }

        // 2. Discrete 3-Lane Input (Left / Right)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }

        // Determine exact target X for the selected track
        // Lane 0: -laneDistance (-3.0)
        // Lane 1:  0.0 (Middle)
        // Lane 2: +laneDistance (+3.0)
        targetX = (currentLane - 1) * laneDistance;

        // 3. Smooth, snappy interpolation into the center of the target track
        float currentX = transform.position.x;
        float newX = Mathf.MoveTowards(currentX, targetX, laneChangeSpeed * Time.deltaTime);
        float deltaX = newX - currentX;

        // 4. Grounding & Jumping (Spacebar)
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVelocity.y = -2.0f; // Keep firmly grounded

            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity.y = jumpForce;
                if (audioSource != null && jumpSound != null)
                {
                    audioSource.PlayOneShot(jumpSound, 0.85f);
                }
            }
        }
        else
        {
            // Apply downward gravity
            verticalVelocity.y -= gravity * Time.deltaTime;
        }

        // 5. Assemble Movement Vector
        Vector3 moveVector = new Vector3(
            deltaX / Mathf.Max(Time.deltaTime, 0.0001f),
            verticalVelocity.y,
            forwardSpeed
        );

        // Apply forward and vertical movement via CharacterController
        controller.Move(moveVector * Time.deltaTime);

        // 6. Hard-lock X to the exact calculated lane position (Prevents any drift outside the 3 tracks)
        Vector3 pos = transform.position;
        pos.x = newX;
        transform.position = pos;

        // 7. Fall Protection (Safety check if player drops below track level)
        if (transform.position.y < -1.5f)
        {
            var colDetect = GetComponent<CollisionDetect>();
            if (colDetect != null)
            {
                colDetect.HandleCollision(gameObject);
            }
            else
            {
                // Reset onto the current lane at track height
                transform.position = new Vector3(targetX, 0.5f, transform.position.z);
                verticalVelocity = Vector3.zero;
            }
        }

        // 8. Distance tracking in MasterLevelInfo
        if (MasterLevelInfo.Instance != null)
        {
            MasterLevelInfo.Instance.SetRawDistance(transform.position.z - initialZ);
        }

        // 9. Visual Bank / Lean Animation during lane switch
        if (visualModel != null)
        {
            float xDifference = targetX - currentX;
            float lean = 0f;
            if (Mathf.Abs(xDifference) > 0.05f)
            {
                lean = Mathf.Sign(xDifference) * -leanAngle;
            }

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, lean);
            visualModel.localRotation = Quaternion.Slerp(visualModel.localRotation, targetRotation, Time.deltaTime * leanSpeed);
        }
    }

    /// <summary>
    /// Shifts the active lane by direction (-1 for Left, +1 for Right).
    /// Clamps strictly between 0 (Left track) and 2 (Right track).
    /// </summary>
    public void ChangeLane(int direction)
    {
        int newLane = Mathf.Clamp(currentLane + direction, 0, 2);
        if (newLane != currentLane)
        {
            currentLane = newLane;
            targetX = (currentLane - 1) * laneDistance;
        }
    }

    public void StopMoving()
    {
        canMove = false;
        if (MasterLevelInfo.Instance != null)
        {
            MasterLevelInfo.Instance.StopTracking();
        }
    }

    public void ResumeMoving()
    {
        canMove = true;
    }
}
