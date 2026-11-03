using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// MainMenuControl: Manages the main menu for Airport Dash.
/// Displays saved coins, gems, and distance from PlayerPrefs.
/// Navigates to Stage Selection or Gameplay.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class MainMenuControl : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button startButton;
    public Button quickPlayButton;
    public Button resetButton;

    [Header("Saved Stats UI Displays")]
    public Text totalCoinsText;
    public Text totalGemsText;
    public Text bestDistanceText;

    [Header("Alternative TMP Component References")]
    public Component totalCoinsTmp;
    public Component totalGemsTmp;
    public Component bestDistanceTmp;

    [Header("Navigation")]
    public string stageSelectSceneName = "StageSelect";
    public string gamePlaySceneName = "GamePlay";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Auto-discover buttons if not explicitly assigned in inspector
        AutoBindButtons();
    }

    private void Start()
    {
        AutoBindButtons();
        LoadAndDisplaySavedStats();
    }

    private void Update()
    {
        // Keyboard shortcuts so player can also start instantly with Enter or Space
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            OnStartGameClicked();
        }
    }

    public void AutoBindButtons()
    {
        if (startButton == null)
        {
            var btnObj = GameObject.Find("StartButton");
            if (btnObj != null) startButton = btnObj.GetComponent<Button>();
        }
        if (quickPlayButton == null)
        {
            var btnObj = GameObject.Find("QuickPlayButton");
            if (btnObj != null) quickPlayButton = btnObj.GetComponent<Button>();
        }
        if (resetButton == null)
        {
            var btnObj = GameObject.Find("ResetButton");
            if (btnObj != null) resetButton = btnObj.GetComponent<Button>();
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartGameClicked);
            startButton.onClick.AddListener(OnStartGameClicked);
        }
        if (quickPlayButton != null)
        {
            quickPlayButton.onClick.RemoveListener(OnDirectPlayClicked);
            quickPlayButton.onClick.AddListener(OnDirectPlayClicked);
        }
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(OnResetProgressClicked);
            resetButton.onClick.AddListener(OnResetProgressClicked);
        }
    }

    public void LoadAndDisplaySavedStats()
    {
        int coins = SaveLoad.GetSavedCoins();
        int gems = SaveLoad.GetSavedGems();
        int distance = SaveLoad.GetSavedDistance();

        if (totalCoinsText != null) totalCoinsText.text = coins.ToString();
        if (totalGemsText != null) totalGemsText.text = gems.ToString();
        if (bestDistanceText != null) bestDistanceText.text = distance.ToString() + "m";

        SetTmpText(totalCoinsTmp, coins.ToString());
        SetTmpText(totalGemsTmp, gems.ToString());
        SetTmpText(bestDistanceTmp, distance.ToString() + "m");
    }

    public void OnStartGameClicked()
    {
        Debug.Log("[MainMenuControl] START DASH Clicked! Loading StageSelect...");
        PlayClickSound();

        if (Application.CanStreamedLevelBeLoaded(stageSelectSceneName))
        {
            SceneManager.LoadScene(stageSelectSceneName);
        }
        else
        {
            SceneManager.LoadScene(gamePlaySceneName);
        }
    }

    public void OnDirectPlayClicked()
    {
        Debug.Log("[MainMenuControl] QUICK RUN Clicked! Loading GamePlay directly...");
        PlayClickSound();
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void OnResetProgressClicked()
    {
        PlayClickSound();
        SaveLoad.ResetData();
        LoadAndDisplaySavedStats();
    }

    public void OnQuitGameClicked()
    {
        PlayClickSound();
        Application.Quit();
        Debug.Log("[Airport Dash] Game closed.");
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, 0.9f);
        }
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
