using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// LoadToStage: Controls the Information / Loading scene.
/// Displays key bindings (A/D/Arrows for Left/Right, Spacebar for Jump),
/// theme hazards (Luggage, Carts, Stanchions), and smoothly loads the GamePlay scene.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class LoadToStage : MonoBehaviour
{
    [Header("Loading Settings")]
    public string targetSceneName = "GamePlay";
    public float displayDuration = 3.5f;

    [Header("UI Elements")]
    public Slider progressBar;
    public Text loadingText;
    public Image fadeImage;
    public Button skipButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private void Awake()
    {
        if (skipButton == null)
        {
            var btnObj = GameObject.Find("SkipButton");
            if (btnObj != null) skipButton = btnObj.GetComponent<Button>();
        }

        if (skipButton != null)
        {
            skipButton.onClick.RemoveListener(OnSkipClicked);
            skipButton.onClick.AddListener(OnSkipClicked);
        }
    }

    private void Start()
    {
        StartCoroutine(LoadingRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            OnSkipClicked();
        }
    }

    private IEnumerator LoadingRoutine()
    {
        float elapsed = 0f;

        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / displayDuration);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (loadingText != null)
            {
                int dots = (int)(elapsed * 3f) % 4;
                loadingText.text = "PREPARING BOARDING GATE" + new string('.', dots);
            }

            yield return null;
        }

        // Fade out
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float fadeElapsed = 0f;
            float fadeDuration = 0.5f;
            Color c = fadeImage.color;

            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(fadeElapsed / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }
        }

        SceneManager.LoadScene(targetSceneName);
    }

    public void OnSkipClicked()
    {
        Debug.Log("[LoadToStage] Skip / Start Dash Now clicked! Launching GamePlay...");
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        StopAllCoroutines();
        SceneManager.LoadScene(targetSceneName);
    }
}
