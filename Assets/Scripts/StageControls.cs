using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// StageControls: Handles stage selection screen, focusing on Airport Dash.
/// Features stage preview turntable, stage info, and navigation to Information/Gameplay.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public class StageControls : MonoBehaviour
{
    [Header("Stage Visuals")]
    [Tooltip("3D revolving preview cube/diorama showcasing airport terminal theme")]
    public Transform stagePreviewCube;
    public float rotationSpeed = 35f;

    [Header("Stage Info UI")]
    public Text stageTitleText;
    public Text stageDescriptionText;
    public Button playButton;
    public Button backButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Scene Destinations")]
    public string informationSceneName = "Information";
    public string mainMenuSceneName = "MainMenu";
    public string gamePlaySceneName = "GamePlay";

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

        AutoBindButtons();
    }

    private void Start()
    {
        AutoBindButtons();

        if (stageTitleText != null)
        {
            stageTitleText.text = "AIRPORT DASH";
        }
        if (stageDescriptionText != null)
        {
            stageDescriptionText.text = "Dash through the international terminal concourse! Dodge luggage piles, baggage carts, and boarding stanchions.";
        }
    }

    private void Update()
    {
        // Smoothly rotate the stage preview diorama cube
        if (stagePreviewCube != null)
        {
            stagePreviewCube.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.World);
        }

        // Keyboard navigation fallback
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayStageClicked();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
        }
    }

    public void AutoBindButtons()
    {
        if (playButton == null)
        {
            var btnObj = GameObject.Find("PlayButton");
            if (btnObj != null) playButton = btnObj.GetComponent<Button>();
        }
        if (backButton == null)
        {
            var btnObj = GameObject.Find("BackButton");
            if (btnObj != null) backButton = btnObj.GetComponent<Button>();
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayStageClicked);
            playButton.onClick.AddListener(OnPlayStageClicked);
        }
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackClicked);
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    public void OnPlayStageClicked()
    {
        Debug.Log("[StageControls] PLAY STAGE Clicked! Loading Information / GamePlay...");
        PlaySound();
        if (Application.CanStreamedLevelBeLoaded(informationSceneName))
        {
            SceneManager.LoadScene(informationSceneName);
        }
        else
        {
            SceneManager.LoadScene(gamePlaySceneName);
        }
    }

    public void OnBackClicked()
    {
        Debug.Log("[StageControls] BACK Clicked! Returning to MainMenu...");
        PlaySound();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, 0.9f);
        }
    }
}
