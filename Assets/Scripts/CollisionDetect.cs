using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// CollisionDetect: Attached to the Player or Obstacle triggers.
/// Detects collisions with airport obstacles (luggage stacks, airport luggage carts, stanchion barriers).
/// - Halts forward movement
/// - Plays collision impact audio & camera shake / stumble
/// - Fades out screen
/// - Saves run progress with SaveLoad (COINSAVE, GEMSAVE, DISTANCESAVE)
/// - Reloads current scene or triggers restart
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class CollisionDetect : MonoBehaviour
{
    [Header("Collision Effects")]
    public AudioSource audioSource;
    public AudioClip collisionSound;
    public Image fadeOutPanel;
    public GameObject gameOverUI;

    [Header("Timing")]
    public float restartDelay = 2.2f;

    [Header("Camera Shake Reference")]
    public Camera mainCamera;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;

    private bool hasCollided = false;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for obstacle collision
        if (other.CompareTag("Obstacle") || other.name.ToLower().Contains("obstacle") ||
            other.name.ToLower().Contains("luggage") || other.name.ToLower().Contains("cart") ||
            other.name.ToLower().Contains("barrier") || other.name.ToLower().Contains("stanchion"))
        {
            HandleCollision(other.gameObject);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hasCollided) return;

        if (hit.gameObject.CompareTag("Obstacle") || hit.gameObject.name.ToLower().Contains("obstacle") ||
            hit.gameObject.name.ToLower().Contains("luggage") || hit.gameObject.name.ToLower().Contains("cart") ||
            hit.gameObject.name.ToLower().Contains("barrier") || hit.gameObject.name.ToLower().Contains("stanchion"))
        {
            // Only trigger if hit from the front or side, not just standing on ground
            if (hit.normal.y < 0.6f)
            {
                HandleCollision(hit.gameObject);
            }
        }
    }

    public void HandleCollision(GameObject obstacleHit)
    {
        if (hasCollided) return;
        hasCollided = true;

        Debug.Log($"[Airport Dash] Collision with {obstacleHit.name}!");

        // 1. Stop Player Movement
        if (playerMovement != null)
        {
            playerMovement.StopMoving();
        }

        // 2. Play Collision Sound
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound, 1.0f);
        }

        // 3. Save run metrics to PlayerPrefs
        int currentCoins = MasterLevelInfo.coinCount;
        int currentGems = MasterLevelInfo.gemCount;
        int currentDistance = MasterLevelInfo.distanceRun;
        SaveLoad.SaveRunData(currentCoins, currentGems, currentDistance);

        // 4. Stumble Animation / Camera Shake / Fade out
        StartCoroutine(StumbleAndRestartSequence());
    }

    private IEnumerator StumbleAndRestartSequence()
    {
        // 1. Shake Camera
        if (mainCamera != null)
        {
            StartCoroutine(DoCameraShake());
        }

        // 2. Fall / Stumble visual rotation
        Transform visual = playerMovement != null && playerMovement.visualModel != null ? playerMovement.visualModel : transform;
        Quaternion startRot = visual.localRotation;
        Quaternion stumbleRot = Quaternion.Euler(-30f, 25f, 40f);

        float stumbleTime = 0.4f;
        float elapsed = 0f;
        while (elapsed < stumbleTime)
        {
            elapsed += Time.deltaTime;
            visual.localRotation = Quaternion.Slerp(startRot, stumbleRot, elapsed / stumbleTime);
            yield return null;
        }

        // 3. Show Game Over UI if configured
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 4. Fade to Black
        if (fadeOutPanel != null)
        {
            fadeOutPanel.gameObject.SetActive(true);
            Color color = fadeOutPanel.color;
            float fadeElapsed = 0f;
            float fadeDuration = restartDelay - stumbleTime;

            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                color.a = Mathf.Clamp01(fadeElapsed / fadeDuration);
                fadeOutPanel.color = color;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(restartDelay - stumbleTime);
        }

        // 5. Reload Scene to Restart
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private IEnumerator DoCameraShake()
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }
}

