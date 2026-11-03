using UnityEngine;
using UnityEngine.UI;
#if TMPro
using TMPro;
#endif

/// <summary>
/// MasterLevelInfo: Manages current run metrics (coins, gems, distance) and updates HUD in real-time.
/// Designed for Airport Dash theme.
/// Author: Sk. Neha Fariyal - 2303A51598
/// </summary>
public class MasterLevelInfo : MonoBehaviour
{
    public static MasterLevelInfo Instance { get; private set; }

    [Header("Run Statistics")]
    public static int coinCount = 0;
    public static int gemCount = 0;
    public static int distanceRun = 0;

    [Header("UI Text References (Standard UI)")]
    public Text coinText;
    public Text gemText;
    public Text distanceText;

    [Header("UI Text References (TextMeshPro - Optional)")]
    public Component coinTmpText;
    public Component gemTmpText;
    public Component distanceTmpText;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip gemSound;

    private float rawDistance = 0f;
    private bool isTrackingDistance = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Reset counters for the new run
        coinCount = 0;
        gemCount = 0;
        distanceRun = 0;
        rawDistance = 0f;
        isTrackingDistance = true;

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
        UpdateHUD();
    }

    private void Update()
    {
        if (isTrackingDistance)
        {
            // Update distance based on forward progress
            distanceRun = Mathf.FloorToInt(rawDistance);
            UpdateHUD();
        }
    }

    public void AddDistance(float amount)
    {
        if (!isTrackingDistance) return;
        rawDistance += amount;
        distanceRun = Mathf.FloorToInt(rawDistance);
        UpdateHUD();
    }

    public void SetRawDistance(float currentZ)
    {
        if (!isTrackingDistance) return;
        if (currentZ > rawDistance)
        {
            rawDistance = currentZ;
            distanceRun = Mathf.FloorToInt(rawDistance);
            UpdateHUD();
        }
    }

    public void AddCoin(int amount = 1)
    {
        coinCount += amount;
        if (audioSource != null && coinSound != null)
        {
            audioSource.PlayOneShot(coinSound, 0.9f);
        }
        UpdateHUD();
    }

    public void AddGem(int amount = 1)
    {
        gemCount += amount;
        if (audioSource != null && gemSound != null)
        {
            audioSource.PlayOneShot(gemSound, 1.0f);
        }
        UpdateHUD();
    }

    public void StopTracking()
    {
        isTrackingDistance = false;
    }

    public void UpdateHUD()
    {
        // Update standard UI Text if assigned
        if (coinText != null) coinText.text = coinCount.ToString();
        if (gemText != null) gemText.text = gemCount.ToString();
        if (distanceText != null) distanceText.text = distanceRun.ToString() + "m";

        // Update TextMeshPro reflection-safe if assigned
        SetTmpText(coinTmpText, coinCount.ToString());
        SetTmpText(gemTmpText, gemCount.ToString());
        SetTmpText(distanceTmpText, distanceRun.ToString() + "m");
    }

    private void SetTmpText(Component target, string value)
    {
        if (target == null) return;
        var prop = target.GetType().GetProperty("text");
        if (prop != null)
        {
            prop.SetValue(target, value, null);
        }
    }
}

